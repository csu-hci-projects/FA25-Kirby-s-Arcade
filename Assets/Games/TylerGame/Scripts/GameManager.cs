using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Gameplay")]
    public int startingLives = 3;
    public float scorePerSecond = 5f;
    public int scorePerBarrel = 100;

    public int scorePerJump = 100;
    public GameObject scorePopupPrefab;
[Header("Scenes")]
[SerializeField] private string menuSceneName = "StartKirbyKong";

    [Header("UI")]
    public TMP_Text scoreText;
    public TMP_Text livesText;
    public TMP_Text gameOverText;
    public TMP_Text restartHintText;

    // lives persist across scene reloads
    private static int livesGlobal = -1;

    private int currentScore;
    private int currentLives;
    private bool isGameOver;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        // initialize global lives once
        if (livesGlobal < 0)
            livesGlobal = startingLives;

        currentLives = livesGlobal;
        currentScore = 0;
        isGameOver = false;

        SafeSetActive(gameOverText, false);
        SafeSetActive(restartHintText, false);

        UpdateUI();
    }

void Update()
{
    if (isGameOver)
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Time.timeScale = 1f;
            livesGlobal = startingLives;   // reset lives for next run
            ReloadLevel();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Time.timeScale = 1f;
            livesGlobal = startingLives;   // also reset lives when going back to menu
            SceneManager.LoadScene(menuSceneName);
        }

        return;
    }

    currentScore += Mathf.RoundToInt(scorePerSecond * Time.deltaTime);
    UpdateScoreUI();
}

    /* ------------ CALLED FROM OTHER SCRIPTS ------------ */

    // This is what you call when the player is hit
    public void PlayerHitAndRestart()
    {
        if (isGameOver) return;

        livesGlobal--;
        currentLives = livesGlobal;
        UpdateLivesUI();

        if (livesGlobal <= 0)
        {
            livesGlobal = 0;
            StartCoroutine(GameOverRoutine());
        }
        else
        {
            StartCoroutine(LoseLifeAndRestartRoutine());
        }
    }

    public void RegisterBarrelDestroyed()
    {
        AddScore(scorePerBarrel);
    }

    public void PlayerReachedGoal()
{
    if (isGameOver) return;

    isGameOver = true;
    Time.timeScale = 0f; // pause the game

    // Optional bonus
    currentScore += 500;
    UpdateScoreUI();

    SafeSetText(gameOverText, "YOU WIN!");
    SafeSetText(restartHintText, "Press R to Restart\nPress M for Menu");
    SafeSetActive(gameOverText, true);
    SafeSetActive(restartHintText, true);
}


    public void AddScore(int amount)
    {
        if (isGameOver) return;
        currentScore += amount;
        UpdateScoreUI();
    }

    public void AwardJumpScore(Vector3 worldPosition)
{
    AddScore(scorePerJump);

    if (scorePopupPrefab != null)
    {
        Vector3 spawnPos = worldPosition + Vector3.up * 0.5f;
        GameObject obj = Instantiate(scorePopupPrefab, spawnPos, Quaternion.identity);

        ScorePopup popup = obj.GetComponent<ScorePopup>();
        if (popup != null)
        {
            popup.Init(scorePerJump);
        }
    }
}


    /* ---------------- COROUTINES ---------------- */

    // hit, still have lives: pause, then restart level
    System.Collections.IEnumerator LoseLifeAndRestartRoutine()
    {
        isGameOver = true;

        Time.timeScale = 0f;               // pause physics
        yield return new WaitForSecondsRealtime(0.8f);

        Time.timeScale = 1f;
        ReloadLevel();
    }

    // hit, no lives left: pause and show GAME OVER
    System.Collections.IEnumerator GameOverRoutine()
    {
        isGameOver = true;

        Time.timeScale = 0f;

        SafeSetText(gameOverText, "GAME OVER");
        SafeSetText(restartHintText, "Press R to Restart\nPress M for Menu");
        SafeSetActive(gameOverText, true);
        SafeSetActive(restartHintText, true);

        // wait here until player presses R in Update()
        yield break;
    }

    /* ---------------- INTERNAL HELPERS ---------------- */

    void ReloadLevel()
    {
        var scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.buildIndex);
    }

    void UpdateUI()
    {
        UpdateScoreUI();
        UpdateLivesUI();
    }

    void UpdateScoreUI()
    {
        SafeSetText(scoreText, "Score: " + currentScore);
    }

    void UpdateLivesUI()
    {
        SafeSetText(livesText, "Lives: " + currentLives);
    }

    bool IsValid(TMP_Text t)
    {
        return t != null && !t.Equals(null);
    }

    void SafeSetText(TMP_Text t, string text)
    {
        if (!IsValid(t)) return;
        t.text = text;
    }

    void SafeSetActive(TMP_Text t, bool active)
    {
        if (!IsValid(t)) return;
        t.gameObject.SetActive(active);
    }

    
}
