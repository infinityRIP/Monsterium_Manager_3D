using UnityEngine;

public class Billboard : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;
    private void LateUpdate()
    {
        _mainCamera = Camera.main;

        Vector3 cameraPos = _mainCamera.transform.position;

        cameraPos.y = transform.position.y;

        transform.LookAt(cameraPos);
        transform.Rotate(0f, 180f, 0f);
    }
}
