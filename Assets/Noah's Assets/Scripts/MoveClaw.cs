// using UnityEngine;
// using UnityEngine.InputSystem;

// public class MoveClaw : MonoBehaviour
// {
//     public float speed = 5f;
//     public float rotationSpeed = 10f;
    
//     private Animator animator;

//     void Start()
//     {
//         animator = GetComponent<Animator>();
//     }

//     void FixedUpdate()
//     {
//         Vector2 moveInput = Vector2.zero;

//         if (Keyboard.current != null)
//         {
//             moveInput.x = Keyboard.current.dKey.ReadValue() - Keyboard.current.aKey.ReadValue();
//             moveInput.y = Keyboard.current.wKey.ReadValue() - Keyboard.current.sKey.ReadValue();
//         } else {
//             animator.SetBool("KirbyWalk", false);
//         }

//         float currentSpeed = moveInput.magnitude;

//         // Move the player
//         if (currentSpeed > 0.1f)
//         {
//             Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y).normalized * speed * Time.deltaTime;
//             transform.Translate(movement, Space.World);
//         }
//     }
// }
using UnityEngine;
using UnityEngine.InputSystem;

public class MoveClaw : MonoBehaviour
{
    public float speed = 5f;
    public float rotationSpeed = 10f;
    
    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        // animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector2 moveInput = Vector2.zero;

        if (Keyboard.current != null)
        {
            moveInput.x = Keyboard.current.dKey.ReadValue() - Keyboard.current.aKey.ReadValue();
            moveInput.y = Keyboard.current.wKey.ReadValue() - Keyboard.current.sKey.ReadValue();
        } 
        // else 
        // {
        //     animator.SetBool("KirbyWalk", false);
        // }

        float currentSpeed = moveInput.magnitude;

        // Move the player using physics
        if (currentSpeed > 0.1f)
        {
            Vector3 movement = new Vector3(moveInput.x, 0, moveInput.y).normalized * speed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
            
            // animator.SetBool("KirbyWalk", true);
        }
        // else
        // {
        //     animator.SetBool("KirbyWalk", false);
        // }
    }
}