using System.Collections;
using UnityEngine;
using TMPro;

public class GameManagerMK : MonoBehaviour
{
    public static GameManagerMK Instance { get; private set; }
    void Awake() { Instance = this; }

    [Header("Fighters")]
    public SamuraiFighter player;
    public SamuraiFighter cpu;

    [Header("UI (optional)")]
    public TMP_Text titleText;   // ok to leave null
    public TMP_Text signalText;
    public TMP_Text timerText;
    public TMP_Text infoText;

    [Header("Timing")]
    public Vector2 cpuReactionRangeMs = new Vector2(120f, 260f); // CPU delay after DRAW (ms)
    public float preCueDelayMin = 0.5f;  // suspense before DRAW (s)
    public float preCueDelayMax = 1.0f;

    [Header("Safety (no player timeout)")]
    public float cpuHitGrace = 0.20f;

    [Header("Opponents")]
    public int totalOpponents = 5;
    public Vector2[] opponentCpuReactionRangesMs = new Vector2[5];

    [Header("Effects (optional)")]
    public UIEffects uiEffects; // blink canvas

    private enum State { Idle, WaitingForCue, Live, Resolved }
    private State state = State.Idle;

    private bool resultLocked;
    private bool cueShown;
    private float cueTime;

    // CPU bookkeeping
    private bool cpuAttackSent;
    private float cpuSwungTime;

    // Ladder
    private int currentOpponentIndex = 0;
    private int enemiesDefeated = 0;
    private SamuraiFighter lastWinner;

    // Early foul
    private bool playerFouledEarly = false;

    void Start()
    {
        EnsureOpponentRanges();
        ResetUI();
        ToIdle();
    }

    void Update()
    {
        if (state == State.Idle && Input.GetKeyDown(KeyCode.Space))
        {
            currentOpponentIndex = 0;
            enemiesDefeated = 0;
            StartCoroutine(RoundRoutine());
            return;
        }

        if (state == State.WaitingForCue && Input.GetKeyDown(KeyCode.Space))
        {
            HandleEarlyFoul();
            return;
        }

        if (state == State.Live && cueShown && !playerFouledEarly && Input.GetKeyDown(KeyCode.Space))
        {
            if (player != null && !player.isDead) player.Attack();
        }

        if (state == State.Live && cueShown && timerText != null)
        {
            int ms = Mathf.Max(0, Mathf.RoundToInt((Time.time - cueTime) * 1000f));
            timerText.text = ms + " ms";
        }

        if (state == State.Live && cpuAttackSent && !resultLocked &&
            Time.time >= cpuSwungTime + cpuHitGrace)
        {
            ForceResolve(cpu);
        }
    }

    IEnumerator RoundRoutine()
    {
        state = State.WaitingForCue;
        resultLocked = false;
        cueShown = false;
        cpuAttackSent = false;
        playerFouledEarly = false;

        if (player) player.ResetForRound();
        if (cpu)    cpu.ResetForRound();

        Vector2 range = GetCurrentOpponentRange();
        cpuReactionRangeMs = range;

        SafeSet(titleText, "Samurai " + (currentOpponentIndex + 1) + " / " + totalOpponents);
        SafeSet(signalText, "…");
        SafeSet(timerText, "0 ms");
        SafeSet(infoText, "Wait for DRAW, then Space = Slash");

        float delay = Random.Range(preCueDelayMin, preCueDelayMax);
        yield return new WaitForSeconds(delay);

        cueShown = true;
        cueTime = Time.time;
        SafeSet(signalText, "DRAW!");
        state = State.Live;

        float cpuDelay = Random.Range(cpuReactionRangeMs.x, cpuReactionRangeMs.y) / 1000f;
        float cpuFireAt = cueTime + cpuDelay;

        while (!resultLocked)
        {
            if (!cpuAttackSent && Time.time >= cpuFireAt && cpu != null && !cpu.isDead)
            {
                cpu.Attack();
                cpuAttackSent = true;
                cpuSwungTime = Time.time;
            }
            yield return null;
        }
    }

    public void NotifyStrike(SamuraiFighter striker)
    {
        if (resultLocked || state != State.Live) return;

        SamuraiFighter winner = striker;
        SamuraiFighter loser  = (striker == player) ? cpu : player;

        if (loser) loser.Die();
        resultLocked = true;
        state = State.Resolved;
        lastWinner = winner;

        SafeSet(titleText, winner == player ? "Player wins!" : "CPU wins!");
        SafeSet(infoText, "");

        StartCoroutine(HandleRoundEnd());
    }

    private IEnumerator HandleRoundEnd()
    {
        if (uiEffects != null)
            yield return uiEffects.Blink(1f, 2.0f);
        else
            yield return new WaitForSeconds(0.18f);

        if (lastWinner == player)
        {
            enemiesDefeated++;

            if (enemiesDefeated >= totalOpponents)
            {
                SafeSet(titleText, "GRAND CHAMPION SAMURAI");
                SafeSet(infoText, "Space = Start");

                if (uiEffects != null)
                    yield return uiEffects.Blink(0f, 0.25f);
                else
                    yield return new WaitForSeconds(0.25f);

                ToIdle();
            }
            else
            {
                currentOpponentIndex = Mathf.Clamp(currentOpponentIndex + 1, 0, totalOpponents - 1);

                SafeSet(titleText, "Next challenger approaching");
                SafeSet(infoText, "Enemies defeated " + enemiesDefeated + " / " + totalOpponents);

                yield return new WaitForSeconds(0.4f);

                if (uiEffects != null)
                    yield return uiEffects.Blink(0f, 0.25f);
                else
                    yield return new WaitForSeconds(0.25f);

                StartCoroutine(RoundRoutine());
            }
        }
        else
        {
            SafeSet(titleText, "CPU wins!");
            SafeSet(infoText, "You defeated " + enemiesDefeated + " / " + totalOpponents + ". Space = Start");

            if (uiEffects != null)
                yield return uiEffects.Blink(0f, 0.25f);
            else
                yield return new WaitForSeconds(0.25f);

            ToIdle();
        }
    }

    private void ForceResolve(SamuraiFighter forcedWinner)
    {
        if (resultLocked || state != State.Live) return;
        NotifyStrike(forcedWinner);
    }

    private void HandleEarlyFoul()
    {
        if (playerFouledEarly) return;
        playerFouledEarly = true;
        if (player) player.ShowFoul(true);
    }

    private void ToIdle()
    {
        state = State.Idle;
        resultLocked = false;
        cueShown = false;
        cpuAttackSent = false;
        playerFouledEarly = false;

        if (player) player.ResetForRound();
        if (cpu)    cpu.ResetForRound();

        ResetUI();
        SafeSet(infoText, "Space = Start");
    }

    private void ResetUI()
    {
        SafeSet(titleText, "SAMURAI DUEL");
        SafeSet(signalText, "");
        SafeSet(timerText, "0 ms");
    }

    private void SafeSet(TMP_Text t, string v)
    {
        if (t) t.text = v;
    }

    private void EnsureOpponentRanges()
    {
        if (totalOpponents < 1) totalOpponents = 1;

        if (opponentCpuReactionRangesMs == null || opponentCpuReactionRangesMs.Length < totalOpponents)
        {
            opponentCpuReactionRangesMs = new Vector2[totalOpponents];
        }

        Vector2[] defaults =
        {
            new Vector2(120f, 160f),
            new Vector2(100f, 140f),
            new Vector2(70f, 110f),
            new Vector2(60f, 90f),
            new Vector2(50f, 80f)
        };

        for (int i = 0; i < totalOpponents; i++)
        {
            bool unset = opponentCpuReactionRangesMs[i].x <= 0f && opponentCpuReactionRangesMs[i].y <= 0f;
            if (unset)
            {
                int idx = Mathf.Min(i, defaults.Length - 1);
                opponentCpuReactionRangesMs[i] = defaults[idx];
            }
        }
    }

    private Vector2 GetCurrentOpponentRange()
    {
        if (opponentCpuReactionRangesMs == null || opponentCpuReactionRangesMs.Length == 0)
            return cpuReactionRangeMs;

        int idx = Mathf.Clamp(currentOpponentIndex, 0, opponentCpuReactionRangesMs.Length - 1);
        return opponentCpuReactionRangesMs[idx];
    }
}
