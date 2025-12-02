// using UnityEngine;
// using UnityEngine.InputSystem;

// public class ClawGrabber : MonoBehaviour
// {
//     public Transform eastClaw;
//     public Transform westClaw;
//     public Transform southClaw;
//     public Transform northClaw;
    
//     public float openAngle = 45f;
//     public float closeAngle = 0f;
//     public float rotationSpeed = 2f;
    
//     private bool isOpen = false;
//     private bool keyPressed = false;
//     float targetAngle;
//     void Update()
//     {
//         if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
//         {
//             keyPressed = !keyPressed;
//             isOpen = !isOpen;

//         }
//             if (isOpen == false) { 
//                 targetAngle = 0;
//             } else if (isOpen == true) {
//                 targetAngle = 45f;
//             }
//             // Try rotating each on Y-axis (up/down) - adjust if needed
//             // Positive angle for some, negative for others to go inward
//             RotateClawEuler(eastClaw, new Vector3(0, -90, -targetAngle));
//             RotateClawEuler(westClaw, new Vector3(0, 90, -targetAngle));  // Opposite
//             RotateClawEuler(southClaw, new Vector3(0, 180, -targetAngle));
//             RotateClawEuler(northClaw, new Vector3(0, 0, -targetAngle)); // Opposite   
//             Debug.Log(targetAngle);        

        

//     }
    
//     void RotateClawEuler(Transform claw, Vector3 targetEuler)
//     {
//         if (claw == null) return;
        
//         Quaternion targetRotation = Quaternion.Euler(targetEuler);
//         claw.localRotation = Quaternion.Lerp(claw.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
//     }
// }
using UnityEngine;

public class ClawGrabber : MonoBehaviour
{
    public Transform eastClaw;
    public Transform westClaw;
    public Transform southClaw;
    public Transform northClaw;
    
    public float openAngle = 45f;
    public float closeAngle = 0f;
    public float rotationSpeed = 5f;
    
    private float targetAngle = 0f;
    
    void Update()
    {
        // Smoothly interpolate to target angle
        RotateClawEuler(eastClaw, new Vector3(0, -90, -targetAngle));
        RotateClawEuler(westClaw, new Vector3(0, 90, -targetAngle));
        RotateClawEuler(southClaw, new Vector3(0, 180, -targetAngle));
        RotateClawEuler(northClaw, new Vector3(0, 0, -targetAngle));
    }
    
    // PUBLIC METHOD - This is what ClawController calls
    public void SetClawOpen(bool open)
    {
        targetAngle = open ? openAngle : closeAngle;
    }
    
    void RotateClawEuler(Transform claw, Vector3 targetEuler)
    {
        if (claw == null) return;
        
        Quaternion targetRotation = Quaternion.Euler(targetEuler);
        claw.localRotation = Quaternion.Lerp(claw.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
    }
}