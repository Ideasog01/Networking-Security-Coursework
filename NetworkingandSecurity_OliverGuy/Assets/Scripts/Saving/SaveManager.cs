using UnityEngine;
using System.IO;
using Leguar.TotalJSON;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private string filePath;

    private static PlayerData _playerData;

    public PlayerData PlayerData
    {
        get { return _playerData; }
    }

    private void Start()
    {
        LoadPlayerData();
    }

    public void SavePlayerData()
    {
        string serialisedDataString = JSON.Serialize(_playerData).CreateString();
        File.WriteAllText(filePath, serialisedDataString);
    }

    public void LoadPlayerData()
    {
        if(File.Exists(filePath))
        {
            _playerData = new PlayerData();
            SavePlayerData();
        }

        string fileContents = File.ReadAllText(filePath);
        _playerData = JSON.ParseString(fileContents).Deserialize<PlayerData>();
    }
}
