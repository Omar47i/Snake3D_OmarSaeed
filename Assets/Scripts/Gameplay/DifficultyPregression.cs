using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyPregression : MonoBehaviour
{
    private float minMovementFreq = .02f; // This is the minimum movement frequency of the snake as we don't want the game to be impossible to play

    private float difficultyFactor = .01f;// Decrease movement frequency by this value

    private int foodCountToIncrease = 4;  // each time we eat 4 fruits, increase the difficulty
    private int fruitAteCount = 0;        // keep track of number of eaten fruits

    private int spawnBombAfter = 2;       // start to spawn bombs after 2 eaten fruits
    private float bombSpawnCounter = 0f;  // used to spawn bombs at specific intervals

    private float minBombSpawnTime = 2f;
    private float maxBombSpawnTime = 11f;
    private float nextBombSpawnTime = 0f;
    private bool canSpawnBombs = true;    // used to prevent spawning a bomb while another bomb still exists

    [SerializeField]
    private PlayerController playerController;
    void Start()
    {
        // .. Listen to the fruite ate event
        GameManager.Instance.FruitAteEvent.AddListener(OnFruitAte);

        // .. Listen to the reset event to reset difficulty
        GameManager.Instance.ResetEvent.AddListener(OnReset);

        // .. Listen to the bomb delpleted event to prepare another bomb for spawning
        GameManager.Instance.BombDepletedEvent.AddListener(OnBombDepleted);
    }

    /// <summary>
    /// Increase difficulty based on the number of eated fruits
    /// </summary>
    /// <param name="scoreAdded">Amount of score added by eating the fruit, Ignore</param>
    /// <param name="position">position of the eaten fruit, Ignore</param>
    private void OnFruitAte(int scoreAdded, Vector3 position)
    {
        fruitAteCount++;

        if (fruitAteCount % foodCountToIncrease == 0)
        {
            // .. Increase difficulty
            playerController.DecreaseMovementFrequency(difficultyFactor, minMovementFreq);
        }
    }

    private void Update()
    {
        // .. Spawn bombs only if we ate at least 2 fruits
        if (fruitAteCount >= 2 && canSpawnBombs)
        {
            // .. Calculate next bomb spawn time
            if (bombSpawnCounter == 0)
            {
                nextBombSpawnTime = Time.time + Random.Range(minBombSpawnTime, maxBombSpawnTime);
            }

            bombSpawnCounter += Time.deltaTime;

            // .. We reached the next spawn time, spawn a bomb!
            if (bombSpawnCounter + Time.time >= nextBombSpawnTime)
            {
                // .. Invoke the bomb creation event so that PickupSpawner.cs spawns it!
                GameManager.Instance.CreateBombEvent.Invoke();

                // .. Reset counter
                bombSpawnCounter = 0f;

                // .. Stop spawning bombs right now
                canSpawnBombs = false;
            }
        }
    }

    /// <summary>
    /// Enable spawning bombs when the last bomb is destroyed
    /// </summary>
    private void OnBombDepleted()
    {
        canSpawnBombs = true;
    }

    /// <summary>
    /// Reset eaten fruit count when returning to main menu
    /// </summary>
    private void OnReset()
    {
        fruitAteCount = 0;
    }
}
