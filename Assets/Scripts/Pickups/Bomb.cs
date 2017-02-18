using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {

    void Awake()
    {
        iTween.Init(gameObject);     // initialize tween engine for this object to avoid hiccups

        StartingAnimation();         // play the starting animation

        // .. Listen to the game over event so that bomb doesn't stau when player loses
        GameManager.Instance.GameOverEvent.AddListener(OnGameOver);
    }

    void OnTriggerEnter(Collider other)
    {
        // .. We collider with the player (fruit's collision layer only collides with the player's head)
        if (GameManager.Instance.gameState == GameState.Playing)
        {
            GameManager.Instance.GameOverEvent.Invoke();

            Destroy(gameObject);
            //ObjectsPoolManager.Instance.DestroyGameObjectWithPooledChildren(gameObject);
        }
    }

    private void OnGameOver()
    {
        Destroy(gameObject);
    }

    private void StartingAnimation()
    {
        iTween.ScaleAdd(transform.GetChild(0).gameObject, iTween.Hash(
                "amount", new Vector3(.1f, .1f, .1f),
                "time", .15f,
                "looptype", iTween.LoopType.pingPong,
                "easeType", iTween.EaseType.linear));
    }

    void OnDestroy()
    {
        GameManager.Instance.GameOverEvent.RemoveListener(OnGameOver);
        // .. When destroying this bomb, invoke the event to inform the DIfficulyProgression.cs to spawn a new bomb
        GameManager.Instance.BombDepletedEvent.Invoke();
    }
}
