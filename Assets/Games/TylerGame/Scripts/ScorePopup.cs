using UnityEngine;
using TMPro;

public class ScorePopup : MonoBehaviour
{
    public float floatSpeed = 1f;
    public float lifetime = 1f;

    private TMP_Text text;
    private Color startColor;
    private float timer;

void Awake()
{
    text = GetComponentInChildren<TMP_Text>();
    if (text == null)
        Debug.LogError("ScorePopup: No TMP_Text found in children!");
    else
        startColor = text.color;
}

void Update()
{
    timer += Time.deltaTime;

    transform.position += Vector3.up * floatSpeed * Time.deltaTime;

    if (text != null)
    {
        float t = timer / lifetime;
        Color c = startColor;
        c.a = Mathf.Lerp(1f, 0f, t);
        text.color = c;
    }

    if (timer >= lifetime)
        Destroy(gameObject);

    if (Camera.main != null)
        transform.rotation = Camera.main.transform.rotation;
}

    public void Init(int amount)
    {
        if (text != null)
        {
            text.text = "+" + amount;
        }
    }


}
