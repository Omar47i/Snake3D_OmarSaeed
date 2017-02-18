using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DifficultyPregression : MonoBehaviour
{
    private float minMovementFreq = .02f; // This is the minimum movement frequency of the snake as we don't want the game to be impossible to play

    private float difficultyFactor = .01f;// Decrease movement frequency by this value

    private int foodCountToIncreaseSpeed = 4;  // each time we eat 4 fruits, increase player's speed
    private int foodCountToIncreaseBombCount = 7;
    private int fruitAteCount = 0;        // keep track of number of eaten fruits

    private int spawnBombAfter = 2;       // start to spawn bombs after 2 eaten fruits
    private float bombSpawnCounter = 0f;  // used to spawn bombs at specific intervals

    private float minBombSpawnTime = 2f;
    private float maxBombSpawnTime = 11f;
    private float nextBombSpawnTime = 0f;
    private bool canSpawnBombs = true;    // used to prevent spawning a bomb while another bomb still exists

    private int bombCount = 1;            // amount of spawning bombs
    private int depletedCounter = 0;

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

        if (fruitAteCount % foodCountToIncreaseSpeed == 0)
        {
            // .. Increase player speed
            playerController.DecreaseMovementFrequency(difficultyFactor, minMovementFreq);
        }

        if (fruitAteCount % foodCountToIncreaseBombCount == 0)
        {
            // .. Increase number of spawned bombs at once
            bombCount++;
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
                depletedCounter = Random.Range(1, bombCount + 1);
                GameManager.Instance.CreateBombEvent.Invoke(depletedCounter);

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
        depletedCounter--;

        if (depletedCounter == 0)
            canSpawnBombs = true;
    }

    /// <summary>
    /// Reset eaten fruit count when returning to main menu
    /// </summary>
    private void OnReset()
    {
        canSpawnBombs = true;
        fruitAteCount = 0;
        bombSpawnCounter = 0f;
        bombCount = 1;
    }
}
