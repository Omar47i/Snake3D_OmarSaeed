using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : PooledObject
{
    [SerializeField]
    private float Minimum = 2f;

    [SerializeField]
    private float Maximum = 11f;

    void Awake()
    {
        iTween.Init(gameObject);     // initialize tween engine for this object to avoid hiccups

        StartingAnimation();         // play the starting animation

        // .. Listen to the game over event so that bomb doesn't stau when player loses
        GameManager.Instance.GameOverEvent.AddListener(OnGameOver);
    }

    public override void OnPooledObjectActivated()
    {
        base.OnPooledObjectActivated();

        // .. Listen to the game over event so that bomb doesn't stau when player loses
        GameManager.Instance.GameOverEvent.AddListener(OnGameOver);

        StartingAnimation();         // play the starting animation

        StartCoroutine(DestroyAfter(Random.Range(Minimum, Maximum)));
    }

    /// <summary>
    /// Destroy this bomb after a random duration
    /// </summary>
    /// <param name="dur"></param>
    /// <returns></returns>
    IEnumerator DestroyAfter(float dur)
    {
        yield return new WaitForSeconds(dur);

        ObjectsPoolManager.Instance.DestroyGameObjectWithPooledChildren(gameObject);

        // .. When destroying this bomb, invoke the event to inform the DIfficulyProgression.cs to spawn a new bomb
        GameManager.Instance.BombDepletedEvent.Invoke();
    }

    void OnTriggerEnter(Collider other)
    {
        // .. We collider with the player (fruit's collision layer only collides with the player's head)
        if (GameManager.Instance.gameState == GameState.Playing)
        {
            GameManager.Instance.GameOverEvent.Invoke();
;
            ObjectsPoolManager.Instance.DestroyGameObjectWithPooledChildren(gameObject);
        }
    }

    private void OnGameOver()
    {
        ObjectsPoolManager.Instance.DestroyGameObjectWithPooledChildren(gameObject);
    }

    private void StartingAnimation()
    {
        iTween.ScaleAdd(transform.GetChild(0).gameObject, iTween.Hash(
                "amount", new Vector3(.1f, .1f, .1f),
                "time", .15f,
                "looptype", iTween.LoopType.pingPong,
                "easeType", iTween.EaseType.linear));
    }

    public override void OnPooledObjectDeactivated()
    {
        base.OnPooledObjectDeactivated();

        GameManager.Instance.GameOverEvent.RemoveListener(OnGameOver);
    }
}
