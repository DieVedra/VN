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

    public bool BuferCurrentClothesIsActive = false;
    public bool BuferCurrentSwimsuitsIsActive = false;
    public bool BuferCurrentHairstyleIsActive = false;
    
    public int BuferCurrentClothesIndex;
    public int BuferCurrentSwimsuitsIndex;
    public int BuferCurrentHairstyleIndex;
    
    public List<int> OpenedClothesIndexes;
    public List<int> OpenedSwimsuitsIndexes;
    public List<int> OpenedHairstyleIndexes;
    
    public List<int> PurchasedClothesIndexes;
    public List<int> PurchasedSwimsuitsIndexes;
    public List<int> PurchasedHairstyleIndexes;
}