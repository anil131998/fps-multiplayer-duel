using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using TMPro;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField nameTxt;
    [SerializeField] private GameObject settingsPanel;

    private void Awake()
    {
        if (!PlayerPrefs.HasKey("Name"))
        {
            PlayerPrefs.SetString("Name", "Noname");
        }

        PhotonNetwork.AutomaticallySyncScene = true;
    }

    private void Start()
    {
        nameTxt.text = PlayerPrefs.GetString("Name");

        UpdatePlayerNickName();
        SetDefaultCustomProperties();
    }


    #region uiUpdates
    public void UpdateName()
    {
        PlayerPrefs.SetString("Name", nameTxt.text);
        UpdatePlayerNickName();
    }
    public void OpenSettings()
    {
        settingsPanel.SetActive(true);
    }
    public void CloseGame()
    {
        Application.Quit();
    }
    #endregion


    #region SettingDefaults

    private void UpdatePlayerNickName()
    {
        string playerName = PlayerPrefs.GetString("Name");
        PhotonNetwork.LocalPlayer.NickName = playerName;
    }

    private void SetDefaultCustomProperties()
    {
        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
        
        if (!hash.ContainsKey("isReady"))
            hash.Add("isReady", false);
        else
            hash["isReady"] = false;
      
        if (!hash.ContainsKey("TeamNr"))
            hash.Add("TeamNr", 0);
        else
            hash["TeamNr"] = 0;

        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    #endregion


    #region matchmaking
    public void FindMatch()
    {
        PhotonNetwork.JoinRandomOrCreateRoom();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        Debug.Log(message);
    }

    public override void OnJoinedRoom()
    {
        if (PhotonNetwork.LocalPlayer.IsMasterClient)
            PhotonNetwork.LoadLevel("GAMEROOM");
    }
    #endregion



}
