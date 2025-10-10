using UnityEngine;

[DefaultExecutionOrder(100)] 
public class PlayerAnimation : MonoBehaviour
{
    [SerializeField] Transform cameraTransform;   
    [SerializeField] float flipDeadzone = 0.05f; 

    Animator am;
    SpriteRenderer sr;
    string currentAnimation = "";

    void Awake()
    {
        am = GetComponent<Animator>();
        sr = GetComponent<SpriteRenderer>();
        if (!cameraTransform) cameraTransform = Camera.main ? Camera.main.transform : null;
    }

    void Update()
    {
        SpriteDirectionChecker();
    }

    void SpriteDirectionChecker()
    {
        Vector3 hv = Player.Instance.motion;
        hv.y = 0f;

        if (hv.sqrMagnitude < flipDeadzone * flipDeadzone) return; 

        Vector3 camRight = cameraTransform ? cameraTransform.right : Vector3.right;
        camRight.y = 0f; camRight.Normalize();
        hv.Normalize();

        float side = Vector3.Dot(hv, camRight);
        sr.flipX = side < 0f;
    }

    public void ChangeAnimation(string animation)
    {
        if (string.IsNullOrEmpty(animation) || currentAnimation == animation) return;
        am.Play(animation);           
        currentAnimation = animation; 
    }
}
