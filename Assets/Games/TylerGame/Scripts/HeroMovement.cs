using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(Collider))]
public class HeroMovement : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 6f;
    

    [Header("Jump")]
    public float jumpForce = 7f;
    public Transform groundCheck;           // empty child at feet
    public float groundCheckRadius = 0.15f; // tweak in Inspector
    public LayerMask groundLayer;

    public bool IsJumping { get; private set; }


    [Header("Jump Tuning")]
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;

    [Header("Ladders")]
    public float climbSpeed = 4f;
    public string ladderTag = "Ladder";
    public string playerLayerName = "Player";
    public string groundLayerName = "Ground";

    private bool inLadderZone;
    private float currentLadderTopY;


    private Rigidbody rb;
    private float inputX;
    private float inputY;
    private bool jumpRequested;
    private bool isOnLadder;
    private bool isGrounded;

    private int playerLayerIndex;
    private int groundLayerIndex;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.constraints = RigidbodyConstraints.FreezeRotation |
                         RigidbodyConstraints.FreezePositionZ;

        playerLayerIndex = LayerMask.NameToLayer(playerLayerName);
        groundLayerIndex = LayerMask.NameToLayer(groundLayerName);
    }

    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        // simple ground check using a small sphere at the feet
      if (groundCheck != null)
{
    isGrounded = Physics.CheckSphere(
        groundCheck.position,
        groundCheckRadius,
        groundLayer
    );
}
else
{
    isGrounded = Physics.Raycast(transform.position, Vector3.down, 0.4f, groundLayer);
}


        // request jump only if grounded and not on ladder
      if (Input.GetKeyDown(KeyCode.Space) && isGrounded && !isOnLadder)
{
    jumpRequested = true;
    
    IsJumping = true;
}
if (isGrounded || isOnLadder)
{
    IsJumping = false;
}
    }

void FixedUpdate()
{
    Vector3 v = rb.linearVelocity;

    bool wantsToClimb = Mathf.Abs(inputY) > 0.1f;
    bool belowLadderTop = groundCheck == null
        ? transform.position.y < currentLadderTopY - 0.05f
        : groundCheck.position.y < currentLadderTopY - 0.05f;

    if (inLadderZone && wantsToClimb && belowLadderTop)
    {
        // ENTER / STAY in ladder mode
        if (!isOnLadder && playerLayerIndex >= 0 && groundLayerIndex >= 0)
        {
            isOnLadder = true;
            Physics.IgnoreLayerCollision(playerLayerIndex, groundLayerIndex, true);
        }

        rb.useGravity = false;
        v.x = inputX * moveSpeed;
        v.y = inputY * climbSpeed;
        v.z = 0f;
        rb.linearVelocity = v;
    }
    else
    {
        // EXIT ladder mode
        if (isOnLadder && playerLayerIndex >= 0 && groundLayerIndex >= 0)
        {
            isOnLadder = false;
            Physics.IgnoreLayerCollision(playerLayerIndex, groundLayerIndex, false);
        }

        rb.useGravity = true;

        v.x = inputX * moveSpeed;
        v.z = 0f;
        rb.linearVelocity = v;

            if (jumpRequested)
            {
                jumpRequested = false;

                // snap any downward speed so jump is snappy
                v = rb.linearVelocity;
                if (v.y < 0f) v.y = 0f;
                rb.linearVelocity = v;

                rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            }
        }
        if (!isOnLadder)
    {
    Vector3 vel = rb.linearVelocity;

    // Faster fall
    if (vel.y < 0f)
    {
        vel += Vector3.up * Physics.gravity.y * (fallMultiplier - 1f) * Time.fixedDeltaTime;
    }
    // Shorter jump if player releases jump early
    else if (vel.y > 0f && !Input.GetButton("Jump"))
    {
        vel += Vector3.up * Physics.gravity.y * (lowJumpMultiplier - 1f) * Time.fixedDeltaTime;
    }

    rb.linearVelocity = vel;
    }

    }

void OnTriggerEnter(Collider other)
{
    if (other.CompareTag(ladderTag))
    {
        inLadderZone = true;
        currentLadderTopY = other.bounds.max.y;   // top of this ladder collider
    }
}

void OnTriggerExit(Collider other)
{
    if (other.CompareTag(ladderTag))
    {
        inLadderZone = false;

        if (isOnLadder && playerLayerIndex >= 0 && groundLayerIndex >= 0)
        {
            isOnLadder = false;
            Physics.IgnoreLayerCollision(playerLayerIndex, groundLayerIndex, false);
        }
    }
}


    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position,
                            transform.position + Vector3.down * 0.4f);
        }
    }
}
