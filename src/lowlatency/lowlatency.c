// lowlatency.c
// Cross-platform tiny latency helpers.
// Build: Windows (cl): cl /O2 /LD lowlatency.c /link user32.lib winmm.lib
//        Linux (gcc): gcc -O2 -shared -fPIC -o liblowlatency.so lowlatency.c -lpthread
//        macOS (clang): clang -O2 -shared -fPIC -o liblowlatency.dylib lowlatency.c

#include <stdint.h>

#if defined(_WIN32)
#define WIN32_LEAN_AND_MEAN
#include <windows.h>
#include <processthreadsapi.h>
#include <mmsystem.h>
#pragma comment(lib, "winmm.lib")

static double qpc_freq_inv = 0.0;
__declspec(dllexport) void ll_init() {
    LARGE_INTEGER f; QueryPerformanceFrequency(&f);
    qpc_freq_inv = 1.0 / (double)f.QuadPart;
    timeBeginPeriod(1); // improve timer granularity (system-wide)
}
__declspec(dllexport) uint64_t ll_time_ns() {
    LARGE_INTEGER c; QueryPerformanceCounter(&c);
    double s = (double)c.QuadPart * qpc_freq_inv;
    return (uint64_t)(s * 1e9);
}
__declspec(dllexport) void ll_sleep_until_ns(uint64_t target_ns) {
    for (;;) {
        uint64_t now = ll_time_ns();
        if (now >= target_ns) return;
        uint64_t delta = target_ns - now;
        if (delta > 2000000ULL) { // >2 ms: sleep 1 ms
            Sleep(1);
        }
        else if (delta > 200000ULL) { // >0.2 ms: yield
            Sleep(0);
        }
        else {
            // short busy wait
            while (ll_time_ns() < target_ns) {
                YieldProcessor();
            }
            return;
        }
    }
}
__declspec(dllexport) int ll_set_realtime_thread() {
    // elevate priority; avoid true hard realtime which can starve the OS
    HANDLE th = GetCurrentThread();
    BOOL ok = SetThreadPriority(th, THREAD_PRIORITY_TIME_CRITICAL);
    return ok ? 0 : -1;
}
__declspec(dllexport) int ll_set_affinity(int core_index) {
    if (core_index < 0) return -1;
    DWORD_PTR mask = (DWORD_PTR)1 << core_index;
    HANDLE th = GetCurrentThread();
    return SetThreadAffinityMask(th, mask) ? 0 : -1;
}

#elif defined(__APPLE__)
#include <mach/mach_time.h>
#include <mach/thread_policy.h>
#include <pthread.h>
static mach_timebase_info_data_t s_timebase;

__attribute__((visibility("default"))) void ll_init() {
    mach_timebase_info(&s_timebase);
}
__attribute__((visibility("default"))) uint64_t ll_time_ns() {
    uint64_t t = mach_absolute_time();
    return t * s_timebase.numer / s_timebase.denom;
}
__attribute__((visibility("default"))) void ll_sleep_until_ns(uint64_t target_ns) {
    // mach_wait_until expects absolute "mach time units"
    uint64_t target_abs = target_ns * s_timebase.denom / s_timebase.numer;
    while (mach_absolute_time() < target_abs) {
        // sleep until absolute
        // On newer macOS you can call mach_wait_until; fall back to short busy wait if it fails
        kern_return_t kr = mach_wait_until(target_abs);
        if (kr == 0) return;
        // else spin a bit
    }
}
__attribute__((visibility("default"))) int ll_set_realtime_thread() {
    thread_time_constraint_policy_data_t policy;
    policy.period = 1000000; // 1 ms
    policy.computation = 500000;  // 0.5 ms
    policy.constraint = 2000000; // 2 ms
    policy.preemptible = 1;
    kern_return_t kr = thread_policy_set(mach_thread_self(),
        THREAD_TIME_CONSTRAINT_POLICY,
        (thread_policy_t)&policy,
        THREAD_TIME_CONSTRAINT_POLICY_COUNT);
    return (kr == KERN_SUCCESS) ? 0 : -1;
}
__attribute__((visibility("default"))) int ll_set_affinity(int core_index) {
    // macOS doesn't expose stable core indices in a friendly way; no-op is fine.
    (void)core_index; return 0;
}

#else // Linux/Unix
#include <time.h>
#include <pthread.h>
#include <sched.h>
#include <unistd.h>

__attribute__((visibility("default"))) void ll_init() {}
static uint64_t timespec_to_ns(const struct timespec* ts) {
    return (uint64_t)ts->tv_sec * 1000000000ULL + (uint64_t)ts->tv_nsec;
}
__attribute__((visibility("default"))) uint64_t ll_time_ns() {
    struct timespec ts; clock_gettime(CLOCK_MONOTONIC, &ts);
    return timespec_to_ns(&ts);
}
__attribute__((visibility("default"))) void ll_sleep_until_ns(uint64_t target_ns) {
    struct timespec ts;
    ts.tv_sec = target_ns / 1000000000ULL;
    ts.tv_nsec = target_ns % 1000000000ULL;
    // try absolute sleep, then busy for the tail
    clock_nanosleep(CLOCK_MONOTONIC, TIMER_ABSTIME, &ts, NULL);
    while (ll_time_ns() < target_ns) { /* busy tail */ }
}
__attribute__((visibility("default"))) int ll_set_realtime_thread() {
    struct sched_param sp; sp.sched_priority = 10; // modest RT prio
    if (pthread_setschedparam(pthread_self(), SCHED_FIFO, &sp) != 0) return -1;
    return 0;
}
__attribute__((visibility("default"))) int ll_set_affinity(int core_index) {
    if (core_index < 0) return -1;
    cpu_set_t set; CPU_ZERO(&set); CPU_SET(core_index, &set);
    return pthread_setaffinity_np(pthread_self(), sizeof(set), &set);
}
#endif
