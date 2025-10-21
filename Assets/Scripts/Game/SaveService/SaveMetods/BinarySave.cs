using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BinarySave : ISaveMetod
{
    private const string _fileFormat = ".dat";

    private readonly BinaryFormatter _binaryFormatter;

    public BinarySave()
    {
        _binaryFormatter = new BinaryFormatter();
    }

    public string FileFormat => _fileFormat;

    public T Load<T>(string path)
    {
        if (File.Exists(path) == true)
        {
            using FileStream file = new FileStream(path, FileMode.Open);
            T result = (T)_binaryFormatter.Deserialize(file);
            Debug.Log($"File is Loaded  {path}");
            return result;
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
            using (FileStream file = new FileStream(path, FileMode.Create))
            {
                Debug.Log("File Saved");

                _binaryFormatter.Serialize(file, data);
            }
        }
    }
}
