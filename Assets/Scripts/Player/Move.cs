﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move : MonoBehaviour
{
    [Header("Camera(s)")]
    public Camera PlayerCamera;
    [Header("Player Movement")]
    [Tooltip("To adjust player speed")]
    public float MovementSpeed;
    [Tooltip("To adjust player sprint speed")]
    public float SprintMod;
    [Tooltip("To adjust player sprint speed")]
    public float CrouchMod;
    [Tooltip("To adjust how fast player reaches sprint")]
    public float GetToSpeed;
    [Tooltip("To adjust player jump")]
    public float JumpForce;
    [Tooltip("To adjust player fov when sprinting")]
    private readonly float SprintFOVMod = 1.25f;

    private Rigidbody Rig;
    private float AdjustSpeed;
    private float BaseFOV;
    private Jump Jumper;

    void Awake()
    {
        AdjustSpeed = MovementSpeed;
        Rig = this.GetComponent<Rigidbody>();
        BaseFOV = PlayerCamera.fieldOfView;
        Jumper = this.GetComponent<Jump>();
    }

    //Fixed update to manipulate physics
    void FixedUpdate()
    {
        //variables 
        float hMove = Input.GetAxisRaw("Horizontal");
        float vMove = Input.GetAxisRaw("Vertical");
        bool sprint = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
        bool crouch = Input.GetKeyDown(KeyCode.LeftControl);
        bool isGrounded = Jumper.isGrounded;

        //basic movement
        Vector3 direction = new Vector3(hMove, 0, vMove);
        direction.Normalize();

        if (sprint && isGrounded)
        {
            Sprint(sprint);
            AdjustSpeed = Mathf.Lerp(AdjustSpeed, MovementSpeed * SprintMod, Time.fixedDeltaTime * GetToSpeed);
        }
        else if (crouch)
        {
            AdjustSpeed = Mathf.Lerp(AdjustSpeed, MovementSpeed * CrouchMod, Time.fixedDeltaTime * GetToSpeed);
        }
        else
        {
            AdjustSpeed = Mathf.Lerp(AdjustSpeed, MovementSpeed, Time.fixedDeltaTime * GetToSpeed);
        }

        //apply velocity to player
        Vector3 targetVelocity = transform.TransformDirection(direction) * AdjustSpeed * Time.fixedDeltaTime;
        targetVelocity.y = Rig.velocity.y;
        Rig.velocity = Vector3.Lerp(Rig.velocity, targetVelocity, Time.fixedDeltaTime * 10);
    }

    private void Sprint(bool sprinting)
    {
        if (sprinting)
        {
            PlayerCamera.fieldOfView = Mathf.Lerp(PlayerCamera.fieldOfView, BaseFOV * SprintFOVMod, Time.deltaTime * GetToSpeed);
        }
        else
        {
            PlayerCamera.fieldOfView = Mathf.Lerp(PlayerCamera.fieldOfView, BaseFOV, Time.deltaTime * GetToSpeed);

        }
    }
}