using UnityEngine;
using System.Collections;

public class CpuCombatant : MonoBehaviour, ICombatant
{
    [HideInInspector] public Vector2 cpuReactionRange = new Vector2(0.18f, 0.32f);

    private bool acted = false;
    private float actTime = 0f;
    private Coroutine routine;

    public bool HasActed => acted;
    public float ActTime => actTime;

    public void OnSignalShown(float signalTime)
    {
        if (routine != null) StopCoroutine(routine);
        acted = false;
        actTime = 0f;
        routine = StartCoroutine(React(signalTime));
    }

    private IEnumerator React(float signalTime)
    {
        float delay = Random.Range(cpuReactionRange.x, cpuReactionRange.y);
        yield return new WaitForSeconds(delay);
        acted = true;
        actTime = Time.time;
    }
}