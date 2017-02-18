using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlePanel : MonoBehaviour
{
    private Animator anim;       // cach animator component

    private int slideOutHash;    // cash animator parameters
    private int slideInHash;     // cash animator parameters

    [SerializeField]
    private HUD hudScript;       // reference to the HUD script to slide the head in

    [SerializeField]
    private GameObject player;   // activate the player when play button is pressed

    void Awake()
    {
        anim = GetComponent<Animator>();

        slideOutHash = Animator.StringToHash("SlideOut");
        slideInHash = Animator.StringToHash("SlideIn");
    }

    void Start()
    {
        GameManager.Instance.GameOverEvent.AddListener(OnGameOver);
    }

    /// <summary>
    /// when snake image is clicked, slide out and start game
    /// </summary>
    public void OnPlay()
    {
        GameManager.Instance.gameState = GameState.Playing;
        SoundManager.Instance.PlayMenuMusic();

        // .. Slide out
        anim.SetTrigger(slideOutHash);

        // .. Slide in the HUD
        StartCoroutine(SlideInHUD(.5f));

        // .. Activate player
        StartCoroutine(ActivatePlayer(.85f));
    }

    IEnumerator SlideInHUD(float delay, bool slideIn = true)
    {
        yield return new WaitForSeconds(delay);

        if (slideIn)
            hudScript.SlideIn();
        else
            hudScript.SlideOut();
    }

    IEnumerator ActivatePlayer(float delay)
    {
        yield return new WaitForSeconds(delay);

        GameManager.Instance.GameCommencingEvent.Invoke();   // Game is commencing here 

        player.SetActive(true);
    }

    /// <summary>
    /// Reset game when player loses
    /// </summary>
    private void OnGameOver()
    {
        StartCoroutine(ResetGameAfter(1f));
    }

    IEnumerator ResetGameAfter(float dur)
    {
        yield return new WaitForSeconds(dur);

        // .. Invoke the reset event after dur
        GameManager.Instance.ResetEvent.Invoke();

        OnReturn();
    }

    /// <summary>
    /// Slide In the main menu animation and hide the HUD
    /// </summary>
    private void OnReturn()
    {
        // .. Slide In main menu UI
        anim.SetTrigger(slideInHash);

        // .. Slide out the HUD
        StartCoroutine(SlideInHUD(0f, false));

        // .. Deactivate the player
        player.SetActive(false);
    }
}
