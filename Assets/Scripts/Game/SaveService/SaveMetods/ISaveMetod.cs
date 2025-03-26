using System;
public interface ISaveMetod
{
    string FileFormat { get; }
    object Load(string path);
    void Save(string path, object data);
    
    //void Save(string key, object data, Action<bool> callBack = null);
    //void LoadMainMenu<T>(string key, Action<T> callBack);
}
