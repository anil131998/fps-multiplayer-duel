using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Photon.Pun;

public class UIUpdates : MonoBehaviourPunCallbacks
{
    [SerializeField] private Image healthBarFill;
    [SerializeField] private TMP_Text healthBarTxt;

    [SerializeField] private TMP_Text playerLivesLeftTxt;
    [SerializeField] private TMP_Text EnemyLivesLeftTxt;
    [SerializeField] private TMP_Text currentRoundTxt;

    [SerializeField] private GameObject winnerPanel;
    [SerializeField] private GameObject looserPanel;


    private void Start()
    {
        looserPanel.SetActive(false);
        winnerPanel.SetActive(false);
    }

    private void UpdatePlayerHealth(float currentHealth, float totalHealth)
    {
        healthBarFill.fillAmount = currentHealth / totalHealth;
        healthBarTxt.text = currentHealth + " / " + totalHealth;
    }

    private void UpdateScore(int playerScore, int enemyScore, int currentRound)
    {
        playerLivesLeftTxt.text = playerScore + "";
        EnemyLivesLeftTxt.text = enemyScore + "";
        currentRoundTxt.text = currentRound + "";
    }

    private void GameWon()
    {
        Cursor.lockState = CursorLockMode.Confined;
        winnerPanel.SetActive(true);
    }

    private void GameLost()
    {
        Cursor.lockState = CursorLockMode.Confined;
        looserPanel.SetActive(true);
    }

    public void LeaveGame()
    {
        LeaveRoom();
    }

    private void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        PhotonNetwork.LoadLevel("MENU");
    }

    public override void OnEnable()
    {
        PlayerHealth.HealthBarUpdate += UpdatePlayerHealth;
        GameLogicManger.RoundStartUIUpdate += UpdateScore;
        GameLogicManger.GameWon += GameWon;
        GameLogicManger.Gamelost += GameLost;
        base.OnEnable();
    }

    public override void OnDisable()
    {
        PlayerHealth.HealthBarUpdate -= UpdatePlayerHealth;
        GameLogicManger.RoundStartUIUpdate -= UpdateScore;
        GameLogicManger.GameWon -= GameWon;
        GameLogicManger.Gamelost -= GameLost;
        base.OnDisable();
    }

}
