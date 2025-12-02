using UnityEngine;

public class FirstPersonMove : MonoBehaviour
{
    public float speed = 5f;
    public float mouseSensitivity = 2f;
    //public Transform playerCamera;

    private float xRotation = 0f;

    void Start()
    {
        // Lock cursor to the center of the screen
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // --- Mouse look ---
        // float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity;
        // float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity;

        //xRotation -= mouseY;
        // xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //playerCamera.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        //transform.Rotate(Vector3.up * mouseX);

       
        float moveX = Input.GetAxis("Horizontal"); 
        float moveZ = Input.GetAxis("Vertical");   

        Vector3 move = transform.right * moveX + transform.forward * moveZ;
        transform.position += move * speed * Time.deltaTime;
    }
}
