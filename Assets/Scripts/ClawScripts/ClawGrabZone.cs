using UnityEngine;
using System.Collections.Generic;

public class ClawGrabZone : MonoBehaviour
{
    private List<GameObject> objectsInZone = new List<GameObject>();
    private List<GameObject> grabbedObjects = new List<GameObject>();
    public Transform clawParent;
    private bool isGrabbing = false; // ADD THIS
    
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            if (!objectsInZone.Contains(other.gameObject))
            {
                objectsInZone.Add(other.gameObject);
                Debug.Log("Object entered grab zone: " + other.gameObject.name);
            }
        }
    }
    
    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Character"))
        {
            objectsInZone.Remove(other.gameObject);
            Debug.Log("Object left grab zone: " + other.gameObject.name);
        }
    }
    
    void OnTriggerStay(Collider other)
    {
        // Only grab objects that stay in zone while grabbing is active
        if (isGrabbing && other.CompareTag("Character"))
        {
            if (!grabbedObjects.Contains(other.gameObject))
            {
                GrabSingleObject(other.gameObject);
            }
        }
    }
    
    public void GrabObjects()
    {
        isGrabbing = true; // Enable grabbing
        Debug.Log("Attempting to grab " + objectsInZone.Count + " objects");
        
        // Make a copy of the list to avoid modification during iteration
        List<GameObject> objectsToGrab = new List<GameObject>(objectsInZone);
        
        foreach (GameObject obj in objectsToGrab)
        {
            if (obj != null && !grabbedObjects.Contains(obj))
            {
                GrabSingleObject(obj);
            }
        }
    }
    
void GrabSingleObject(GameObject obj)
{
    Debug.Log("Grabbing: " + obj.name);
    
    Rigidbody rb = obj.GetComponent<Rigidbody>();
    if (rb != null)
    {
        // Remove ALL constraints while being carried
        rb.constraints = RigidbodyConstraints.None;
        rb.isKinematic = true;
        rb.useGravity = false;
    }
    
    // Disable ALL colliders on object and children
    Collider[] colliders = obj.GetComponentsInChildren<Collider>();
    foreach (Collider col in colliders)
    {
        col.enabled = false;
    }
    
    obj.transform.SetParent(clawParent);
    grabbedObjects.Add(obj);
}

public void ReleaseObjects()
{
    isGrabbing = false;
    Debug.Log("Releasing " + grabbedObjects.Count + " objects");
    
    foreach (GameObject obj in grabbedObjects)
    {
        if (obj != null)
        {
            obj.transform.SetParent(null);
            
            Rigidbody rb = obj.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
                // Restore constraints for wandering
                rb.constraints = RigidbodyConstraints.FreezePositionY | 
                                 RigidbodyConstraints.FreezeRotation;
            }
            
            // Re-enable ALL colliders
            Collider[] colliders = obj.GetComponentsInChildren<Collider>();
            foreach (Collider col in colliders)
            {
                col.enabled = true;
            }
        }
    }
    
    grabbedObjects.Clear();
    objectsInZone.Clear();
}
}