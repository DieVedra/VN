using System.IO;
using Cysharp.Threading.Tasks;
using Newtonsoft.Json;
using UnityEngine;

public class JSonSave : ISaveMethod
{
    private const string _fileName = "/Save";

    private readonly string _savePath;

    private const string _fileFormat = ".json";

    public JSonSave()
    {
        _savePath = Path.Combine(Application.dataPath + _fileName + _fileFormat);
    }

    public UniTask Construct()
    {
        throw new System.NotImplementedException();
    }

    public async UniTask<T> Load<T>()
    {
        if (File.Exists(_savePath) == true)
        {
            string json = File.ReadAllText(_savePath);
            Debug.Log($"File is Loaded  {_savePath}");
            return JsonConvert.DeserializeObject<T>(json);
        }
        else
        {
            Debug.Log("File is null");
            return default;
        }
    }

    public async UniTask Save(SaveData data)
    {
        if (data != null)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            string json = JsonConvert.SerializeObject(data as object, settings);
            File.WriteAllText(_savePath, json);
            Debug.Log($"File Saved: {_savePath}");
        }
    }
}