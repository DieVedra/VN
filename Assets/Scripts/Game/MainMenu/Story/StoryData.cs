﻿
using System.Collections.Generic;
[System.Serializable]
public class StoryData
{
    public string NameAsset;
    public int CurrentSeriaIndex;
    public int CurrentNodeGraphIndex;
    public int CurrentNodeIndex;
    public int CurrentProgressPercent;
    public bool IsLiked;
    public int MyIndex;

    public SaveStat[] Stats;


    public StoryData(string nameAsset)
    {
        NameAsset = nameAsset;
        CurrentSeriaIndex = 0;
        CurrentNodeGraphIndex = 0;
        CurrentNodeIndex = 0;
        CurrentProgressPercent = 0;
        IsLiked = false;
    }
}