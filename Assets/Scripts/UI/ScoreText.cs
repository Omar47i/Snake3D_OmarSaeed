using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreText : MonoBehaviour {

    public float aliveDuration = .6f;           // this score text will stay alive for this duration

    private SpriteRenderer textSpriteRenderer;  // used for fading colot to clear

    // Use this for initialization
    void Awake()
    {
        iTween.Init(gameObject);

        textSpriteRenderer = GetComponent<SpriteRenderer>();
	}

    void Start()
    {
        ScoreTextAnimation();
    }

    /// <summary>
    /// Animate scoring text to appear if it was floating up
    /// </summary>
    private void ScoreTextAnimation()
    {
        // .. Tween position
        iTween.MoveAdd(gameObject, iTween.Hash(
                "y", .4f,
                "time", aliveDuration,
                "delay", 0f,
                "easeType", iTween.EaseType.easeOutQuad));

        // .. Tween color
        iTween.ValueTo(gameObject, iTween.Hash(
                "from", textSpriteRenderer.color,
                "to", Color.clear,
                "time", aliveDuration * .5f,
                "delay", aliveDuration * .5f,
                "onupdate", "OnColorUpdated",
                "easeType", iTween.EaseType.linear));

        // .. Tween scale
        iTween.ScaleTo(gameObject, iTween.Hash(
                "scale", new Vector3(1f, 1f, 1f),
                "time", aliveDuration,
                "delay", 0f,
                "easeType", iTween.EaseType.easeOutQuad));
    }

    private void OnColorUpdated(Color color)
    {
        textSpriteRenderer.color = color;
    }
}
