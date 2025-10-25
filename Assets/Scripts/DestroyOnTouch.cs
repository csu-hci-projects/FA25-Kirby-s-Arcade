using UnityEngine;

public class DestroyOnTouch : MonoBehaviour
{
    void OnTriggerEnter(Collider other)
    {
        
        if (other.CompareTag("Player"))
        {
            Destroy(gameObject); 
        }
    }
}
