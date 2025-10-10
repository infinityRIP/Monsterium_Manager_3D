using Game.Stats;
using NaughtyAttributes;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : Singleton<Player>
{
    #region Stats Setting 

    [Serializable]
    public struct BaseEntry { public PlayerStatId id; public float baseValue; }
    public List<BaseEntry> bases = new()
    {
        new BaseEntry{ id=PlayerStatId.MaxHp,      baseValue=100 },
        new BaseEntry{ id=PlayerStatId.Attack,     baseValue=5  },
        new BaseEntry{ id=PlayerStatId.Defense,    baseValue=1   },
        new BaseEntry{ id=PlayerStatId.Persuasion, baseValue=1  },
    };

    public float CurrentHp { get; private set; }

    #endregion

    #region Movement Setting
    [BoxGroup("Movement Setting"), SerializeField] private Transform cameraTransform;
    [BoxGroup("Movement Setting"), SerializeField] public float moveSpeed = 5f;
    [BoxGroup("Movement Setting"), SerializeField] public LayerMask terrainLayer;
    [BoxGroup("Movement Setting"), SerializeField] public Vector3 motion;
    [BoxGroup("Movement Setting"), SerializeField] private bool shouldFaceMoveDirection = false;
    [BoxGroup("Movement Setting"), SerializeField] private float _gravity = -9.81f;
    [BoxGroup("Movement Setting"), SerializeField] private float gravityMultiplier = 3.0f;
    [BoxGroup("Movement Setting"), SerializeField] private float _velocity;
    [BoxGroup("Movement Setting"), SerializeField] private Vector3 horizontalDirection;      // เฉพาะ xz
    [BoxGroup("Movement Setting"), SerializeField] public float verticalVelocity;          // ความเร็วแกน y
    #endregion

    #region Component
    JumpController jc;
    Rigidbody rb;
    SpriteRenderer sr;
    PlayerAnimation Pa;
    Animator am;
    CharacterController controller;
    #endregion

    #region Unity Methods
    void Awake()
    {
        jc = GetComponent<JumpController>();
        sr = GetComponent<SpriteRenderer>();
        Pa = GetComponent<PlayerAnimation>();
        am = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
    }
        
    void Update()
    {
        am.SetFloat("Speed", new Vector2(motion.x, motion.z).magnitude);
        ApplyGravity();
        InputManagement();
        Run();
        Move();

    }
    #endregion

    #region Movement Methods
    
    void ApplyGravity()
    {
        if (controller.isGrounded && verticalVelocity < 0.0f)
        {
            verticalVelocity = -1.0f;
        }else
        {
            verticalVelocity += _gravity * gravityMultiplier * Time.deltaTime;
        }
            
    }
    void InputManagement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0f;
        right.y = 0f;

        forward.Normalize();
        right.Normalize();

        horizontalDirection = forward * moveZ + right * moveX;
  
    }
    void Move()
    {
        if (horizontalDirection.sqrMagnitude > 1f) horizontalDirection.Normalize();
        motion = (horizontalDirection * moveSpeed) + Vector3.up * verticalVelocity;
        controller.Move(motion * Time.deltaTime);


        if (shouldFaceMoveDirection && horizontalDirection.sqrMagnitude > 0.001f)
        {
            Quaternion toRotation = Quaternion.LookRotation(horizontalDirection, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, 10f * Time.deltaTime);
        }
    }
    void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            moveSpeed = 8f;
            am.SetBool("isRunning", true);
        }
        else
        {
            moveSpeed = 5f;
            am.SetBool("isRunning", false);
        }
    }
    #endregion
}
