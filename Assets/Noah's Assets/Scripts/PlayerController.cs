// using UnityEngine;
// using UnityEngine.InputSystem; // Add this

// public class PlayerController : MonoBehaviour
// {
//     public float speed = 5f;

//     void Update()
//     {
//         // New Input System way
//         Vector2 moveInput = new Vector2(
//             Input.GetKey(Key.D) - Input.GetKey(Key.A),
//             Input.GetKey(Key.W) - Input.GetKey(Key.S)
//         );
        
//         // Or use Keyboard.current
//         float horizontal = (Keyboard.current.dKey.isPressed ? 1f : 0f) - 
//                           (Keyboard.current.aKey.isPressed ? 1f : 0f);
//         float vertical = (Keyboard.current.wKey.isPressed ? 1f : 0f) - 
//                         (Keyboard.current.sKey.isPressed ? 1f : 0f);
        
//         Vector3 movement = new Vector3(horizontal, 0, vertical) * speed * Time.deltaTime;
//         transform.Translate(movement);
//     }
// }