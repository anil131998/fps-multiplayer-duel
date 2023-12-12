using System.Collections;
using UnityEngine;
using Photon.Pun;
using System;

public class GunController : MonoBehaviour
{
    [SerializeField] private Transform shootPoint;
    [SerializeField] private LayerMask hitLayerMask;
    [SerializeField] private float fireRange = 500f;
    [SerializeField] private float fireRate = 4f;
    [SerializeField] private float damage = 10f;

    [SerializeField] private GameObject muzzleFlashFX;
    [SerializeField] private GameObject hitFX;
    [SerializeField] private GameObject playerHitFX;
    [SerializeField] private GameObject bulletTrace;
    [SerializeField] private AudioSource fireSound;
    [SerializeField] private Transform gunTransform;

    private Transform gameCamera;
    private PhotonView view;
    private MobileInput mobileInput;
    private bool shootInput;
    private bool aimInput;
    private bool fireReady = true;
    private bool isGameRunning = true;

    private void Start()
    {
        view = GetComponent<PhotonView>();
        gameCamera = Camera.main.transform;
        mobileInput = MobileInput.Instance;
    }

    private void Update()
    {
        if (view.IsMine && isGameRunning)
        {
            ReadInput();
        }
    }

    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            if (shootInput && fireReady)
            {
                ShootBullet();
                StartCoroutine(FireCoolDown());
            }
        }
    }

    private void ReadInput()
    {

//        shootInput = mobileInput.fireInput;

//#if UNITY_EDITOR
//        shootInput = Input.GetKey(KeyCode.Mouse0);
//        aimInput = Input.GetKey(KeyCode.Mouse1);
//#endif

        shootInput = Input.GetKey(KeyCode.Mouse0);
        aimInput = Input.GetKey(KeyCode.Mouse1);
    }

    //game object instantiation
    //private void ShootBullet()
    //{
    //    RaycastHit Hit;
    //    if (Physics.Raycast(gameCamera.position, gameCamera.forward, out Hit, fireRange, hitLayerMask))
    //    {
    //        GameObject muzzleFlashObj;
    //        muzzleFlashObj = (GameObject)Instantiate(muzzleFlashFX, shootPoint.position, Quaternion.identity, shootPoint);
    //        Destroy(muzzleFlashObj.gameObject, 1f);

    //        GameObject bulletTraceObj;
    //        bulletTraceObj = (GameObject)Instantiate(bulletTrace, shootPoint.position, Quaternion.identity);
    //        LineRenderer lineRenderer = bulletTraceObj.GetComponent<LineRenderer>();
    //        lineRenderer.SetPosition(0, shootPoint.position);
    //        lineRenderer.SetPosition(1, Hit.point);
    //        Destroy(bulletTraceObj.gameObject, 1f);

    //        GameObject impactFXObj;
    //        impactFXObj = (GameObject) Instantiate(hitFX, Hit.point, Quaternion.identity );
    //        Destroy(impactFXObj.gameObject, 1f);
    //    }
    //}

    private void ShootBullet()
    {
        RaycastHit Hit;
        if (Physics.Raycast(gameCamera.position, gameCamera.forward, out Hit, fireRange, hitLayerMask))
        {
            gunTransform.LookAt(Hit.point);

            GameObject muzzleFlashObj;
            muzzleFlashObj = PhotonNetwork.Instantiate(muzzleFlashFX.name, shootPoint.position, Quaternion.identity);
            muzzleFlashObj.transform.SetParent(shootPoint);
            StartCoroutine(DestroyNetworkObjWithDelay(1f, muzzleFlashObj));

            view.RPC(nameof(DrawBulletTrace), RpcTarget.All, Hit.point);

            if(Hit.transform.gameObject.layer == 3)
            {
                GameObject playerHitFXObj;
                playerHitFXObj = PhotonNetwork.Instantiate(playerHitFX.name, Hit.point, Quaternion.identity);
                StartCoroutine(DestroyNetworkObjWithDelay(1f, playerHitFXObj));
            }
            else
            {
                GameObject impactFXObj;
                impactFXObj = PhotonNetwork.Instantiate(hitFX.name, Hit.point, Quaternion.identity);
                StartCoroutine(DestroyNetworkObjWithDelay(1f, impactFXObj));
            }
            

            fireSound.PlayOneShot(fireSound.clip);

            DoDamage(Hit);
        }
    }

    private void DoDamage(RaycastHit Hit)
    {
        GameObject PlayerObj = Hit.transform.gameObject;

        if (PlayerObj.TryGetComponent(out PlayerHealth playerHealth))
        {
            int actorNr = PlayerObj.GetComponent<PhotonView>().OwnerActorNr;
            playerHealth.TookDamage(damage,actorNr);
        }
    }

    [PunRPC]
    private void DrawBulletTrace(Vector3 hitPos)
    {
        GameObject bulletTraceObj;
        bulletTraceObj = PhotonNetwork.Instantiate(bulletTrace.name, shootPoint.position, Quaternion.identity);
        LineRenderer lineRenderer = bulletTraceObj.GetComponent<LineRenderer>();
        lineRenderer.SetPosition(0, shootPoint.position);
        lineRenderer.SetPosition(1, hitPos);
        StartCoroutine(DestroyNetworkObjWithDelay(1f, bulletTraceObj));
    }

    private IEnumerator DestroyNetworkObjWithDelay(float delay, GameObject obj)
    {
        yield return new WaitForSeconds(delay);
        PhotonNetwork.Destroy(obj);
    }

    private IEnumerator FireCoolDown()
    {
        fireReady = false;
        yield return new WaitForSeconds(1/fireRate);
        fireReady = true;
    }

    private void RoundStart()
    {
        isGameRunning = true;
    }
    private void RoundEnd()
    {
        isGameRunning = false;

        shootInput = false;
        aimInput = false;
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
