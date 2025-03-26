using UnityEngine;
using System.Collections.Generic;
public class BackgroundEditMode : Background
{
    [SerializeField] private SpriteRenderer _spriteRendererPrefab;
    [SerializeField] private List<BackgroundContentEditMode> Backgrounds;

    public override void Construct(DisableNodesContentEvent disableNodesContentEvent, ISetLighting setLighting)
    {
        List<BackgroundContent> backgroundContent = new List<BackgroundContent>(Backgrounds.Count);
        for (int i = 0; i < Backgrounds.Count; i++)
        {
            backgroundContent.Add(Backgrounds[i]);
        }
        BaseConstruct(backgroundContent);
        foreach (var content in Backgrounds)
        {
            content.Construct(disableNodesContentEvent, setLighting, DurationMovementDuringDialogue);
            content.SetPrefab(_spriteRendererPrefab);
        }
    }
}