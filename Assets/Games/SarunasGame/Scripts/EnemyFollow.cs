using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyFollow : MonoBehaviour
{
    [Header("Player Reference")]
    public Transform playerTransform;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float stopDistance = 1f;
    public bool rotateTowardsPlayer = true; // Face player while chasing
    public float rotationSpeed = 5f;

    private CharacterController controller;

    void Start()
    {
        // Auto-find player
        if (playerTransform == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
            else Debug.LogError("EnemyFollow: No 'Player' tag found!");
        }

        // **CharacterController = PERFECT WALL COLLISION/SLIDING (no through/bounce/fly!)**
        controller = GetComponent<CharacterController>();
        if (controller == null)
        {
            controller = gameObject.AddComponent<CharacterController>();
            controller.height = 2f; // Adjust to fit your enemy model
            controller.radius = 0.5f;
            controller.skinWidth = 0.08f; // Good collision tolerance
            controller.stepOffset = 0.3f; // For stairs/slopes if needed
            controller.slopeLimit = 45f;
        }
        Debug.Log("Enemy: CharacterController added/configured for bulletproof wall sliding!");
    }

    void Update() // CharacterController.Move works in Update (handles substeps internally)
    {
        if (playerTransform == null || controller == null) return;

        // Horizontal direction to player (ignore height)
        Vector3 direction = playerTransform.position - transform.position;
        direction.y = 0f;
        float distance = direction.magnitude;

        if (distance > stopDistance)
        {
            direction.Normalize();

            // **Move() with collision: Slides perfectly along walls/corners!**
            Vector3 moveDelta = direction * moveSpeed * Time.deltaTime;
            controller.Move(moveDelta);

            // Optional: Rotate to face player
            if (rotateTowardsPlayer)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }
    }

    // Detect player touch via controller collision
    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if (hit.gameObject.CompareTag("Player"))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }
}