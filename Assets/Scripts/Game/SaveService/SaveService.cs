using System.IO;
using UnityEngine;

public class SaveService
{
    private const string _fileName = "/Save";
    private readonly string _savePath;
    private ISaveMetod _saveMethod;
    public SaveService()
    {
        _saveMethod = new BinarySave();
        
        _savePath = Path.Combine(Application.dataPath + _fileName + _saveMethod.FileFormat);


        // _savePath = Path.Combine(Application.persistentDataPath, _fileName + _saveMethod.FileFormat);
        
    }

    public SaveData LoadData()
    {
        return _saveMethod.Load(_savePath) as SaveData;
    }

    public void Save(SaveData saveData)
    {
        _saveMethod.Save(_savePath, saveData);
    }
    // public PlayerData GetPlayerConfigAfterLoading(int countConfigurations, int testMoney, bool saveOn)
    // {
    //     if (saveOn == true)
    //     {
    //         var objectData = _saveMethod.LoadMainMenu(_savePath);
    //         PlayerData playerData;
    //         if (objectData != null)
    //         {
    //             playerData = CreatePlayerConfig((Save)objectData);
    //         }
    //         else
    //         {
    //             playerData = CreatePlayerConfigDefault(countConfigurations, testMoney);
    //         }
    //         return playerData;
    //     }
    //     else
    //     {
    //         return CreatePlayerConfigDefault(countConfigurations, testMoney);
    //     }
    // }
    // public void SetPlayerDataToSaving(IPlayerData playerData, bool saveOn)
    // {
    //     if (saveOn == true)
    //     {
    //         _saveMethod.Save(_savePath, CreateSaveData(playerData));
    //     }
    // }
    // private PlayerData CreatePlayerConfig(Save saveData)
    // {
    //     CarConfigurationInParkingLot[] parkingLotIndexes = new CarConfigurationInParkingLot[saveData.SavesParkingsIndexes.Length];
    //     for (int i = 0; i < parkingLotIndexes.Length; i++)
    //     {
    //         parkingLotIndexes[i] = new CarConfigurationInParkingLot(
    //             saveData.SavesParkingsIndexes[i].EnginePowerCurrentIndex,
    //             saveData.SavesParkingsIndexes[i].GearRatioCurrentIndex,
    //             saveData.SavesParkingsIndexes[i].WheelsCurrentIndex,
    //             saveData.SavesParkingsIndexes[i].GunCurrentIndex,
    //             saveData.SavesParkingsIndexes[i].CorpusCurrentIndex,
    //             saveData.SavesParkingsIndexes[i].BoosterCurrentIndex,
    //             saveData.SavesParkingsIndexes[i].FuelQuantityCurrentIndex
    //             );
    //     }
    //     return new PlayerData(
    //         new Wallet(saveData.Money),
    //         new GarageConfig(parkingLotIndexes, saveData.CurrentSelectLotCarIndex, saveData.AvailableLotCarIndex),
    //         saveData.Level,
    //         saveData.Days,
    //         saveData.NewLevelHasBeenOpen,
    //         saveData.SoundOn,
    //         saveData.MusicOn
    //         );
    // }
    // private PlayerData CreatePlayerConfigDefault(int countConfigurations, int testMoney)
    // {
    //     CarConfigurationInParkingLot[] parkingLotIndexes = new CarConfigurationInParkingLot[countConfigurations];
    //     for (int i = 0; i < parkingLotIndexes.Length; i++)
    //     {
    //         parkingLotIndexes[i] = new CarConfigurationInParkingLot();
    //     }
    //     return new PlayerData(new Wallet(testMoney), new GarageConfig(parkingLotIndexes));
    // }
    // private Save CreateSaveData(IPlayerData playerData)
    // {
    //     SaveParkingIndexes[] savesParkingsIndexes = new SaveParkingIndexes[
    //         playerData.GarageConfig.CarConfigurationsInParkingLotsIndexes.Count];
    //     for (int i = 0; i < savesParkingsIndexes.Length; i++)
    //     {
    //         savesParkingsIndexes[i] = new SaveParkingIndexes
    //         {
    //             EnginePowerCurrentIndex = playerData.GarageConfig.CarConfigurationsInParkingLotsIndexes[i].EnginePowerIndex,
    //             GearRatioCurrentIndex = playerData.GarageConfig.CarConfigurationsInParkingLotsIndexes[i].GearRatioIndex,
    //             WheelsCurrentIndex = playerData.GarageConfig.CarConfigurationsInParkingLotsIndexes[i].WheelsIndex,
    //             GunCurrentIndex = playerData.GarageConfig.CarConfigurationsInParkingLotsIndexes[i].GunIndex,
    //             CorpusCurrentIndex = playerData.GarageConfig.CarConfigurationsInParkingLotsIndexes[i].CorpusIndex,
    //             BoosterCurrentIndex = playerData.GarageConfig.CarConfigurationsInParkingLotsIndexes[i].BoosterIndex,
    //             FuelQuantityCurrentIndex = playerData.GarageConfig.CarConfigurationsInParkingLotsIndexes[i].FuelQuantityIndex
    //         };
    //     }
    //     return new Save()
    //     {
    //         CurrentSelectLotCarIndex = playerData.GarageConfig.CurrentSelectLotCarIndex,
    //         AvailableLotCarIndex = playerData.GarageConfig.AvailableLotCarIndex,
    //         Money = playerData.Wallet.Money,
    //         Days = playerData.Days,
    //         Level = playerData.Level,
    //         NewLevelHasBeenOpen = playerData.NewLevelHasBeenOpen,
    //         SoundOn = playerData.SoundOn,
    //         MusicOn = playerData.MusicOn,
    //         SavesParkingsIndexes = savesParkingsIndexes
    //     };
    // }
}
