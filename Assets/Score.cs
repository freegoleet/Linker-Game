using UnityEngine;
using UnityEngine.UI;

public class Score : MonoBehaviour
{
    [SerializeField] private Text scoreText;
    [SerializeField] private Text scoreMultiplierText;

    [Header("Score Settings")]
    [SerializeField] private int baseLinkScore = 10;
    [SerializeField] private float scoreMultiplier = 1.5f;

    [HideInInspector] public float currentScore;

    public void UpdateScore(int numberOfLinks) {

        currentScore += baseLinkScore * numberOfLinks * (scoreMultiplier * numberOfLinks);

        scoreText.text = currentScore.ToString("0");
        scoreMultiplierText.text = "0";
    }

    public void UpdateMultiplier(int currentNumberOfLinks) {
        scoreMultiplierText.text = "x" + (currentNumberOfLinks * scoreMultiplier).ToString();
    }
}
