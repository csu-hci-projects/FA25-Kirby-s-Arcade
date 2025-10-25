using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerInput))]
public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f;

    private Vector2 moveInput;

    private void OnEnable()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Move"].performed += OnMove;
        playerInput.actions["Move"].canceled += OnMove;
    }

    private void OnDisable()
    {
        var playerInput = GetComponent<PlayerInput>();
        playerInput.actions["Move"].performed -= OnMove;
        playerInput.actions["Move"].canceled -= OnMove;
    }

    private void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    private void Update()
    {
        Vector3 move = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed * Time.deltaTime;
        transform.position += move;

        if (move != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(move);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
    }
}
