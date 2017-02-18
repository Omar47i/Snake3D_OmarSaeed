using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SnakeNode : MonoBehaviour {

    void OnTriggerEnter(Collider other)
    {
        // .. We collider with the player (wall's collision layer only collides with the player's head)
        if (GameManager.Instance.gameState == GameState.Playing)
        {
            GameManager.Instance.GameOverEvent.Invoke();
        }
    }
}
