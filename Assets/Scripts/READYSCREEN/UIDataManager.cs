using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class UIDataManager : MonoBehaviour
{
    [SerializeField] private PlayerUIData[] playerUiData;
    [SerializeField] private GameObject gameStartingScreen;

    private void Awake()
    {
        gameStartingScreen.SetActive(false);
    }

    private void Start()
    {
        foreach (PlayerUIData player in playerUiData)
        {
            player.UpdateUI();
        }
    }

    public void UpdateUIPlayerList()
    {
        Player[] players = PhotonNetwork.PlayerList;
        for (int i = 0; i < players.Length; i++) 
        {
            string name = players[i].NickName;
            bool isReady = (bool)players[i].CustomProperties["isReady"];
            bool isLocal = players[i].IsLocal;
            playerUiData[i].UpdateUI(name,isReady,isLocal);
        }
    }

    public void ActivateGameStartingScreen()
    {
        gameStartingScreen.SetActive(true);
    }

}



[Serializable]
public class PlayerUIData
{
    string name;
    bool isReady;
    bool isLocal;

    [SerializeField] private TMP_Text nameTxt;
    [SerializeField] private Image ReadyImg;
    [SerializeField] private TMP_Text ReadyTxt;
    [SerializeField] private Button readyBtn;

    PlayerUIData()
    {
        name = "Finding";
        isReady = false;
        isLocal = false;
    }

    public void UpdateUI(string _name, bool _isready, bool _isLocal)
    {
        name = _name;
        isReady = _isready;
        isLocal = _isLocal;

        UpdateUI();
    }

    public void UpdateUI()
    {
        nameTxt.text = name;

        ReadyImg.color = isReady ? Color.green : Color.red;
        ReadyTxt.text = isReady ? "READY" : "NOT  READY";

        readyBtn.interactable = isLocal;
    }
}
