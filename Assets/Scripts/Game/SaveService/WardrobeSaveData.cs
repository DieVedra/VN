using System;
using System.Collections.Generic;

[Serializable]
public class WardrobeSaveData
{
    public int BodyIndex;
    public int ClothesIndex;
    public int SwimsuitsIndex;
    public int HairstyleIndex;
    
    
    public List<int> OpenedClothesIndexes;
    public List<int> OpenedSwimsuitsIndexes;
    public List<int> OpenedHairstyleIndexes;
    
    public List<int> PurchasedClothesIndexes;
    public List<int> PurchasedSwimsuitsIndexes;
    public List<int> PurchasedHairstyleIndexes;
}