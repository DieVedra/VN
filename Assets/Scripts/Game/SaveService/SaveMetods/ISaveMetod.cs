
public interface ISaveMetod
{
    string FileFormat { get; }
    object Load(string path);
    void Save(string path, object data);
}
