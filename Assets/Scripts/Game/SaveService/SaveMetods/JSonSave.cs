
using System.IO;
using Newtonsoft.Json;
using UnityEngine;

public class JSonSave : ISaveMetod
{
    private const string _fileFormat = ".json";
    public string FileFormat => _fileFormat;

    public object Load(string path)
    {
        if (File.Exists(path) == true)
        {
            string json = File.ReadAllText(path);
            Debug.Log($"File is Loaded  {path}");
            return JsonConvert.DeserializeObject(json);
        }
        else
        {
            Debug.Log("File is null");
            return null;
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