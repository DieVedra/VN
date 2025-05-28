
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class MainMenuLocalizationFileProvider : JSonAssetLoader
{
    public async UniTask<Dictionary<string, string>> LoadLocalizationFile(string name)
    {
        string jsonText = await Load(name);
        
        if (!string.IsNullOrEmpty(jsonText))
        {
            Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);
            Unload();
            return result;
        }
        else
        {
            Unload();
            return default;
        }
    }
}