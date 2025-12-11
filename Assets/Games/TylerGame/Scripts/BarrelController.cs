using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class BarrelController : MonoBehaviour
{
    [Header("Movement")]
    public float startSpeed = 6f;        // initial horizontal speed
    public float maxHorizontalSpeed = 11f;
    public int direction = 1;            // 1 = right, -1 = left

    [Header("Lifetime")]
    public float lifeTime = 30f;         // destroy after N seconds
    public float killBelowY = -10f;      // auto-destroy if falls off level

    private Rigidbody rb;

    public string playerTag = "Player";

    public Transform jumpZone;

    void LateUpdate()
    {
        // Prevent the jump zone from spinning with the barrel
        jumpZone.rotation = Quaternion.identity;
    }
private bool jumpScored = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();

        // lock to 2.5D plane (allow Z-rotation so it looks like it rolls)
        rb.constraints = RigidbodyConstraints.FreezePositionZ |
                         RigidbodyConstraints.FreezeRotationX |
                         RigidbodyConstraints.FreezeRotationY;
    }

    void Start()
    {
        // give it an initial push
        Vector3 v = rb.linearVelocity;
        v.x = direction * startSpeed;
        rb.linearVelocity = v;

        // optional, destroy after some time
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        // keep horizontal motion alive, let gravity handle Y
        Vector3 v = rb.linearVelocity;

        // if it ever slows too much, nudge it back toward target speed
        if (Mathf.Abs(v.x) < maxHorizontalSpeed)
        {
            float sign = v.x != 0 ? Mathf.Sign(v.x) : direction;
            v.x = sign * Mathf.Clamp(Mathf.Abs(v.x), startSpeed, maxHorizontalSpeed);
        }

        v.z = 0f;
        rb.linearVelocity = v;

        // add a bit of angular speed so it visibly spins
        rb.AddTorque(Vector3.forward * -direction * 5f, ForceMode.Acceleration);

        // kill if it falls off the world
        if (transform.position.y < killBelowY)
        {
            Destroy(gameObject);
        }
    }

private void OnTriggerEnter(Collider other)
{
    if (jumpScored) return;
    if (!other.CompareTag(playerTag)) return;

    // Get the player's rigidbody (from this collider or a parent)
    Rigidbody playerRb = other.attachedRigidbody;
    if (playerRb == null)
        playerRb = other.GetComponentInParent<Rigidbody>();

    if (playerRb == null)
    {
        return;
    }

    // 1) Is the player above the barrel?
    bool isAbove = other.transform.position.y > transform.position.y + 0.1f;

    // 2) Is the player actually moving vertically (jumping / falling)?
    float verticalSpeed = playerRb.linearVelocity.y;
    bool isMovingVertically = Mathf.Abs(verticalSpeed) > 0.1f;



    if (isAbove && isMovingVertically && GameManager.Instance != null)
    {
        jumpScored = true;
        GameManager.Instance.AwardJumpScore(transform.position);
    }
}


}
