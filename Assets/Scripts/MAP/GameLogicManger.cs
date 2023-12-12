using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using System;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameLogicManger : MonoBehaviour
{
    [SerializeField] private int startingLives = 3;
    [SerializeField] private TMP_Text countdownTxt;

    private PhotonView view;
    private Player localPlayer;
    private int livesLeft;
    private int enemyLivesLeft;
    private int round = 0;

    public static UnityAction RoundStart;
    public static UnityAction RoundEnd;
    public static UnityAction<int, int, int> RoundStartUIUpdate;
    public static UnityAction GameWon;
    public static UnityAction Gamelost;

    private void Awake()
    {
        view = GetComponent<PhotonView>();
        localPlayer = PhotonNetwork.LocalPlayer;
        livesLeft = startingLives;
        enemyLivesLeft = startingLives;
        round = 0;

        //Cursor.lockState = CursorLockMode.None;
        //#if UNITY_EDITOR
        //        Cursor.lockState = CursorLockMode.Locked;
        //#endif

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void Start()
    {
        if (localPlayer.IsMasterClient)
            StartRound();
    }


    #region RoundLogic

    private void StartRound()
    {
        view.RPC(nameof(StartRoundRPC), RpcTarget.AllBufferedViaServer);
    }

    [PunRPC]
    private void StartRoundRPC()
    {
        round++;
        RoundStartUIUpdate?.Invoke(livesLeft, enemyLivesLeft, round);
        StartCoroutine(Countdown(3f));
    }

    private IEnumerator Countdown(float time)
    {
        float timer = time;
        while (timer > 0)
        {
            timer -= Time.deltaTime;
            countdownTxt.text = (int)timer + "";
            yield return null;
        }
        countdownTxt.text = "";
        RoundStart?.Invoke();
    }


    private void EndRound()
    {
        view.RPC(nameof(UpdateScoreForEnemyRPC), RpcTarget.Others, livesLeft);
        view.RPC(nameof(EndRoundRpc), RpcTarget.AllViaServer);
     
        StartRound();
    }

    [PunRPC]
    private void EndRoundRpc()
    {
        RoundEnd.Invoke();
    }

    #endregion



    #region PlayerScoreLogic
    private void LifeLost()
    {
        livesLeft--;

        if (livesLeft <= 0)
        {
            GameOver();
        }
        EndRound();
    }

    [PunRPC]
    private void UpdateScoreForEnemyRPC(int livesLeft)
    {
        enemyLivesLeft = livesLeft;
    }

    private void GameOver()
    {
        Gamelost?.Invoke();
        view.RPC(nameof(GameWonRPC), RpcTarget.Others);
    }

    [PunRPC]
    private void GameWonRPC()
    {
        GameWon?.Invoke();
    }

    #endregion
    
    private void OnEnable()
    {
        PlayerHealth.PlayerDied += LifeLost;
    }
    private void OnDisable()
    {
        PlayerHealth.PlayerDied -= LifeLost;
    }

}
