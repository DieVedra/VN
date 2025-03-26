using System.Collections.Generic;
using UnityEngine;

public class BackgroundBuildMode : Background
{
    [SerializeField] private List<BackgroundContentBuildMode> Backgrounds;
    
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
        }
    }
}