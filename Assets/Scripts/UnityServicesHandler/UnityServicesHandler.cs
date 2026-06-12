using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.CrashReportHandler;

public class UnityServicesHandler
{
    public async UniTask Construct(bool setAnalyticsEnabledKey)
    {
        await UnityServices.InitializeAsync();
        
        await AnalyticsService.Instance.SetAnalyticsEnabled(setAnalyticsEnabledKey);
        CrashReportHandler.SetUserMetadata("version", Application.version);
        CrashReportHandler.SetUserMetadata("player_id", GetPlayerId());
        var evt = new Unity.Services.Analytics.Internal.Event("menu_start", null);
        evt.Parameters.Set($"{Application.platform.ToString()}", Application.version);
        AnalyticsService.Instance.RecordInternalEvent(evt);
        AnalyticsService.Instance.Flush();
        
        Debug.Log("Unity Services ready!");
    }
    private string GetPlayerId()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }
}