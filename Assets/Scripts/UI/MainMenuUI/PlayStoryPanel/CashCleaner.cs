
public class CashCleaner
{
    private readonly AssetExistsHandler _assetExistsHandler = new AssetExistsHandler();
    private readonly LocalizationInfoHolder _localizationInfoHolder;

    public CashCleaner(LocalizationInfoHolder localizationInfoHolder)
    {
        _localizationInfoHolder = localizationInfoHolder;
    }

    public void CleanCashStory(string storyName)
    {
        // GameSeriesProvider.NodeGraphsHandlerSeriaName
        // CharacterProviderBuildMode.CharactersProviderName
        // CharacterProviderBuildMode.CharactersDataProviderName
        // SeriaGameStatsProviderBuild.SeriaGameStatsProviderName
        // AudioClipProvider.NameAmbientAsset
        // AudioClipProvider.NameMusicAsset
        
        // BackgroundDataProvider.Locations
        // BackgroundDataProvider.AdditionalImages
        // BackgroundDataProvider.Arts
        // BackgroundDataProvider.Compressed
        // BackgroundDataProvider.BackgroundDataSeriaNameAsset
        // BackgroundDataProvider.WardrobeBackgroundDataNameAsset
        // PhoneProviderInBuildMode.NameDataProviderAsset
        // PhoneProviderInBuildMode.NameContactsToSeriaProviderAsset
        // LevelLocalizationProvider.NameLevelLocalizationAsset
    }

    public void CleanAllCash()
    {
        
    }
}