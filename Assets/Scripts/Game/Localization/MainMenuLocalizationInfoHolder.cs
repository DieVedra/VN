
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MainMenuLocalizationInfoHolder", menuName = "MainMenuLocalizationInfoHolder", order = 51)]
public class MainMenuLocalizationInfoHolder : ScriptableObject
{
    [SerializeField] private List<MyLanguageName> _languageNames;
    public IReadOnlyList<MyLanguageName> LanguageNames => _languageNames;
}