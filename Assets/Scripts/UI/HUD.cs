using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUD : MonoBehaviour
{
    [SerializeField]
    private Text currentScoreText;

    [SerializeField]
    private Text bestScoreText;

    private Animator anim;           // cach animator component
    private int slideInHash;         // cach animator parameters
    private int slideOutHash;

    void Start()
    {
        anim = GetComponent<Animator>();
        slideInHash = Animator.StringToHash("SlideIn");
        slideOutHash = Animator.StringToHash("SlideOut");

        // .. Listen to the score update events
        ScoreManager.Instance.CurrentScoreUpdatedEvent.AddListener(OnCurrentScoreUpdated);
        ScoreManager.Instance.BestScoreUpdatedEvent.AddListener(OnBestScoreUpdated);

        // .. Set the best score text at the start
        bestScoreText.text = ScoreManager.Instance.BestScore.ToString();
    }

    /// <summary>
    /// Update current score UI when its value changes
    /// </summary>
    private void OnCurrentScoreUpdated()
    {
        currentScoreText.text = ScoreManager.Instance.CurrentScore.ToString();
    }

    /// <summary>
    /// Update best score UI when its value changes
    /// </summary>
    private void OnBestScoreUpdated()
    {
        bestScoreText.text = ScoreManager.Instance.BestScore.ToString();
    }

    public void OnPause()
    {
        if (GameManager.Instance.gameState == GameState.Playing)
        {
            // .. Pause/Continue
            Time.timeScale = (Time.timeScale == 1f) ? 0f : 1f;
        }
    }

    /// <summary>
    /// Start sliding out animation 
    /// </summary>
    public void SlideOut()
    {
        anim.SetTrigger(slideOutHash);
    }

    public void SlideIn()
    {
        anim.SetTrigger(slideInHash);
    }
}
