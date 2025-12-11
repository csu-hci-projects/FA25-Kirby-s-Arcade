using UnityEngine;

public class BarrelSpawner : MonoBehaviour
{
    public GameObject barrelPrefab;
    public Transform spawnPoint;      // where barrels appear
    public float spawnInterval = 3f;  // seconds between barrels
    public int direction = 1;         // 1 = right, -1 = left

    private float timer;

 void Start()
    {
        SpawnBarrel();
        timer = 0f;   // reset timer so next barrel spawns after interval
    }
    void Reset()
    {
        // auto-assign spawnPoint to this transform if none set
        if (spawnPoint == null)
            spawnPoint = transform;
    }

    void Update()
    {
        if (barrelPrefab == null || spawnPoint == null)
            return;

        timer += Time.deltaTime;

        if (timer >= spawnInterval)
        {
            SpawnBarrel();
            timer = 0f;
        }
    }

    void SpawnBarrel()
    {
        GameObject barrel = Instantiate(
            barrelPrefab,
            spawnPoint.position,
            spawnPoint.rotation
        );

        // set the roll direction on the new barrel
        BarrelController bc = barrel.GetComponent<BarrelController>();
        if (bc != null)
        {
            bc.direction = direction;
        }
    }
}
