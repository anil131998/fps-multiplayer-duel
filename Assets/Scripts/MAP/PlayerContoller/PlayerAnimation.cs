using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAnimation : MonoBehaviour
{
    private int currentState;
    private float lockedTill;
    private Vector2 moveInput;
    private bool runInput;
    private bool jumpInput;
    private bool isGrounded;

    private Animator anim;
    private PhotonView view;

    private readonly int Idle = Animator.StringToHash("Idle");
    private readonly int Walk = Animator.StringToHash("Walk");
    private readonly int Run = Animator.StringToHash("Run");
    private readonly int LeftStrafe = Animator.StringToHash("LeftStrafe");
    private readonly int RightStrafe = Animator.StringToHash("RightStrafe");
    private readonly int Jump = Animator.StringToHash("Jump");

    private void Awake()
    {
        anim = GetComponent<Animator>();
        view = GetComponent<PhotonView>();
    }

    private void FixedUpdate()
    {
        if (view.IsMine)
        {
            var state = GetState();

            if (state == currentState) return;

            view.RPC(nameof(RPCAnimatePlayer), RpcTarget.All, state);
        }
    }

    [PunRPC]
    private void RPCAnimatePlayer(int state)
    {
        anim.CrossFade(state, 0.1f, 0);
        currentState = state;
    }

    private int GetState()
    {
        if (Time.time < lockedTill) return currentState;

        // Set Animations

        if(jumpInput)
        {
            return Jump;
        }

        if(isGrounded)
        {
            if (moveInput.magnitude >= 0.1f)
            {
                if (moveInput.x < -0.5f && moveInput.y == 0)
                    return LeftStrafe;
                if (moveInput.x > 0.5f && moveInput.y == 0)
                    return RightStrafe;
                if (runInput)
                    return Run;
                
                return Walk;
            }
            else
            {
                return Idle;
            }
        }
        

        //if (_attacked) return LockState(Attack, _attackAnimTime);

        return currentState;

        //int LockState(int s, float t)
        //{
        //    lockedTill = Time.time + t;
        //    return s;
        //}
    }

    public void SetAnimatorState(Vector2 _moveInput, bool _runInput, bool _jumpInput, bool _isGrounded)
    {
        moveInput = _moveInput;
        runInput = _runInput;
        jumpInput = _jumpInput;
        isGrounded = _isGrounded;
    }

}
