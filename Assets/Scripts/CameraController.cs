using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject player;
    public float sensitivity = 5.0f;
    public float minimumY = -60.0f;
    public float maximumY = 60.0f;

    private Vector3 offset;
    private float rotationY = 0.0f;

    void Start()
    {
        offset = transform.position - player.transform.position;
    }

    void LateUpdate()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity;

        transform.RotateAround(player.transform.position, Vector3.up, mouseX);

        rotationY += mouseY;
        rotationY = Mathf.Clamp(rotationY, minimumY, maximumY);

        transform.rotation = Quaternion.Euler(-rotationY, transform.rotation.eulerAngles.y, 0);

        transform.position = player.transform.position + transform.rotation * offset;
    }
}