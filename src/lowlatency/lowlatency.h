#pragma once
// lowlatency.h
// Public header for the low-latency timing/scheduling shim.

#ifndef LOWLATENCY_H
#define LOWLATENCY_H

#include <stdint.h>

#ifdef __cplusplus
extern "C" {
#endif

    // Export/import macro
#if defined(_WIN32)
#if defined(LL_BUILD) || defined(LOWLATENCY_EXPORTS)
#define LL_API __declspec(dllexport)
#else
#define LL_API __declspec(dllimport)
#endif
#else
#define LL_API __attribute__((visibility("default")))
#endif

// Initialize the module (sets up timer granularity on Windows).
    LL_API void     ll_init(void);

    // Monotonic time in nanoseconds (high resolution).
    LL_API uint64_t ll_time_ns(void);

    // Sleep until an ABSOLUTE monotonic time (ns). Busy-spins for the last ~<200µs.
    LL_API void     ll_sleep_until_ns(uint64_t target_ns);

    // Raise the current thread’s priority to a real-time-ish level.
    // Returns 0 on success, -1 on failure.
    LL_API int      ll_set_realtime_thread(void);

    // Pin the current thread to a core (0-based). Returns 0 on success, -1 on failure.
    // On macOS this is a no-op that returns 0.
    LL_API int      ll_set_affinity(int core_index);

#ifdef __cplusplus
} // extern "C"
#endif

#endif // LOWLATENCY_H
