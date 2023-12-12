using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.Events;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float totalHealth = 100;
    [SerializeField] private Image floatingHPBar;

    public static UnityAction<float, float> HealthBarUpdate;
    public static UnityAction PlayerDied;

    private float currentHealth;
    private bool isGameRunning = true;
    private PhotonView view;

    private void Awake()
    {
        currentHealth = totalHealth;
        view = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (view.IsMine)
        {
            floatingHPBar.gameObject.SetActive(false);
        }
    }

    public void TookDamage(float damage, int actrNr)
    {
        if (!isGameRunning) return;

        view.RPC(nameof(TookDamageRPC), RpcTarget.AllViaServer, damage, actrNr);
    }

    [PunRPC]
    private void TookDamageRPC(float damage, int actrNr)
    {
        if (view.OwnerActorNr == actrNr )
        {
            currentHealth -= damage;
        }

        if (view.IsMine)
        {
            HealthBarUpdate?.Invoke(currentHealth, totalHealth);
        }

        floatingHPBar.fillAmount = currentHealth / totalHealth;

        if (currentHealth <= 0 && view.IsMine)
        {
            Die();
        }
    }

    private void Die()
    {
        PlayerDied?.Invoke();
    }

    private void RoundEnd()
    {
        isGameRunning = false;
        currentHealth = totalHealth;
        HealthBarUpdate?.Invoke(currentHealth, totalHealth);
    }
    private void RoundStart()
    {
        isGameRunning = true;
    }

    private void OnEnable()
    {
        GameLogicManger.RoundStart += RoundStart;
        GameLogicManger.RoundEnd += RoundEnd;
    }
    private void OnDisable()
    {
        GameLogicManger.RoundStart -= RoundStart;
        GameLogicManger.RoundEnd -= RoundEnd;
    }
}
