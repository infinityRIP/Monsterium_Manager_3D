using NaughtyAttributes;
using UnityEngine;

public class JumpController : MonoBehaviour
{
    #region Jump Setting
    [BoxGroup("Movement Stats"), SerializeReference] public KeyCode jumpKey = KeyCode.Space;
    [BoxGroup("Movement Stats"), SerializeReference] public float jumpForce = 5f;

    [Foldout("Ground", true)] public Transform groundCheck;
    [Foldout("Ground", true)] public float groundRadius = 0.25f;
    [Foldout("Ground", true)] public LayerMask groundLayer;
    #endregion

    #region Bool
    [Foldout("Bool", true)] public bool isGrounded;
    [Foldout("Bool", true)] public bool jumpPressed;
    #endregion 

    #region Component
    Rigidbody rb;
    Animator am;
    CharacterController cc;
    #endregion

    #region Unity Methods
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        am = GetComponent<Animator>();
        cc = GetComponent<CharacterController>();
    }
    void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundRadius, groundLayer, QueryTriggerInteraction.Ignore);

        if (Input.GetKeyDown(jumpKey))
            jumpPressed = true;
            CheckJump();
    }
    #endregion

    #region Jump Methods
    void CheckJump()
    {
        if (isGrounded && jumpPressed)
        {
            jumpPressed = false;
            am?.SetTrigger("Jump");
            Player.Instance.verticalVelocity += jumpForce;
        }
        else
        {
            return;
        }
    }
    #endregion

    #region Gizmos
    void OnDrawGizmosSelected()
    {
        if (!groundCheck) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundRadius);
    }
    #endregion

}
