using UnityEngine;
using System.IO;
using Leguar.TotalJSON;
using PlayFab;
using PlayFab.ClientModels;

public class SaveManager : MonoBehaviour
{
    [SerializeField] private string filePath;

    private static PlayerData _playerData;

    private static readonly string keyWord = "1234567";

    public PlayerData PlayerData
    {
        get { return _playerData; }
    }

    private void Start()
    {
        LoadPlayerData();
        LoginToPlayFab();
    }

    public void SavePlayerData()
    {
        string serialisedDataString = JSON.Serialize(_playerData).CreateString();

        string encodeBase64String = EncodeBase64(serialisedDataString);
        string encryptedDataString = EncryptDecryptData(encodeBase64String);
        File.WriteAllText(filePath, encryptedDataString);
    }

    public void LoadPlayerData()
    {
        if(!File.Exists(filePath))
        {
            _playerData = new PlayerData();
            SavePlayerData();
        }

        string fileContents = File.ReadAllText(filePath);
        string decryptedDataString = EncryptDecryptData(fileContents);
        string decodeBase64String = DecodeBase64(decryptedDataString);

        _playerData = JSON.ParseString(decodeBase64String).Deserialize<PlayerData>();
    }

    private void LoginToPlayFab()
    {
        LoginWithCustomIDRequest request = new LoginWithCustomIDRequest()
        {
            CreateAccount = true,
            CustomId = PlayerData.uid,
        };

        PlayFabClientAPI.LoginWithCustomID(request, PlayFabLoginResult, PlayFabLoginError);
    }

    private void PlayFabLoginResult(LoginResult loginResult)
    {
        Debug.Log("PlayFab - Login Succeeded: " + loginResult.ToString());
    }

    private void PlayFabLoginError(PlayFabError loginError)
    {
        Debug.Log("PlayFab - Login Failed: " + loginError.ToString());
    }

    public string EncodeBase64(string input)
    {
        var bytes = System.Text.Encoding.UTF8.GetBytes(input);
        return System.Convert.ToBase64String(bytes);
    }

    public string DecodeBase64(string input)
    {
        var base64EncodedBytes = System.Convert.FromBase64String(input);
        return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
    }

    private string EncryptDecryptData(string dataString)
    {
        string result = "";

        for (int i = 0; i < dataString.Length; i++)
        {
            result += (char)(dataString[i] ^ keyWord[i % keyWord.Length]);
        }

        return result;
    }
}
