using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UniRx;
using UnityEngine;

[NodeTint("#006C17")]
public class NarrativeNode : BaseNode, ILocalizable
{
	[SerializeField] protected LocalizationString _localizationText;

	protected NarrativePanelUIHandler NarrativePanelUI;
	protected CompositeDisposable CompositeDisposable;
	public void ConstructMyNarrativeNode(NarrativePanelUIHandler narrativePanelUI)
	{
		NarrativePanelUI = narrativePanelUI;
	}

	public override async UniTask Enter(bool isMerged = false)
	{
		CancellationTokenSource = new CancellationTokenSource();
		CompositeDisposable = SetLocalizationChangeEvent.SubscribeWithCompositeDisposable(() =>
		{
			NarrativePanelUI.SetText(_localizationText.DefaultText);
		});
		IsMerged = isMerged;
		if (isMerged == false)
		{
			ButtonSwitchSlideUIHandler.ActivateSkipTransition(SkipEnterTransition);
		}

		await NarrativePanelUI.EmergenceNarrativePanelInPlayMode(_localizationText.DefaultText, CancellationTokenSource.Token);
		TryActivateButtonSwitchToNextSlide();
	}

	public override async UniTask Exit()
	{
		CancellationTokenSource = new CancellationTokenSource();
		await NarrativePanelUI.DisappearanceNarrativePanelInPlayMode(CancellationTokenSource.Token);
		CompositeDisposable?.Dispose();
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
		NarrativePanelUI.NarrativeInEditMode(_localizationText);
	}

	protected override void TryActivateButtonSwitchToNextSlide()
	{
		if (IsMerged == false)
		{
			ButtonSwitchSlideUIHandler.ActivateButtonSwitchToNextNode();
		}
	}
}
