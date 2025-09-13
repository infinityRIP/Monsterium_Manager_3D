using NaughtyAttributes;
using UnityEngine;

public class JumpController : MonoBehaviour
{
    [BoxGroup("Movement Stats"), SerializeReference] public KeyCode jumpKey = KeyCode.Space;
    [BoxGroup("Movement Stats"), SerializeReference] public float jumpForce = 5f;

    [Foldout("Ground", true)] public Transform groundCheck;
    [Foldout("Ground", true)] public float groundRadius = 0.25f;
    [Foldout("Ground", true)] public LayerMask groundLayer;

    #region Bool
    [Foldout("Bool", true)] public bool isGrounded;
    [Foldout("Bool", true)] public bool jumpPressed;
    #endregion 

    #region Component
    Rigidbody rb;
    Animator am;
    #endregion

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
