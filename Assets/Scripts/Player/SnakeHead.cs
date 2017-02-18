using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeHead : MonoBehaviour {

	void Start ()
    {
        // .. Used to update score when fruit ate
        GameManager.Instance.FruitAteEvent.AddListener(OnFruitAte);
    }

    /// <summary>
    /// invoked when the snake eats a fruit
    /// </summary>
    private void OnFruitAte(int scoreAddition, Vector3 position)
    {
        // .. Shake snake's head when he eats a fuit
        iTween.ShakeScale(transform.GetChild(0).gameObject, iTween.Hash(
                "amount", new Vector3(1f, 1f, 1f),
                "time", .25f,
                "delay", 0f,
                "easeType", iTween.EaseType.linear));
    }
}
