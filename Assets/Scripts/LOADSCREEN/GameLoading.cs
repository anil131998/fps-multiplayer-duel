using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class GameLoading : MonoBehaviourPunCallbacks
{

    [SerializeField] private TMP_Text loadingText;

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        loadingText.text = "CONNECTING TO SERVER . . .";
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        loadingText.text = "LOADING GAME .";
    }

    public override void OnJoinedLobby()
    {
        loadingText.text = "LOADING GAME . .";
        StartCoroutine(LoadScreenAnimation());
    }

    private IEnumerator LoadScreenAnimation()
    {
        float fixedLoadTIme = 2f;
        float timer = 0;
        while (timer < fixedLoadTIme)
        {
            //loadingImage.fillAmount = timer / fixedLoadTIme;
            timer += Time.deltaTime;
            yield return null;
        }
        loadingText.text = "LOADING GAME . . .";
        SceneManager.LoadScene("MENU");
    }

}
