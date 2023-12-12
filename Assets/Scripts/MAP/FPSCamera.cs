using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class FPSCamera : MonoBehaviour
{
    private CinemachineVirtualCamera cmVCam;
    private CinemachinePOV cmPOV;
    private MobileInput mobileInput;
    public float mouse_X;
    public float mouse_Y;

    private void Awake()
    {
        cmVCam = GetComponent<CinemachineVirtualCamera>();

        cmPOV = cmVCam.GetCinemachineComponent<CinemachinePOV>();
        
    }

    private void Start()
    {
        mobileInput = MobileInput.Instance;
    }

    private void AttachCameraToPlayer(GameObject player)
    {
        Transform cameraHandle = player.transform.Find("CameraHandle");
        cmVCam.Follow = cameraHandle;
        cmVCam.LookAt = cameraHandle;
    }

    private void Update()
    {
//        cmPOV.m_HorizontalAxis.m_MaxSpeed = 0f;
//        cmPOV.m_VerticalAxis.m_MaxSpeed = 0f;

//#if UNITY_EDITOR
//        cmPOV.m_HorizontalAxis.m_MaxSpeed = 300f;
//        cmPOV.m_VerticalAxis.m_MaxSpeed = 300f;
//#endif

        cmPOV.m_HorizontalAxis.m_MaxSpeed = 300f;
        cmPOV.m_VerticalAxis.m_MaxSpeed = 300f;

        mouse_X = mobileInput.mouse_X;
        mouse_Y = mobileInput.mouse_Y;
        cmPOV.m_HorizontalAxis.Value += mouse_X;
        cmPOV.m_VerticalAxis.Value += mouse_Y * -1;
    }

    private void OnEnable()
    {
        SpawnPlayer.PlayerSpawned += AttachCameraToPlayer;
    }

    private void OnDisable()
    {
        SpawnPlayer.PlayerSpawned -= AttachCameraToPlayer;
    }

}
