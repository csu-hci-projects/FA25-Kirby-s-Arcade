using UnityEngine;

public class RandomRoamer : MonoBehaviour
{
    public float moveSpeed = 2f;
    public float rotationSpeed = 2f;
    public float noiseScale = 0.5f;
    
    private Animator animator;
    private float noiseOffsetX;
    private float noiseOffsetZ;

    void Start()
    {
        animator = GetComponent<Animator>();
        
        // Random starting points in the noise map for variation
        noiseOffsetX = Random.Range(0f, 1000f);
        noiseOffsetZ = Random.Range(0f, 1000f);
        
        // Set animation immediately
        if (animator != null)
        {
            animator.SetBool("Moving", true);
        }
    }

    void Update()
    {
        // Get smooth random values using Perlin noise
        float noiseX = Mathf.PerlinNoise(Time.time * noiseScale + noiseOffsetX, 0) * 2 - 1;
        float noiseZ = Mathf.PerlinNoise(0, Time.time * noiseScale + noiseOffsetZ) * 2 - 1;
        
        Vector3 direction = new Vector3(noiseX, 0, noiseZ).normalized;
        
        // Rotate towards the direction
        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }
        
        // Move forward
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }
}