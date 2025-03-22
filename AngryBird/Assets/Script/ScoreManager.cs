using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager instance;

    private int totalScore = 0;

    [SerializeField] private TextMeshProUGUI scoreText;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        UpdateScoreUI();
    }

    public void AddScore(int score)
    {
        totalScore += score;
        UpdateScoreUI();
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = "Score : " + totalScore;
        }
    }
}
