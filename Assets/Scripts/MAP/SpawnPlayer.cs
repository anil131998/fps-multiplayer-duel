using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.Events;

public class SpawnPlayer : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private Transform[] spawnlocations;

    public static UnityAction<GameObject> PlayerSpawned;

    GameObject Player;
    private bool playerCreated = false;
    private int TeamNr;

    private void Start()
    {
        TeamNr = (int)PhotonNetwork.LocalPlayer.CustomProperties["TeamNr"];
    }

    private void SpawnOrResetPlayers()
    {
        if (!playerCreated)
        {
            Player = PhotonNetwork.Instantiate(playerPrefab.name, spawnlocations[TeamNr].position, spawnlocations[TeamNr].rotation.normalized);
            PlayerSpawned?.Invoke(Player);
            playerCreated = true;
        }
        else
        {
            Player.transform.SetPositionAndRotation(spawnlocations[TeamNr].position, spawnlocations[TeamNr].rotation.normalized);
        }
    }

    private void OnEnable()
    {
        GameLogicManger.RoundStart += SpawnOrResetPlayers;
    }
    private void OnDisable()
    {
        GameLogicManger.RoundStart -= SpawnOrResetPlayers;
    }
}
