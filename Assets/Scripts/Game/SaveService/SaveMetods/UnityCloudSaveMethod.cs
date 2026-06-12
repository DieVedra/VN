using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Authentication;
using Unity.Services.CloudSave;
using Unity.Services.Core;
using UnityEngine;

public class UnityCloudSaveMethod : ISaveMethod
{
    private const string _key = "save_data";
    
    public async UniTask Construct()
    {
        await UnityServices.InitializeAsync();
        
        if (!AuthenticationService.Instance.IsSignedIn)
        {
            await AuthenticationService.Instance.SignInAnonymouslyAsync();
        }
    }
    public async UniTask<T> Load<T>()
    {
        var data = await CloudSaveService.Instance.Data.LoadAsync(new HashSet<string> {_key});
        if (data.TryGetValue(_key, out string json))
        {
            return JsonConvert.DeserializeObject<T>(json);
        }
        return default;
    }

    public async UniTask Save(SaveData data)
    {
        var settingsJsonConvert = new JsonSerializerSettings
        {
            Formatting = Formatting.Indented,
            NullValueHandling = NullValueHandling.Ignore
        };
        string json = JsonConvert.SerializeObject(data as object, settingsJsonConvert);
        var saveData = new Dictionary<string, object>
        {
            {_key, json }
        };

        await CloudSaveService.Instance.Data.ForceSaveAsync(saveData);
        Debug.Log("Saved to cloud!");
    }
}