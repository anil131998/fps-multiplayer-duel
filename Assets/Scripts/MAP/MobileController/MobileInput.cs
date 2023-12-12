using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class MobileInput : MonoBehaviour
{
    public static MobileInput Instance;

    [SerializeField] private Joystick movementJoystick;
    [SerializeField] private float movementSensitivity = 2;
    [SerializeField] private float mouseSensitivity = 2;
    
    [SerializeField] private ButtonHandler fireButton;
    [SerializeField] private ButtonHandler runButton;
    [SerializeField] private ButtonHandler jumpButton;

    [HideInInspector] public float horizontalInput;
    [HideInInspector] public float verticalInput;
    [HideInInspector] public float mouse_X;
    [HideInInspector] public float mouse_Y;
    [HideInInspector] public bool fireInput;
    [HideInInspector] public bool runInput;
    [HideInInspector] public bool jumpInput;

    //mouse/swipe input
    private Vector2 firstPressPos;
    private Vector2 secondPressPos;
    private Vector2 currentSwipe;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
        }


    }

    

    private void Update()
    {
        horizontalInput = movementJoystick.Horizontal * movementSensitivity;
        verticalInput = movementJoystick.Vertical * movementSensitivity;

        AimMouseInput();
        
        fireInput = fireButton.isPressed;
        runInput = true; // runButton.isPressed;
        jumpInput = jumpButton.isPressed;
    }


    public void AimMouseInput()
    {
        if (Input.touchCount <= 0) return;

        Touch t = Input.GetTouch(0);
        bool foundTouch = false;

        for (int i = 0; i < Input.touchCount; i++)
        {
            t = Input.GetTouch(i);
            if (t.position.x > Screen.width / 2)
            {
                foundTouch = true;
                break;
            }
        }

        if (!foundTouch) return;

        if (t.phase == TouchPhase.Began)
        {
            firstPressPos = new Vector2(t.position.x, t.position.y);
        }
        if (t.phase == TouchPhase.Moved)
        {
            secondPressPos = new Vector2(t.position.x, t.position.y);
            currentSwipe = new Vector2(secondPressPos.x - firstPressPos.x, secondPressPos.y - firstPressPos.y);
            currentSwipe.Normalize();
        }
        if(t.phase == TouchPhase.Ended)
        {
            currentSwipe = Vector2.zero;
        }

        mouse_X = currentSwipe.x * mouseSensitivity;
        mouse_Y = currentSwipe.y * mouseSensitivity;

    }
}
