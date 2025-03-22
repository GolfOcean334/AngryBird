using UnityEngine;
using TMPro;
using DG.Tweening;

public class FloatingScoreText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textMesh;
    [SerializeField] private float floatSpeed = 2f;
    [SerializeField] private float duration = 1f;
    [SerializeField] private float scaleMultiplier;
    [SerializeField] private float maxScale = 2.5f;
    [SerializeField] private float fadeDuration = 0.3f;

    private float timer = 0f;
    private Transform parent;

    public void Initialize(int score)
    {
        if (textMesh != null)
        {
            textMesh.text = "+" + score.ToString();

            float newScale = Mathf.Min(1f + (score * scaleMultiplier), maxScale);
            textMesh.transform.localScale = Vector3.one * newScale;

            textMesh.alpha = 0;
            textMesh.DOFade(1f, fadeDuration);
        }

        parent = new GameObject("FloatingTextParent").transform;
        parent.position = transform.position;
        transform.SetParent(parent);
    }

    private void Update()
    {
        parent.position += Vector3.up * floatSpeed * Time.deltaTime;
        timer += Time.deltaTime;

        if (timer >= duration - fadeDuration)
        {
            textMesh.DOFade(0f, fadeDuration).OnComplete(() => Destroy(parent.gameObject));
        }
    }
}
