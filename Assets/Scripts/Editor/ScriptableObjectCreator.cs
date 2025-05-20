

using UnityEditor;
using UnityEngine;

namespace MyNamespace
{
    public class ScriptableObjectCreator
    {
        public const char Separator = '/';

        public T CreateScriptableObjectAsset<T>(string path, string name)  where T : ScriptableObject
        {
            string newPath = path;
            if (name[0] != Separator)
            {
                newPath = $"{path}{Separator}{name}";
            }
            else
            {
                newPath = $"{path}{name}";
            }
            T scriptableObject = ScriptableObject.CreateInstance<T>();
            AssetDatabase.CreateAsset(scriptableObject, newPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            return scriptableObject;
        }
    }
}