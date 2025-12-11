using UnityEngine;
using TMPro;
using System.Collections;

public class SamuraiDuelManager : MonoBehaviour
{
    [Header("UI")]
    public TextMeshProUGUI titleText;
    public TextMeshProUGUI signalText;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI infoText;

    [Header("Difficulty")]
    public DifficultyConfig difficulty;

    [Header("Combatants")]
    public PlayerCombatant player;
    public CpuCombatant cpu;

    private enum State { Idle, Arming, WaitingForSignal, SignalShown, Resolved, Lock }
    private State state = State.Idle;

    private float signalTime;
    private bool drawShown;

    void Start() { ResetUI(); }

    void Update()
    {
        switch (state)
        {
            case State.Idle:
                // Space to start
                if (UnityEngine.InputSystem.Keyboard.current != null &&
                    UnityEngine.InputSystem.Keyboard.current.spaceKey.wasPressedThisFrame)
                {
                    StartCoroutine(RoundFlow());
                }
                break;

            case State.SignalShown:
                UpdateTimer();
                // If both acted, resolve
                if (player.HasActed && cpu.HasActed) Resolve();
                break;

            case State.Resolved:
                // Enter to restart
                if (UnityEngine.InputSystem.Keyboard.current != null &&
                    UnityEngine.InputSystem.Keyboard.current.enterKey.wasPressedThisFrame)
                {
                    ResetForNextRound();
                }
                break;
        }

        // Escape to hub
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("Hub3D",
                UnityEngine.SceneManagement.LoadSceneMode.Single);
        }
    }

    private IEnumerator RoundFlow()
    {
        state = State.Arming;
        drawShown = false;
        timerText.text = "0 ms";
        signalText.text = "…";
        titleText.text = "Get ready…";
        infoText.text = "Don’t false start. Space = attack • Enter = restart";

        // tiny grace
        yield return new WaitForSeconds(0.12f);

        state = State.WaitingForSignal;

        // wait random delay
        float wait = Random.Range(difficulty.randomWaitRange.x, difficulty.randomWaitRange.y);
        yield return new WaitForSeconds(wait);

        // show DRAW!
        signalTime = Time.time;
        signalText.text = "DRAW!";
        drawShown = true;
        state = State.SignalShown;

        // notify combatants
        cpu.cpuReactionRange = difficulty.cpuReactionRange;
        cpu.OnSignalShown(signalTime);
        player.OnSignalShown(signalTime);
    }

    private void Resolve()
    {
        state = State.Lock;

        float p = player.ActTime - signalTime;
        float c = cpu.ActTime - signalTime;

        if (Mathf.Abs(p - c) <= difficulty.tieWindow) titleText.text = "Tie!";
        else if (p < c) titleText.text = "Player wins!";
        else titleText.text = "CPU wins!";

        infoText.text = "Enter = restart • Esc = Hub";
        StartCoroutine(ResolvePause());
    }

    private IEnumerator ResolvePause()
    {
        yield return new WaitForSeconds(difficulty.lockAfterResolve);
        state = State.Resolved;
    }

    private void UpdateTimer()
    {
        if (!drawShown) { timerText.text = "0 ms"; return; }

        float t;
        if (player.HasActed) t = player.ActTime - signalTime;
        else t = Time.time - signalTime;

        int ms = Mathf.Max(0, Mathf.RoundToInt(t * 1000f));
        timerText.text = ms + " ms";
    }

    private void ResetForNextRound()
    {
        ResetUI();
        state = State.Idle;
    }

    private void ResetUI()
    {
        titleText.text = "SAMURAI DUEL";
        signalText.text = " ";
        timerText.text = "0 ms";
        infoText.text = "Space = Start • Esc = Hub";
    }
}