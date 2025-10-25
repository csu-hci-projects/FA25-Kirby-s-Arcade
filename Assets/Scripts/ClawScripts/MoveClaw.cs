using UnityEngine;
using UnityEngine.InputSystem;

public class MoveClaw : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;
    
    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        Vector2 moveInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            moveInput.x = Keyboard.current.dKey.ReadValue() - Keyboard.current.aKey.ReadValue();
            moveInput.y = Keyboard.current.wKey.ReadValue() - Keyboard.current.sKey.ReadValue();
        } else {
            animator.SetBool("KirbyWalk", false);
        }

        float currentSpeed = moveInput.magnitude;

        // Set animation speed
        if (animator != null) {
            if (currentSpeed > 0.1)
            {
                animator.SetBool("KirbyWalk", true); // Use float parameter instead
            }
            else {
                animator.SetBool("KirbyWalk", false);
            }
        }
        // Move the player
        if (currentSpeed > 0.1f)
        {
            Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y).normalized * speed * Time.deltaTime;
            transform.Translate(movement, Space.World);
        }
    }
}