using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

[NodeTint("#006C17")]
public class NarrativeNode : BaseNode
{
	[SerializeField] private LocalizationString _localizationText;

	private NarrativePanelUIHandler _narrativePanelUI;
	public void ConstructMyNarrativeNode(NarrativePanelUIHandler narrativePanelUI)
	{
		_narrativePanelUI = narrativePanelUI;
		TryInitStringsToLocalization(_localizationText);
	}

	public override async UniTask Enter(bool isMerged = false)
	{
		CancellationTokenSource = new CancellationTokenSource();
		IsMerged = isMerged;
		if (isMerged == false)
		{
			ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
		}
		_narrativePanelUI.EmergenceNarrativePanelInPlayMode(_localizationText.DefaultText);
		await _narrativePanelUI.AnimationPanel.UnfadePanel(CancellationTokenSource.Token);
		await _narrativePanelUI.TextConsistentlyViewer.SetTextConsistently(CancellationTokenSource.Token, _localizationText.DefaultText);
		TryActivateButtonSwitchToNextSlide();
		_localizationText.SetText(_localizationText.DefaultText);
		TryInitStringsToLocalization(_localizationText);
	}

	public override async UniTask Exit()
	{
		CancellationTokenSource = new CancellationTokenSource();
		if (IsMerged == false)
		{
			ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipExitTransition);
		}
		await _narrativePanelUI.AnimationPanel.FadePanel(CancellationTokenSource.Token);
		_narrativePanelUI.DisappearanceNarrativePanelInPlayMode();
	}
	
	protected override void SetInfoToView()
	{
		_narrativePanelUI.NarrativeInEditMode(_localizationText);
	}
	public override void SkipEnterTransition()
	{
		CancellationTokenSource.Cancel();
		SetInfoToView();
		TryActivateButtonSwitchToNextSlide();
	}
	public override void SkipExitTransition()
	{
		CancellationTokenSource.Cancel();
		// _narrativePanelUI.DisappearanceNarrativePanelInPlayMode();
	}
	protected override void TryActivateButtonSwitchToNextSlide()
	{
		if (IsMerged == false)
		{
			ButtonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();
		}
	}
}
