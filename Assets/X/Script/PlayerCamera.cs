using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;         // ตัวละคร (Player)
    public Quaternion rotation;
    public Vector3 offset;           // ระยะห่างจากตัวละคร

    private void Start()
    {
        target = Player.Instance.transform;
    }
    void LateUpdate()
    {
        if (target == null)
        {
            Debug.Log("No Target");
            return;
        }
        
        Vector3 desiredPosition = target.position + offset;
        transform.position = desiredPosition;
        transform.rotation = rotation;
    }
}
