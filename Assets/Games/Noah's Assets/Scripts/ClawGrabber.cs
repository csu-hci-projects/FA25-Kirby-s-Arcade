
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
        // Smoothly move to target angle
        RotateClawEuler(eastClaw, new Vector3(0, -90, -targetAngle));
        RotateClawEuler(westClaw, new Vector3(0, 90, -targetAngle));
        RotateClawEuler(southClaw, new Vector3(0, 180, -targetAngle));
        RotateClawEuler(northClaw, new Vector3(0, 0, -targetAngle));
    }
    
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