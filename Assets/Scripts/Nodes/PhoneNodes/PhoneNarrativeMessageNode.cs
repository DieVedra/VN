using System.Threading;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using XNode;

public class PhoneNarrativeMessageNode : NarrativeNode
{
    private CustomizationCurtainUIHandler _curtainUIHandler;
    // public PhoneMessageNode PhoneMessageNode { get; private set; }
    public bool IsEntered { get; private set; }

    public void ConstructMyPhoneNarrativeNode(NarrativePanelUIHandler narrativePanelUI, CustomizationCurtainUIHandler curtainUIHandler)
    {
        _curtainUIHandler = curtainUIHandler;
        ConstructMyNarrativeNode(narrativePanelUI);
        IsEntered = false;
    }

    public override async UniTask Enter(bool isMerged = false)
    {
        Debug.Log($"Enter");
        CancellationTokenSource = new CancellationTokenSource();
        CompositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
        {
            NarrativePanelUI.SetText(_localizationText.DefaultText);
        });
        // TryFindConnectedPhoneMessageNode(OutputPortBaseNode);
        // _curtainUIHandler.SetCurtainUnderTargetPanel(NarrativePanelUI.RectTransform, ++siblig);
        // NarrativePanelUI.SetSibling(++siblig);
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
        // _curtainUIHandler.ResetSibling();
        // switch (GetNextNode())
        // {
        //     case PhoneNarrativeMessageNode phoneNarrativeMessageNode:
        //         
        //         break;
        //     case ChoicePhoneNode choicePhoneNode:
        //         
        //         break;
        // }
        // NarrativePanelUI.ResetSibling();
    }

#if UNITY_EDITOR
    protected override void SetInfoToView()
    {
        Debug.Log($"{_localizationText}");
    }
#endif

    // private void TryFindConnectedPhoneMessageNode(NodePort outputPort)
    // {
    //     for (int i = 0; i < outputPort.GetConnections().Count; i++)
    //     {
    //         if (outputPort.GetConnection(i).node is PhoneMessageNode phoneMessageNode)
    //         {
    //             PhoneMessageNode = phoneMessageNode;
    //             break;
    //         }
    //         else
    //         {
    //             PhoneMessageNode = null;
    //         }
    //     }
    // }
}