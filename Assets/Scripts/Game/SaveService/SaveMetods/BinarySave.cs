using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class BinarySave : ISaveMetod
{
    private readonly string _fileFormat = ".dat";

    private readonly BinaryFormatter _binaryFormatter;

    public BinarySave()
    {
        _binaryFormatter = new BinaryFormatter();
    }

    public string FileFormat => _fileFormat;

    public object Load(string path)
    {
        if (File.Exists(path) == true)
        {
            using FileStream file = new FileStream(path, FileMode.Open);
            Debug.Log($"File is Loaded  {path}");
            return _binaryFormatter.Deserialize(file);
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
            using (FileStream file = new FileStream(path, FileMode.Create))
            {
                Debug.Log("File Saved");

                _binaryFormatter.Serialize(file, data);
            }
        }
    }
}
