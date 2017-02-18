using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;

public enum GameState
{
    Playing,
    MainMenu,
    Revive,
};

public class FruitAteEventBase : UnityEvent<int, Vector3>
{ }

public class CreateBombEventBase : UnityEvent<int>
{ }

public class GameManager : MonoBehaviour {

    public static GameManager Instance;        // Make it singleton!

    [HideInInspector]
    public GameObject player;                  // Store a global reference for the player

    [HideInInspector]                          // an event to be invoked when play button is pressed and starting animation finishes
    public UnityEvent GameCommencingEvent = new UnityEvent();

    [HideInInspector]                          // an event to be invoked when hitting a wall or obstacle
    public UnityEvent GameOverEvent = new UnityEvent();

    [HideInInspector]                          // an event to be invoked when reseting the game
    public UnityEvent ResetEvent = new UnityEvent();

    [HideInInspector]                          // an event to be invoked when creating a bomb pickup
    public CreateBombEventBase CreateBombEvent = new CreateBombEventBase();

    [HideInInspector]                          // an event to be invoked when destroying the bomb
    public UnityEvent BombDepletedEvent = new UnityEvent();

    [HideInInspector]                          // an event to be invoked when eating a fruit
    public FruitAteEventBase FruitAteEvent = new FruitAteEventBase();

    [HideInInspector]
    public GameState gameState;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else if (Instance != this)   // duplicate
        {
            Destroy(gameObject);
            
            throw new UnityException("Duplicate Game Manager!!");
        }
       
        DontDestroyOnLoad(gameObject);

        GameOverEvent.AddListener(OnGameOver);
        ResetEvent.AddListener(OnReset);

        player = GameObject.FindGameObjectWithTag(Tags.player);

        gameState = GameState.MainMenu;
    }

    private void OnGameOver()
    {
        gameState = GameState.Revive;

        // .. Stop the music
        SoundManager.Instance.StopMusic();

        // .. Play gameover sound effect
        SoundManager.Instance.PlaySoundEffect(SoundEffectName.HIT, 1f);
    }

    private void OnReset()
    {
        gameState = GameState.MainMenu;
    }
}
