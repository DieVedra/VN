using System;
using System.Collections.Generic;

[Serializable]
public class WardrobeSaveData
{
    public string NameKey;
    public int CurrentBodyIndex;
    public int CurrentClothesIndex;
    public int CurrentSwimsuitsIndex;
    public int CurrentHairstyleIndex;
    
    
    public List<int> OpenedClothesIndexes;
    public List<int> OpenedSwimsuitsIndexes;
    public List<int> OpenedHairstyleIndexes;
    
    public List<int> PurchasedClothesIndexes;
    public List<int> PurchasedSwimsuitsIndexes;
    public List<int> PurchasedHairstyleIndexes;
}