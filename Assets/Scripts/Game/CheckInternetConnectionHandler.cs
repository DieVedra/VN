
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public class CheckInternetConnectionHandler
{
    // private string _testUrl = "https://config.uca.cloud.unity3d.com";
    private string _testUrl = "https://google.com";
    private int _timeoutSeconds = 5;
    private int _maxRetries = 3;
    private int _milisecondsDelay = 1000;
    private bool _result;
    public bool Result => _result;
    
    public async UniTask CheckInternetConnection()
    {
        _result = false;
        if (Application.internetReachability != NetworkReachability.NotReachable)
        {
            for (int attempt = 1; attempt <= _maxRetries; attempt++)
            {
                using UnityWebRequest request = UnityWebRequest.Head(_testUrl);
                request.timeout = _timeoutSeconds;
                var operation = request.SendWebRequest();

                while (!operation.isDone)
                {
                    await UniTask.Yield();
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Debug.Log($"[CheckInternetConnection] Интернет OK (попытка {attempt})");
                    _result = true;
                    break;
                }
                else
                {
                    Debug.Log($"[CheckInternetConnection] Попытка {attempt} неудачна: {request.error}");
                }

                if (attempt < _maxRetries)
                {
                    await UniTask.Delay(_milisecondsDelay);
                }
            }
        }
        Debug.Log($"[CheckInternetConnection] Result: {_result}");

    }
}