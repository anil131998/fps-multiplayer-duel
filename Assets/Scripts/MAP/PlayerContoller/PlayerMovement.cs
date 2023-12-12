using System;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 300f;
    [SerializeField] private float runSpeedMultiplier = 2f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private Transform cameraHandle;
    [SerializeField] private float shoulerSwitchSpeed = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundCheckDistance = 0.1f;
    [SerializeField] private float jumpForce;

    private float horizontalInput;
    private float verticalInput;
    private bool runInput;
    private bool jumpInput;
    private bool isGameRunning;

    private Transform gameCamera;
    private Vector3 moveDir;
    private float turnSmoothVelocity;
    private float targetAngle;
    private Rigidbody rb;
    private PlayerAnimation anim;
    private PhotonView view;
    private MobileInput mobileInput;


    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponent<PlayerAnimation>();
        view = GetComponent<PhotonView>();
        gameCamera = Camera.main.transform;
        mobileInput = MobileInput.Instance;
        
        isGameRunning = true;
    }

    private void Update()
    {
        if (view.IsMine)
        {
            ReadInputs();
        }
    }

    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            //SwitchCameraShoulder();
            RotatePlayer();
            MovePlayer();
            AnimatePlayer();
        }
    }

    private void ReadInputs()
    {

        //        horizontalInput = mobileInput.horizontalInput;
        //        verticalInput = mobileInput.verticalInput;
        //        runInput = mobileInput.runInput;
        //        jumpInput = mobileInput.jumpInput;

        //#if UNITY_EDITOR
        //        horizontalInput = Input.GetAxis("Horizontal");
        //        verticalInput = Input.GetAxis("Vertical");
        //        runInput = Input.GetKey(KeyCode.LeftShift);
        //        jumpInput = Input.GetKey(KeyCode.Space);
        //#endif

        if (isGameRunning)
        {
            horizontalInput = Input.GetAxis("Horizontal");
            verticalInput = Input.GetAxis("Vertical");
            runInput = Input.GetKey(KeyCode.LeftShift);
            jumpInput = Input.GetKey(KeyCode.Space);
        }
        else
        {
            horizontalInput = 0f;
            verticalInput = 0f;
            runInput = false;
            jumpInput = false;
        }

        moveDir = new Vector3(horizontalInput, 0f, verticalInput).normalized;

        if (moveDir.magnitude >= 0.1f)
        {
            targetAngle = Mathf.Atan2(moveDir.x, moveDir.z) * Mathf.Rad2Deg + gameCamera.eulerAngles.y;
            moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        }
        
    }

    private void RotatePlayer()
    {
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, gameCamera.eulerAngles.y, ref turnSmoothVelocity, turnSmoothTime);
        rb.rotation = Quaternion.Euler(0f, angle, 0f);
    }

    private void MovePlayer()
    {
        if (isGrounded())
        {
            if (jumpInput)
            {
                Jump();
                return;
            }

            if (moveDir.magnitude >= 0.1f)
            {
                float finalMS;

                if (runInput)
                    finalMS = moveSpeed * runSpeedMultiplier;
                else
                    finalMS = moveSpeed;

                rb.velocity = moveDir.normalized * finalMS * Time.fixedDeltaTime;
            }
            else
            {
                rb.velocity = Physics.gravity * Time.deltaTime;
            }
        }
    }

    private void Jump()
    {
        rb.AddForce( (Vector3.up) * jumpForce, ForceMode.Impulse);
    }

    private bool isGrounded()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + 0.1f, transform.position.z);
        return Physics.Raycast(pos, Vector3.down, groundCheckDistance + 0.1f, groundLayer);
    }

    private void SwitchCameraShoulder()
    {
        Vector3 pos = cameraHandle.localPosition;
        
        if (horizontalInput < -0.5f)
        {
            cameraHandle.localPosition = new Vector3( Mathf.Clamp(cameraHandle.localPosition.x + Time.deltaTime * shoulerSwitchSpeed, -2f, 1f) , pos.y, pos.z);
        }
        else //if(moveDir.x > 0.5f)
        {
            cameraHandle.localPosition = new Vector3(Mathf.Clamp(cameraHandle.localPosition.x - Time.deltaTime * shoulerSwitchSpeed, -2f, 1f), pos.y, pos.z);
        }
    }

    private void AnimatePlayer()
    {
        Vector2 _movInput = new Vector2(horizontalInput, verticalInput);
        bool _isGrounded = isGrounded();
        anim.SetAnimatorState(_movInput, runInput, jumpInput, _isGrounded);
    }

    private void RoundStart()
    {
        isGameRunning = true;
    }
    private void RoundEnd()
    {
        isGameRunning = false;
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
