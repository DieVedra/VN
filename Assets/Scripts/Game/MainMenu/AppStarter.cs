
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class AppStarter
{
    private readonly MainMenuUIProvider _mainMenuUIProvider;
    public AppStarter(MainMenuUIProvider mainMenuUIProvider)
    {
        _mainMenuUIProvider = mainMenuUIProvider;
    }

    public async UniTask StartApp(StoriesProvider storiesProvider, LevelLoader levelLoader, Wallet wallet)
    {
        await Addressables.InitializeAsync();
        Transform parent = _mainMenuUIProvider.MainMenuUIView.transform;
        var loadScreenHandler = _mainMenuUIProvider.GetLoadScreenHandler();
        var loadIndicatorUIHandler = _mainMenuUIProvider.GetLoadIndicatorUIHandler();
        var blackFrameUIHandler = _mainMenuUIProvider.GetBlackFrameUIHandler();
        var settingsPanelButtonUIHandler = _mainMenuUIProvider.GetSettingsPanelButtonUIHandler();
        var shopMoneyButtonsUIHandler = _mainMenuUIProvider.GetShopMoneyButtonsUIHandler();
        var bottomPanelUIHandler = _mainMenuUIProvider.GetBottomPanelUIHandler();
        
        await loadScreenHandler.Init(parent);
        await loadIndicatorUIHandler.Init(parent);
        
        
        loadScreenHandler.Show();
        loadIndicatorUIHandler.StartIndicate(loadScreenHandler.ParentMask);
        blackFrameUIHandler.Open().Forget();
        await PrefabsProvider.Init();
        await UniTask.Delay(1000);
        
        RectTransform rectTransformBackground = await CreateBackground();
        
        await UniTask.WhenAll(
            bottomPanelUIHandler.Init(),
            shopMoneyButtonsUIHandler.Init(),
            settingsPanelButtonUIHandler.Init(),
            _mainMenuUIProvider.GetPlayStoryPanelHandler().Init(levelLoader),
            _mainMenuUIProvider.MainMenuUIView.MyScroll.Init(storiesProvider.Stories, _mainMenuUIProvider.GetPlayStoryPanelHandler(), levelLoader,1));
        
        
        rectTransformBackground.gameObject.SetActive(true);
        bottomPanelUIHandler.SubscribeButtons();
        settingsPanelButtonUIHandler.SubscribeButtonAndActivate();
        shopMoneyButtonsUIHandler.SubscribeButtonsAndSetResourcesIndicate();

        loadIndicatorUIHandler.Dispose();
        await loadScreenHandler.Hide();
        loadScreenHandler.Dispose();
    }
    private async UniTask<RectTransform> CreateBackground()
    {
        MenuBackgroundAssetProvider menuBackgroundAssetProvider = new MenuBackgroundAssetProvider();
        RectTransform rectTransformBackground = await menuBackgroundAssetProvider.LoadAsset(_mainMenuUIProvider.MainMenuUIView.transform);
        rectTransformBackground.SetAsFirstSibling();
        return rectTransformBackground;
    }
}