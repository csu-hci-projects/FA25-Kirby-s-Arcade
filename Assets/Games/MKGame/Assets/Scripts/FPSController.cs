using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class FPSController : MonoBehaviour
{
    [Header("Move")]
    public float moveSpeed = 5f;

    [Header("Look")]
    [Tooltip("Degrees per pixel of mouse movement. Start small.")]
    public float sensitivity = 0.08f; // try 0.05–0.12 depending on your DPI
    public float pitchMin = -80f;
    public float pitchMax = 80f;
    public float lookSmoothing = 0.02f; // 0 = raw, 0.02–0.05 = light smoothing

    public Transform cameraPivot; // assign your CameraPivot in Inspector

    private CharacterController cc;
    private float yaw;   // left/right around Y (world up)
    private float pitch; // up/down around X (cameraPivot local)

    // for smoothing
    private Vector2 smoothedDelta;
    private Vector2 smoothedDeltaVel;

    // gravity
    private Vector3 velocity;

    void Awake()
    {
        cc = GetComponent<CharacterController>();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        // initialize yaw/pitch from current transforms (in case you rotated in editor)
        yaw   = transform.eulerAngles.y;
        pitch = (cameraPivot ? cameraPivot.localEulerAngles.x : 0f);

        // normalize pitch to [-180,180] then clamp
        if (pitch > 180f) pitch -= 360f;
        pitch = Mathf.Clamp(pitch, pitchMin, pitchMax);

        // hard set rotations cleanly
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (cameraPivot) cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void Update()
    {
        LookUpdate();
        MoveUpdate();
    }

    private void LookUpdate()
    {
        if (Mouse.current == null) return;

        // mouse delta is pixels since last frame (already frame-based)
        Vector2 rawDelta = Mouse.current.delta.ReadValue();

        // optional smoothing (prevents “wonky/jittery” feel on some setups)
        if (lookSmoothing > 0f)
        {
            smoothedDelta = Vector2.SmoothDamp(
                smoothedDelta, rawDelta, ref smoothedDeltaVel, lookSmoothing);
        }
        else
        {
            smoothedDelta = rawDelta;
        }

        // apply sensitivity (degrees per pixel)
        float dx = smoothedDelta.x * sensitivity;
        float dy = smoothedDelta.y * sensitivity;

        // accumulate yaw/pitch
        yaw   += dx;          // left/right (don’t clamp)
        pitch -= dy;          // invert for natural feel
        pitch  = Mathf.Clamp(pitch, pitchMin, pitchMax);

        // apply rotations with ZERO roll
        transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        if (cameraPivot) cameraPivot.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    private void MoveUpdate()
    {
        Vector2 input = Vector2.zero;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed) input.x -= 1f;
            if (Keyboard.current.dKey.isPressed) input.x += 1f;
            if (Keyboard.current.wKey.isPressed) input.y += 1f;
            if (Keyboard.current.sKey.isPressed) input.y -= 1f;
        }

        Vector3 planar = (transform.right * input.x + transform.forward * input.y).normalized * moveSpeed;

        // basic gravity
        if (cc.isGrounded) velocity.y = -1f;
        else velocity.y += Physics.gravity.y * Time.deltaTime;

        cc.Move((planar + velocity) * Time.deltaTime);
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        // relock cursor when you click back into Game window
        if (hasFocus)
        {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}