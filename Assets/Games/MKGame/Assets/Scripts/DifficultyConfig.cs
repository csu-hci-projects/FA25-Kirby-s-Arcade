using UnityEngine;

[CreateAssetMenu(menuName="SamuraiDuel/Difficulty", fileName="Difficulty_Default")]
public class DifficultyConfig : ScriptableObject
{
    [Header("Signal timing before DRAW! (seconds)")]
    public Vector2 randomWaitRange = new Vector2(1.5f, 4.0f);

    [Header("CPU reaction after DRAW! (seconds)")]
    public Vector2 cpuReactionRange = new Vector2(0.18f, 0.32f);

    [Header("Tie window (seconds)")]
    public float tieWindow = 0.02f;

    [Header("Post-result lock (seconds)")]
    public float lockAfterResolve = 1.2f;
}