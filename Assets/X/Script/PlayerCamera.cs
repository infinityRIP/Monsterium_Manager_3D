using UnityEditor.Timeline.Actions;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    public Transform target;         // ตัวละคร (Player)
    public Quaternion rotation;
    public Vector3 offset;           // ระยะห่างจากตัวละคร
    public Vector2 turn;
    public float sensitivity = 5f;

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        target = Player.Instance.transform;

        if (target == null)
        {
            Debug.Log("No Target");
            return;
        }

        Vector3 desiredPosition = target.position + offset;
        transform.position = desiredPosition;
        transform.rotation = rotation;
    }
    //void LateUpdate()
    //{
        
    //}
    //private void Update()
    //{
    //    turn.x = Input.GetAxis("Mouse X") * sensitivity;
    //    turn.y = Input.GetAxis("Mouse Y") * sensitivity;
    //    this.gameObject.transform.localRotation = Quaternion.Euler(0, turn.x, 0);
    //    transform.localRotation = Quaternion.Euler(-turn.y, turn.x, 0);


    //}
}
