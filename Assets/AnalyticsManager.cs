using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Profiling;

public class AnalyticsManager : MonoBehaviourSingleton<AnalyticsManager> {
    // Start is called before the first frame update
    async void Start()
    {
#if UNITY_EDITOR || DEBUG
        Debug.Log("GAKSK");
        Profiler.logFile = "profilerLog";
        Profiler.enabled = true;
        Profiler.enableBinaryLog = true;

        await UnityServices.InitializeAsync();

        AnalyticsService.Instance.StartDataCollection();
        PerformanceReporting.enabled = true;
#endif
    }
}
