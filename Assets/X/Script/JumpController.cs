using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class JumpController : MonoBehaviour
{
    [Header("Movement Stats")]
    public KeyCode jumpKey = KeyCode.Space;
    public float jumpForce = 5f;
    Rigidbody rb;
    Animator am;

    [Header("Ground")]
    [SerializeField] Transform groundCheck;
    [SerializeField] float groundRadius = 0.25f;
    public LayerMask groundLayer;

    [Header("Bool")]
    public bool isGrounded;
    public bool jumpPressed;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        am = GetComponent<Animator>();
    }
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer, QueryTriggerInteraction.Ignore);

        if (Input.GetKeyDown(jumpKey))
            jumpPressed = true;
            CheckJump();
    }

    void FixedUpdate()
    {
        
    }
    void CheckJump()
    {
        if (isGrounded && jumpPressed)
        {
            jumpPressed = false;
            am?.SetTrigger("Jump");
            var v = rb.linearVelocity;
            v.y = 0f;
            rb.linearVelocity = v;
            rb.linearVelocity = new Vector3(rb.linearVelocity.x, jumpForce, rb.linearVelocity.z);
        }
        else
        {
            return;
        }
    }
    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }

}
