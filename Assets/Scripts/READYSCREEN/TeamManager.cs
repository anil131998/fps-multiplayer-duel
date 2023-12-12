using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class TeamManager : MonoBehaviourPunCallbacks
{
    private bool isReady;
    private string playerName;
    private PhotonView view;
    private UIDataManager uiDataManager;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        uiDataManager = GetComponent<UIDataManager>();
        isReady = false;
    }


    private void Start()
    {
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
            view.RPC(nameof(UpdateUIList), RpcTarget.AllBufferedViaServer);
    }
    //non-masterclients load this scene before joining
    public override void OnJoinedRoom()
    {
        view.RPC(nameof(UpdateUIList), RpcTarget.AllBufferedViaServer);
    }




    #region CustomProperties

    private void UpdatePlayerReadyStatus()
    {
        Hashtable hash = PhotonNetwork.LocalPlayer.CustomProperties;
        hash["isReady"] = isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(hash);
    }

    private void UpdatePlayerTeamNr()
    {
        Hashtable hash;
        Player[] playerList = PhotonNetwork.PlayerList;
        for(int i=0; i < playerList.Length; i++)
        {
            hash = playerList[i].CustomProperties;
            if (!hash.ContainsKey("TeamNr"))
                hash.Add("TeamNr", i);
            else
                hash["TeamNr"] = i;
            playerList[i].SetCustomProperties(hash);
        }
    }

    #endregion




    #region ReadyScreenLogic

    public void ReadyClicked()
    {
        isReady = !isReady;
        UpdatePlayerReadyStatus();

        view.RPC(nameof(UpdateUIList), RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    public void UpdateUIList()
    {
        uiDataManager.UpdateUIPlayerList();

        if (PhotonNetwork.LocalPlayer.IsMasterClient && CheckIfPlayersReady())
        {
            view.RPC(nameof(StartGame), RpcTarget.AllViaServer);
        }
    }

    private bool CheckIfPlayersReady()
    {
        if (!(PhotonNetwork.PlayerList.Length == 2))
        {
            return false;
        }

        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!(bool)player.CustomProperties["isReady"])
            {
                return false;
            }
        }

        return true;
    }

    [PunRPC]
    private void StartGame()
    {
        uiDataManager.ActivateGameStartingScreen();

        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            UpdatePlayerTeamNr();
            PhotonNetwork.LoadLevel("MAP2");
        }
    }


    #endregion


}
