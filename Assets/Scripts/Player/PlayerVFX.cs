using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerVFX : MonoBehaviour {

    [SerializeField]
    private GameObject hitEffect;        // to be instantiated when hitting a wall, obstacle, or self

    private Transform headPosition;      // create the effect at snake's head position

    void Start()
    {
        headPosition = transform.GetChild(0);

        GameManager.Instance.GameOverEvent.AddListener(OnGameOver);
    }

    private void OnGameOver()
    {
        // .. Create the effect
        Instantiate(hitEffect, headPosition.position, Quaternion.identity);
    }
}
