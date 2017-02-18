using UnityEngine.UI;
using UnityEngine;

public class PauseButton : MonoBehaviour {

    [SerializeField]
    private Sprite pauseIcon;
    [SerializeField]
    private Sprite playIcon;

    private Image imageComponent;       // reference to the image component 
    private bool pauseActive = true;    // the current active icon

    void Start()
    {
        imageComponent = GetComponent<Image>();
    }

    public void OnPause()
    {
        if (pauseActive)
        {
            // .. Load the play icon
            imageComponent.overrideSprite = playIcon;

            pauseActive = false;
        }
        else
        {
            // .. Load the pause icon
            imageComponent.overrideSprite = pauseIcon;

            pauseActive = true;
        }
    }
}
