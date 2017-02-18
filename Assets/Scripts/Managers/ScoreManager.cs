using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ScoreManager : MonoBehaviour {

    [HideInInspector]
    public UnityEvent CurrentScoreUpdatedEvent; // to be fired when current score updates

    [HideInInspector]
    public UnityEvent BestScoreUpdatedEvent;    // to be fired when best score updates

    public static ScoreManager Instance;        // make me singleton!

    [SerializeField]
    private GameObject[] scorePrefabs;          // create a scoring text based on the added score

    private bool displayNewHighScore = false;   // display new high score text only one time 

    public int CurrentScore
    {
        get
        {
            return currentScore;
        }
        set
        {
            currentScore = value;

            CurrentScoreUpdatedEvent.Invoke();      // fire current score updated event here
        }
    }

    public int BestScore
    {
        set
        {
            bestScore = value;

            BestScoreUpdatedEvent.Invoke();         // fire best score updated event here

            PlayerSettings.SetBestScore(bestScore); // save the best score in playerprefs
        }
        get
        {
            return bestScore;
        }
    }

    private int currentScore;                // current user score
    private int bestScore;                   // fetched from PlayerPrefs if exists

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        else if (Instance != this)
            Destroy(gameObject);
                                            
        DontDestroyOnLoad(gameObject);       // Makeit survive scene changes assuming we have more than one scene

        CurrentScoreUpdatedEvent = new UnityEvent();
        BestScoreUpdatedEvent = new UnityEvent();

        InitScoreManager();
    }

    void InitScoreManager()
    {
        // .. Initialize score mamanger values
        currentScore = 0;

        // .. Get the last saved best score
        bestScore = PlayerSettings.GetBestScore();

        // .. Listen to the gameOver event to update the best score
        GameManager.Instance.GameOverEvent.AddListener(OnGameOver);

        // .. Used to update score when fruit ate
        GameManager.Instance.FruitAteEvent.AddListener(OnFruitAte);

        // .. Used to reset current score when returning to main menu
        GameManager.Instance.ResetEvent.AddListener(OnReset);
    }

    /// <summary>
    /// invoked when the snake eats a fruit
    /// </summary>
    private void OnFruitAte(int scoreAddition, Vector3 position)
    {
        // .. Increase player score
        CurrentScore += scoreAddition;

        // .. Display a new high score text if we got a new high score
        if (CurrentScore > BestScore && !displayNewHighScore && BestScore != 0)
        {
            displayNewHighScore = true;

            Instantiate(ScoringTextResolution(0), new Vector3(position.x, position.y, 5f), Quaternion.identity);

            // .. Play sfx
            SoundManager.Instance.PlaySoundEffect(SoundEffectName.NEW_HIGH_SCORE, 1f);
        }
        // .. Create a floating score text based on the score addition value
        else
        {
            Instantiate(ScoringTextResolution(scoreAddition), new Vector3(position.x, position.y, 5f), Quaternion.identity);
        }
    }

    /// <summary>
    /// Invoked by the GameOverEvent, used to update best score
    /// </summary>
    private void OnGameOver()
    {
        // .. We have a new best score
        if (currentScore > bestScore)
        {
            // .. Hide the current score and display only the best score
            BestScore = CurrentScore;
        }
    }

    private void OnReset()
    {
        CurrentScore = 0;
    }

    /// <summary>
    /// Get the correspnding prefab based on the score
    /// </summary>
    /// <param name="score">score added</param>
    /// <returns>the corresponding score text prefab</returns>
    private GameObject ScoringTextResolution(int score)
    {
        GameObject scorePrefab = null;

        switch(score)
        {
            case 5:
                scorePrefab = scorePrefabs[0];
                break;
            case 10:
                scorePrefab = scorePrefabs[1];
                break;
            case 20:
                scorePrefab = scorePrefabs[2];
                break;
            case 50:
                scorePrefab = scorePrefabs[3];
                break;
            case 0:
                scorePrefab = scorePrefabs[4];
                break;
        }

        return scorePrefab;
    }
}
