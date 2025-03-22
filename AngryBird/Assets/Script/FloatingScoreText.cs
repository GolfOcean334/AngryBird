using UnityEngine;
using TMPro;

public class FloatingScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float scaleMultiplier;

    private float timer = 0f;
    private Transform parent;

    public void Initialize(int score)
    {
        if (textMesh != null)
        {
            textMesh.text = "+" + score.ToString();
            float newScale = 1f + ((score / 100) * scaleMultiplier);
            textMesh.transform.localScale = Vector3.one * newScale;
        }

        parent = new GameObject("FloatingTextParent").transform;
        parent.position = transform.position;
        transform.SetParent(parent);
    }

    private void Update()
    {
        parent.position += Vector3.up * floatSpeed * Time.deltaTime;
        timer += Time.deltaTime;
        if (timer >= duration)
        {
            Destroy(parent.gameObject);
        }
    }
}
