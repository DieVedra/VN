using System.Collections.Generic;
using UnityEditor;
public class FolderCreator
{
    public const char Separator = '/';
    public void CreateFolder(string path)
    {
        string[] names = path.Split(Separator);
        List<string> paths = new List<string>(names.Length);
        for (int i = 0; i < names.Length; i++)
        {
            if (i == 0)
            {
                paths.Add(names[i]);
            }
            else
            {
                string newPath = names[0];
                for (int j = 0; j < i; j++)
                {
                    newPath = $"{newPath}{Separator}{names[j+1]}";
                }
                paths.Add(newPath);
            }
        }
        for (int i = 0; i < paths.Count; ++i)
        {
            if (AssetDatabase.IsValidFolder(paths[i]) == false)
            {
                AssetDatabase.CreateFolder(paths[i - 1],names[i]);
            }
        }
        AssetDatabase.Refresh();
    }
}