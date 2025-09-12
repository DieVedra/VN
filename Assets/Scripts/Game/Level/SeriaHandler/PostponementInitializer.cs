using System;
using System.Collections.Generic;

public class PostponementInitializer
{
    protected readonly Background Background;
    protected readonly LevelUIProviderEditMode LevelUIProvider;
    protected readonly Sound Sound;
    protected readonly IGameStatsProvider GameStatsProvider;
    protected readonly CharacterViewer CharacterViewer;
    protected readonly Wallet Wallet;
    private readonly WardrobeCharacterViewer _wardrobeCharacterViewer;
    protected List<CharacterNode> CharacterNodeForPostponementInit;
    protected List<CustomizationNode> CustomizationNodeForPostponementInit;

    protected PostponementInitializer(Background background, LevelUIProviderEditMode levelUIProvider,
        Sound sound, IGameStatsProvider gameStatsProvider, CharacterViewer characterViewer, Wallet wallet, WardrobeCharacterViewer wardrobeCharacterViewer)
    {
        Background = background;
        LevelUIProvider = levelUIProvider;
        Sound = sound;
        GameStatsProvider = gameStatsProvider;
        CharacterViewer = characterViewer;
        Wallet = wallet;
        _wardrobeCharacterViewer = wardrobeCharacterViewer;
    }
    public void TryPostponementInit(Func<IReadOnlyList<Character>> charactersForThisSeria, int seriaIndex)
    {
        if (CharacterNodeForPostponementInit != null && CharacterNodeForPostponementInit.Count > 0)
        {
            for (int i = 0; i < CharacterNodeForPostponementInit.Count; i++)
            {
                InitCharactersNode(charactersForThisSeria.Invoke(), CharacterNodeForPostponementInit[i]);
            }
        }

        if (CustomizationNodeForPostponementInit != null && CustomizationNodeForPostponementInit.Count > 0)
        {
            for (int i = 0; i < CustomizationNodeForPostponementInit.Count; i++)
            {
                InitCustomizationNode( GetCustomizableCharactersForThisSeria(charactersForThisSeria.Invoke()), CustomizationNodeForPostponementInit[i], seriaIndex);
            }
        }
        CharacterNodeForPostponementInit = null;
        CustomizationNodeForPostponementInit = null;
    }

    private void InitCharactersNode(IReadOnlyList<Character> charactersForThisSeria, CharacterNode characterNode)
    {
        characterNode.ConstructMyCharacterNode(charactersForThisSeria,
            LevelUIProvider.CharacterPanelUIHandler, Background, CharacterViewer);
    }
    private void InitCustomizationNode(IReadOnlyList<CustomizableCharacter> customizableCharactersForThisSeria,
        CustomizationNode customizationNode, int seriaIndex)
    {
        customizationNode.ConstructMyCustomizationNode(
            LevelUIProvider.CustomizationCharacterPanelUIHandler,
            LevelUIProvider.CustomizationCurtainUIHandler,
            customizableCharactersForThisSeria,
            Background,Sound,
            GameStatsProvider,
            Wallet, _wardrobeCharacterViewer, seriaIndex);
    }

    private IReadOnlyList<CustomizableCharacter> GetCustomizableCharactersForThisSeria(IReadOnlyList<Character> charactersForThisSeria)
    {
        List<CustomizableCharacter> customizableCharacters = new List<CustomizableCharacter>();
        for (int i = 0; i < charactersForThisSeria.Count; i++)
        {
            if (charactersForThisSeria[i] is CustomizableCharacter customizableCharacter)
            {
                customizableCharacters.Add(customizableCharacter);
            }
        }
        return customizableCharacters;
    }
}