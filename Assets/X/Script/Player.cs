using NaughtyAttributes;
using UnityEngine;

public class Player : Singleton<Player>
{
    #region Movement Setting
    [BoxGroup("Movement Setting"), SerializeReference] public float moveSpeed = 5f;
    [BoxGroup("Movement Setting"), SerializeReference] public LayerMask terrainLayer;
    [BoxGroup("Movement Setting"), SerializeReference] public Vector3 moveDir;
    #endregion

    #region Component
    JumpController jc;
    Rigidbody rb;
    SpriteRenderer sr;
    PlayerAnimation Pa;
    Animator am;
    #endregion

    #region Unity Methods
    void Awake()
    {
        jc = GetComponent<JumpController>();
        sr = GetComponent<SpriteRenderer>();
        rb = gameObject.GetComponent<Rigidbody>();
        Pa = GetComponent<PlayerAnimation>();
        am = GetComponent<Animator>();
    }

    void Update()
    {
        am.SetFloat("Speed", Mathf.Abs(moveDir.x));
        InputManagement();
        Run();
    }
    private void FixedUpdate()
    {
        Move();
    }
    #endregion

    #region Movement Methods
    void InputManagement()
    {
        float moveX = Input.GetAxisRaw("Horizontal");
        float moveZ = Input.GetAxisRaw("Vertical");
        moveDir = new Vector3(moveX, 0f, moveZ).normalized; ;
    }
    void Move()
    {
        Vector3 velocity = rb.linearVelocity;
        velocity.x = moveDir.x * moveSpeed;
        velocity.z = moveDir.z * moveSpeed;
        rb.linearVelocity = velocity;
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
