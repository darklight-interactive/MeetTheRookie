using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.Analytics;

public class AnalyticsManager : MonoBehaviourSingleton<AnalyticsManager> {
    // Start is called before the first frame update
    async void Start()
    {
        await UnityServices.InitializeAsync();

        AnalyticsService.Instance.StartDataCollection();
        PerformanceReporting.enabled = true;
    }
}
