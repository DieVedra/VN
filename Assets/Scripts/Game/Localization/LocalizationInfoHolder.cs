
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LocalizationInfoHolder", menuName = "LocalizationInfoHolder", order = 51)]
public class LocalizationInfoHolder : ScriptableObject
{
    [SerializeField] private List<MyLanguageName> _languageNames;
    public IReadOnlyList<MyLanguageName> LanguageNames => _languageNames;
}