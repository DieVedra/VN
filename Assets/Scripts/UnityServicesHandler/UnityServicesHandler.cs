using Cysharp.Threading.Tasks;
using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.CrashReportHandler;

public class UnityServicesHandler
{
    public async UniTask Construct(StartConfig startConfig)
    {
        await UnityServices.InitializeAsync();
        
        await AnalyticsService.Instance.SetAnalyticsEnabled(startConfig.CrashlyticsStatus);
        if (startConfig.CrashlyticsStatus)
        {
        var evt = new Unity.Services.Analytics.Internal.Event("menu_start", null);
            evt.Parameters.Set($"{Application.platform.ToString()}", Application.version);
            AnalyticsService.Instance.RecordInternalEvent(evt);
              AnalyticsService.Instance.Flush();
        }

        if (startConfig.AnalyticsStatus)
        {
            CrashReportHandler.SetUserMetadata("version", Application.version);
            CrashReportHandler.SetUserMetadata("player_id", GetPlayerId());
        }


        Debug.Log("Unity Services ready!");
    }
    private string GetPlayerId()
    {
        return SystemInfo.deviceUniqueIdentifier;
    }
}