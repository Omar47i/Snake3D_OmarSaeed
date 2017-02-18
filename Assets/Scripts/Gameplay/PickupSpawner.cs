// PickupSpawner.cs: Spawns pickups like fruits and any other added pickup type
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupSpawner : MonoBehaviour
{
    [SerializeField]
    private List<Pickups> pickups;      // assign all pickups in the inspector

    private Transform pickupsParent;    // parent of all pickups, used for scene organization and deleting them on gameover

    private int bombAmount = 1;         // spawning count of bombs
    void Start()
    {
        // .. Listen to the fruite ate event to spawn a new fruit pickup
        GameManager.Instance.FruitAteEvent.AddListener(OnFruitAte);

        // .. Listen to the fruite ate event to spawn a new fruit pickup
        GameManager.Instance.CreateBombEvent.AddListener(OnCreateBomb);

        // .. Listen to the game commencing event to spawn a new fruit pickup at the start
        GameManager.Instance.GameCommencingEvent.AddListener(OnGameCommence);

        // .. Listen to the game over event to clear all pickups
        GameManager.Instance.GameOverEvent.AddListener(OnGameOver);

        pickupsParent = GameObject.FindGameObjectWithTag(Tags.pickupsParent).transform;
    }

    private void OnFruitAte(int scoreAdded, Vector3 position)
    {
        StartCoroutine(CreateFruitRandom());
    }

    private void OnCreateBomb(int amount = 1)
    {
        bombAmount = amount;

        StartCoroutine(CreateBombRandom());
    }

    private void OnGameCommence()
    {
        // .. Ensure that all pickups are cleared when starting the game
        OnGameOver();

        StartCoroutine(CreateFruitRandom(false));
    }

    /// <summary>
    /// generate a new fruit object at a random and valid position
    /// </summary>
    private IEnumerator CreateFruitRandom(bool playEatFruit = true)
    {
        while(true)
        {
            int ranX = Random.Range((int)WorldBorders.LeftBorder, (int)WorldBorders.RightBorder);
            int ranY = Random.Range((int)WorldBorders.BottomBorder, (int)WorldBorders.TopBorder);
            Vector3 newPosition = new Vector3(ranX, ranY, 5.72f);

            Collider[] hit = Physics.OverlapSphere(newPosition, (int)Metrics.FRUIT);

            // .. If we collided with something, try to find another valid position after skipping this frame to avoid hiccups
            if (hit.Length > 0)
            {
                yield return new WaitForEndOfFrame();
            }
            else
            {
                // .. We found a valid spot, create the food here
                Quaternion rot = Quaternion.Euler(-90f, 0f, 0f);
                GameObject fruit = ObjectsPoolManager.Instance.GetPooledObject(pickups[0].prefab, newPosition, rot);
                fruit.transform.SetParent(pickupsParent);

                // .. Play fruit spawn sfx
                if (playEatFruit)
                    SoundManager.Instance.PlaySoundEffect(SoundEffectName.EAT_FRUIT, 1f);
                else
                    SoundManager.Instance.PlaySoundEffect(SoundEffectName.COLLECT_FRUIT, 1f);
                yield break;
            }
        }
    }

    /// <summary>
    /// generate a new bomb object at a random and valid position
    /// </summary>
    private IEnumerator CreateBombRandom()
    {
        while (true)
        {
            int ranX = Random.Range((int)WorldBorders.LeftBorder, (int)WorldBorders.RightBorder);
            int ranY = Random.Range((int)WorldBorders.BottomBorder, (int)WorldBorders.TopBorder);
            Vector3 newPosition = new Vector3(ranX, ranY, 5.72f);

            Collider[] hit = Physics.OverlapSphere(newPosition, (int)Metrics.FRUIT);

            // .. If we collided with something, try to find another valid position after skipping this frame to avoid hiccups
            if (hit.Length > 0)
            {
                yield return new WaitForEndOfFrame();
            }
            else
            {
                // .. We found a valid spot, create the bomb here
                GameObject bomb = ObjectsPoolManager.Instance.GetPooledObject(pickups[1].prefab, newPosition, Quaternion.identity);
                bomb.transform.SetParent(pickupsParent);

                // .. Play bomb spawn sfx
                SoundManager.Instance.PlaySoundEffect(SoundEffectName.SPAWN_BOMB, 1f);

                bombAmount--;
                if (bombAmount != 0)        // .. Create another bomb if specified amount is more than 1
                {               
                    StartCoroutine(CreateBombRandom());
                }
                yield break;
            }
        }
    }

    /// <summary>
    /// Destroy all pickups on gameover
    /// </summary>
    private void OnGameOver()
    {
        foreach(Transform pickup in pickupsParent)
        {
            ObjectsPoolManager.Instance.DestroyGameObjectWithPooledChildren(pickup.gameObject);
        }
    }
}

[System.Serializable]
class Pickups
{
    public PickupType type = PickupType.Fruit;
    public GameObject prefab;
}