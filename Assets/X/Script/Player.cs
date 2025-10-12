using Game.Stats;
using NaughtyAttributes;
using UnityEngine;

public class Player : PlayerSingleton<Player>
{
    #region Stats Setting 

    public Stat MaxHealth;
    public Stat Attack;
    public Stat Defense;
    public Stat Persuasion;

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
        UpdateAnimDirection();
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
    void UpdateAnimDirection()
    {
        // ความเร็วจริงจาก CC (ปลอดภัยกว่าอ่านจากตัวแปร)
        Vector3 vel = controller.velocity;
        am.SetFloat("Speed", new Vector2(vel.x, vel.z).magnitude);

        // ถ้าไม่ขยับ ไม่ต้องจัดหมวด
        if (vel.sqrMagnitude < 0.0001f)
        {
            am.SetBool("Side", false);
            am.SetBool("Up", false);
            am.SetBool("Down", false);
            return;
        }

        // สร้างแกนตามกล้อง (บนระนาบ XZ)
        Vector3 camF = cameraTransform.forward; camF.y = 0; camF.Normalize();
        Vector3 camR = cameraTransform.right; camR.y = 0; camR.Normalize();

        // เวคเตอร์การเคลื่อนที่แนวนอน (normalize เพื่อเทียบมุม)
        Vector3 hv = vel; hv.y = 0;
        if (hv.sqrMagnitude > 0.0001f) hv.Normalize();

        // โปรเจกต์ดูว่าไปทางไหนมากกว่า
        float dF = Vector3.Dot(hv, camF); // + ขึ้น, - ลง
        float dR = Vector3.Dot(hv, camR); // + ขวา, - ซ้าย

        float absF = Mathf.Abs(dF);
        float absR = Mathf.Abs(dR);
        const float EPS = 0.15f; // deadzone เพื่อกันสั่น

        bool isSide = absR > absF && absR > EPS;
        bool isUp = absF >= absR && dF > EPS;
        bool isDown = absF >= absR && dF < -EPS;

        am.SetBool("Side", isSide);
        am.SetBool("Up", isUp);
        am.SetBool("Down", isDown);

        // ถ้าใช้ SpriteRenderer แล้วอยากหันซ้าย/ขวา
        if (sr != null && isSide) sr.flipX = (dR < 0f); // true = หันซ้าย
    }

    #endregion

    #region Stats Mothods

    public Player(float baseValue)
    {
        
    }





    #endregion
}
