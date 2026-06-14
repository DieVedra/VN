using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class BinarySave : ISaveMethod
{
    private const string _fileName = "/Save";
    private const string _fileFormat = ".dat";
    private string _savePath;
    private BinaryFormatter _binaryFormatter;


    public UniTask Construct()
    {
        _binaryFormatter = new BinaryFormatter();
        _savePath = Path.Combine(Application.dataPath + _fileName + _fileFormat);
        return default;
    }

    public async UniTask<T> Load<T>()
    {
        if (File.Exists(_savePath) == true)
        {
            using FileStream file = new FileStream(_savePath, FileMode.Open);
            T result = (T)_binaryFormatter.Deserialize(file);
            Debug.Log($"File is Loaded  {_savePath}");
            return result;
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
            using (FileStream file = new FileStream(_savePath, FileMode.Create))
            {
                Debug.Log("File Saved");

                _binaryFormatter.Serialize(file, data as object);
            }
        }
    }
}
