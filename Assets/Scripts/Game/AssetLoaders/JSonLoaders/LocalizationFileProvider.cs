using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;

public class LocalizationFileProvider : JSonAssetLoader
{
    public bool IsLoading { get; private set; }
    public async UniTask<Dictionary<string, string>> LoadLocalizationFile(string name)
    {
        IsLoading = true;
        string jsonText = await Load(name);
        
        if (!string.IsNullOrEmpty(jsonText))
        {
            Dictionary<string, string> result = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonText);
            Unload();
            IsLoading = false;
            return result;
        }
        else
        {
            Unload();
            IsLoading = false;
            return default;
        }
    }
}