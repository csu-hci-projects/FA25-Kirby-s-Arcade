using UnityEngine;

public class PlayerCombatant : MonoBehaviour, ICombatant
{
    [Header("Input")]
    public KeyCode legacyKey = KeyCode.Space;
    public bool useNewInputSystem = true;

    [Header("Animation")]
    [SerializeField] private Animator animator;          // assign in Inspector (or it will auto-find)
    [SerializeField] private int layerIndex = 0;         // usually 0
    [SerializeField] private string idleState = "Idle";  // EXACT state name in your Animator

    private bool acted = false;
    private float actTime = 0f;
    private bool canAct = false;

    public bool HasActed => acted;
    public float ActTime => actTime;

    void Awake()
    {
        if (!animator) animator = GetComponentInChildren<Animator>();
        if (!animator)
            Debug.LogError("[PlayerCombatant] Animator missing. Assign an Animator on this GameObject or a child.");
    }

    void Start()
    {
        SafePlay(idleState);
    }

    private void SafePlay(string state)
    {
        if (!animator) return;

        int stateHash = Animator.StringToHash(state);
        if (animator.HasState(layerIndex, stateHash))
        {
            animator.Play(stateHash, layerIndex, 0f);
        }
        else
        {
            // Helpful debug so you know what to rename in the Animator
            Debug.LogError($"[PlayerCombatant] Animator state '{state}' not found on layer {layerIndex}. " +
                           $"Check the exact state name in the Animator Controller.");
        }
    }

    public void OnSignalShown(float signalTime)
    {
        acted = false;
        actTime = 0f;
        canAct = true;
    }

    void Update()
    {
        if (!canAct) return;

        bool pressed = false;
#if ENABLE_INPUT_SYSTEM
        if (useNewInputSystem && UnityEngine.InputSystem.Keyboard.current != null)
            pressed = UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame;
        else
#endif
            pressed = Input.GetKeyDown(legacyKey);

        if (pressed)
        {
            acted = true;
            actTime = Time.time;
            canAct = false;
        }
    }
}