
public interface ISaveMetod
{
    string FileFormat { get; }
    T Load<T>(string path);
    void Save(string path, object data);
}
