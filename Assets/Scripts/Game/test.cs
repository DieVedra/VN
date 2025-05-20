
using NaughtyAttributes;
using UnityEngine;

public class test : MonoBehaviour
{
    [SerializeField] private LocalizationString _localizationString1;
    [SerializeField] private LocalizationString _localizationString2;
    [SerializeField] private LocalizationString _localizationString3;
    [SerializeField] private LocalizationString _localizationString00 = "00000";
    private LocalizationString _localizationString4 = "Загр";
    private LocalizationString _localizationString5 = "Загр2";
    private LocalizationString _localizationString6 = "Загр2";
    
    
    private ScriptableObjectAssetLoader _scriptableObjectAssetLoader;
    private AssetExistsHandler _assetExistsHandler;
    
    [Button()]
    private void test1()
    {
        Debug.Log($"LocalizationStringDictionary.Count {LocalizationString.LocalizationStringDictionary.Count}");

        foreach (var variable in LocalizationString.LocalizationStringDictionary)
        {
            Debug.Log($"{variable.Value}     {variable.Key}");

        }
        
    }
    
    [Button()]
    private void test2()
    {
        Debug.Log($"  {_localizationString1.DefaultText} {_localizationString1.Key}");
        Debug.Log($"  {_localizationString2.DefaultText} {_localizationString2.Key}");
        Debug.Log($"  {_localizationString3.DefaultText} {_localizationString3.Key}");
        Debug.Log($"  {_localizationString4.DefaultText} {_localizationString4.Key}");
        Debug.Log($"  {_localizationString5.DefaultText} {_localizationString5.Key}");
        Debug.Log($"  {_localizationString6.DefaultText} {_localizationString6.Key}");
    }
}