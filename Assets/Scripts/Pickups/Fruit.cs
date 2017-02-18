using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fruit : PooledObject {

    [HideInInspector]
    public int scoreAddition = 10;

    void Awake()
    {
        iTween.Init(gameObject);     // initialize tween engine for this object to avoid hiccups

        StartingAnimation();         // play the starting animation
    }

    public override void OnPooledObjectActivated()
    {
        base.OnPooledObjectActivated();

        // .. When we get a pooled fruit object, play animation once
        StartingAnimation();
    }

    void OnTriggerEnter(Collider other)
    {
        // .. We collider with the player (fruit's collision layer only collides with the player's head)
        if (GameManager.Instance.gameState == GameState.Playing)
        {
            ObjectsPoolManager.Instance.DestroyPooledGameObject(gameObject);

            GameManager.Instance.FruitAteEvent.Invoke(scoreAddition, transform.position);
        }
    }

    private void StartingAnimation()
    {
        iTween.ShakePosition(gameObject, iTween.Hash(
                "x", .035f,
                "y", .035f,
                "time", .2f,
                "delay", .1f,
                "easeType", iTween.EaseType.easeOutCirc));
    }
}
