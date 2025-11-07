using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

[NodeTint("#006C17")]
public class NarrativeNode : BaseNode, ILocalizable
{
	[SerializeField] private LocalizationString _localizationText;

	private NarrativePanelUIHandler _narrativePanelUI;
	private CompositeDisposable _compositeDisposable;
	public void ConstructMyNarrativeNode(NarrativePanelUIHandler narrativePanelUI)
	{
		_narrativePanelUI = narrativePanelUI;
	}

	public override async UniTask Enter(bool isMerged = false)
	{
		CancellationTokenSource = new CancellationTokenSource();
		_compositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
		{
			_narrativePanelUI.SetText(_localizationText.DefaultText);
		});
		IsMerged = isMerged;
		if (isMerged == false)
		{
			ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
		}

		await _narrativePanelUI.EmergenceNarrativePanelInPlayMode(_localizationText.DefaultText, CancellationTokenSource.Token);
		TryActivateButtonSwitchToNextSlide();
	}

	public override async UniTask Exit()
	{
		CancellationTokenSource = new CancellationTokenSource();
		await _narrativePanelUI.DisappearanceNarrativePanelInPlayMode(CancellationTokenSource.Token);
		_compositeDisposable.Dispose();
	}

	public IReadOnlyList<LocalizationString> GetLocalizableContent()
	{
		return new[] {_localizationText};
	}

	public override void SkipEnterTransition()
	{
		CancellationTokenSource.Cancel();
		SetInfoToView();
		TryActivateButtonSwitchToNextSlide();
	}

	protected override void SetInfoToView()
	{
		_narrativePanelUI.NarrativeInEditMode(_localizationText);
	}

	protected override void TryActivateButtonSwitchToNextSlide()
	{
		if (IsMerged == false)
		{
			ButtonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();
		}
	}
}
