using UnityEngine;

public class SamuraiFighter : MonoBehaviour
{
    [Header("Anim")]
    public Animator animator;
    public string runBoolParam = "isRunning"; // drives Idle↔Run
    public string idleState = "Idle";
    public string runState = "Run";
    public string dieTrigger = "Die";
    public string deathState = "Death";

    [Header("VFX (optional)")]
    public GameObject slashFX;
    public int sortingLayerBoost = 5;

    [Header("Foul (optional)")]
    public GameObject foulIndicator;     // child above the fighter, disabled by default

    public bool isDead { get; private set; }
    public bool isAttacking { get; private set; }

    private SpriteRenderer bodySr;
    private SpriteRenderer slashSr;
    private Animation slashAnim;

    void Awake()
    {
        if (!animator) animator = GetComponent<Animator>();
        bodySr = GetComponent<SpriteRenderer>();

        if (slashFX)
        {
            slashSr = slashFX.GetComponent<SpriteRenderer>();
            slashAnim = slashFX.GetComponent<Animation>();
            slashFX.SetActive(false);
        }

        HideFoul();
    }

    public void ResetForRound()
    {
        isDead = false;
        isAttacking = false;
        SetRun(false);
        PlayIdle();
        HideSlash();
        HideFoul();
    }

    public void PlayIdle()
    {
        if (!animator) return;
        isAttacking = false;
        SetRun(false);
        animator.Play(idleState, 0, 0f);
    }

    public void Attack()
    {
        if (isDead || isAttacking || !animator) return;
        isAttacking = true;
        SetRun(true); // Idle→Run via bool
    }

    // Animation Events on the Run clip:
public void OnStrike()
{
    if (isDead) return;
    PlaySlashFX();
    if (GameManagerMK.Instance != null)
        GameManagerMK.Instance.NotifyStrike(this);
}

    public void OnAttackEnd()
    {
        isAttacking = false;
        SetRun(false); // Run→Idle
    }

    void SetRun(bool v)
    {
        if (animator && !string.IsNullOrEmpty(runBoolParam))
            animator.SetBool(runBoolParam, v);
    }

    void PlaySlashFX()
    {
        if (!slashFX) return;

        slashFX.SetActive(true);
        if (slashSr && bodySr)
            slashSr.sortingOrder = bodySr.sortingOrder + sortingLayerBoost;

        var childAnimator = slashFX.GetComponent<Animator>();
        if (childAnimator) childAnimator.Play(0, 0, 0f);
        else if (slashAnim)
        {
            slashAnim.Stop();
            if (slashAnim.clip) slashAnim.Play(slashAnim.clip.name);
        }

        CancelInvoke(nameof(HideSlash));
        Invoke(nameof(HideSlash), 0.25f);
    }

    public void HideSlash()
    {
        if (slashFX) slashFX.SetActive(false);
    }

    public void ShowFoul(bool on)
    {
        if (foulIndicator) foulIndicator.SetActive(on);
    }

    public void HideFoul()
    {
        ShowFoul(false);
    }

    public void Die()
{
    if (isDead) return;

    isDead = true;
    isAttacking = false;

    SetRun(false);
    HideSlash();
    HideFoul();

    if (!animator) return;

    animator.ResetTrigger("Hit");

    if (!string.IsNullOrEmpty(dieTrigger))
    {
        animator.SetTrigger(dieTrigger);
    }
    else
    {
        animator.CrossFade(deathState, 0.05f, 0);
    }
}
}
