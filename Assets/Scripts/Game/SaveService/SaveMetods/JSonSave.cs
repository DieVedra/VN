
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class JSonSave : ISaveMetod
{
    private const string _fileFormat = ".json";
    public string FileFormat => _fileFormat;

    public T Load<T>(string path)
    {
        if (File.Exists(path) == true)
        {
            string json = File.ReadAllText(path);
            Debug.Log($"File is Loaded  {path}");
            return JsonConvert.DeserializeObject<T>(json);
        }
        else
        {
            Debug.Log("File is null");
            return default;
        }
    }

    public void Save(string path, object data)
    {
        if (data != null)
        {
            var settings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented,
                NullValueHandling = NullValueHandling.Ignore
            };
            string json = JsonConvert.SerializeObject(data, settings);
            File.WriteAllText(path, json);
            Debug.Log($"File Saved: {path}");
        }
    }
}