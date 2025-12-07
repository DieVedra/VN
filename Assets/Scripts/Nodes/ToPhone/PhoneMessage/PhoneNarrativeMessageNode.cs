using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using XNode;

public class PhoneNarrativeMessageNode : NarrativeNode
{
    [SerializeField] private bool _isReaded;

    private CustomizationCurtainUIHandler _curtainUIHandler;
    public bool IsEntered { get; private set; }
    public bool IsReaded => _isReaded;


    public void ConstructMyPhoneNarrativeNode(NarrativePanelUIHandler narrativePanelUI, CustomizationCurtainUIHandler curtainUIHandler)
    {
        _curtainUIHandler = curtainUIHandler;
        ConstructMyNarrativeNode(narrativePanelUI);
        IsEntered = false;
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        CancellationTokenSource = new CancellationTokenSource();
        CompositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
        {
            NarrativePanelUI.SetText(_localizationText.DefaultText);
        });
        _curtainUIHandler.Transform.gameObject.SetActive(true);
        _curtainUIHandler.CurtainImage.raycastTarget = true;

        await UniTask.WhenAll(
            _curtainUIHandler.CurtainImage.DOFade(PhoneAnimValues.FadeEndValue, PhoneAnimValues.Duration).WithCancellation(CancellationTokenSource.Token),
            NarrativePanelUI.EmergenceNarrativePanelInPlayMode(_localizationText.DefaultText, CancellationTokenSource.Token));
        IsEntered = true;
    }

    public override async UniTask Exit()
    {
        await UniTask.WhenAll(
            _curtainUIHandler.CurtainImage.DOFade(PhoneAnimValues.UnfadeEndValue, PhoneAnimValues.Duration).WithCancellation(CancellationTokenSource.Token),
            NarrativePanelUI.DisappearanceNarrativePanelInPlayMode(CancellationTokenSource.Token));
        _curtainUIHandler.Transform.gameObject.SetActive(false);
        _curtainUIHandler.CurtainImage.raycastTarget = false;
    }

#if UNITY_EDITOR
    protected override void SetInfoToView()
    {
        Debug.Log($"{_localizationText}");
    }
#endif
}