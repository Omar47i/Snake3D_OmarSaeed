// This script is responsible for all player movement
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public PlayerDirection direction;       // the direction of the player (Up, down, left, or right)

    [HideInInspector]
    public float stepLength = .2f;          // move by this value each interval

    [HideInInspector]
    public float movementFrequency = .1f;   // as we don't want to move each frame, move by this duration

    [SerializeField]
    private GameObject nodePrefab;          // the snake node that will be created when eating a fruit

    private float defaultMovementFreq = .1f;// Reset movement frequency to this value when restarting the game
    private List<Vector3> deltaPosition;    // store the next position displacement based on player direction
    private List<Rigidbody> nodes;          // keep track of snack nodes to control movement
    private Vector3 fruitNodePosition;      // store the node position that is generate by eating a fruit

    // .. Cash frequently used gameplay variables
    private Rigidbody headRB;             
    private Transform tr;

    private float counter = 0f;
    private bool move = false;
    private bool createNodeAtTail = false;     // this flag is triggered when eating a fruit to create a new node at the tail of the snake

    void Awake()
    {
        tr = transform;        // Cach transform component for efficient access

        InitSnakeNodes();      // Get references to snake nodes

        InitPlayer();          // reform snake parts based on the assigned direction

        deltaPosition = new List<Vector3>()
        {
            new Vector3(-stepLength, 0f),  // -dx .. Left
            new Vector3(0f, stepLength),   // dy  .. Up
            new Vector3(stepLength, 0f),   // dx  .. Right
            new Vector3(0f, -stepLength)   // -dy .. Down
        };

        // .. Listen to the fruit eat event
        GameManager.Instance.FruitAteEvent.AddListener(OnFruitAte);

        // .. Listen to the reset event
        GameManager.Instance.ResetEvent.AddListener(OnReset);
    }

    void Update()
    {
        if (GameManager.Instance.gameState == GameState.Playing)
        {
            counter += Time.deltaTime;

            if (counter >= movementFrequency)
            {
                counter = 0f;
                move = true;
            }
        }
    }

    void FixedUpdate()
    {
        if (move && GameManager.Instance.gameState == GameState.Playing)
        {
            move = false;

            Move();
        }
    }

    /// <summary>
    /// Move the snake with a direction
    /// </summary>
    private void Move()
    {
        // .. Get the discplacement based on player direction
        Vector3 dPosition = deltaPosition[(int)direction];

        Vector3 parentPos = headRB.position;
        Vector3 prevPos;
        // .. Move the head node first then move the rest of the snake
        headRB.position = headRB.position + dPosition;
        
        // .. Move all snake nodes
        for (int i = 1; i < nodes.Count; i++)
        {
            // .. Assign the position of each node to the position of the node it follows!
            prevPos = nodes[i].position;

            nodes[i].position = parentPos;

            parentPos = prevPos;
        }

        // .. After moving all the snake nodes, check if we should create an extra node because we eat a fruit!
        if (createNodeAtTail)
        {
            createNodeAtTail = false;

            GameObject newNode = ObjectsPoolManager.Instance.GetPooledObject(nodePrefab, fruitNodePosition, Quaternion.identity);
            newNode.transform.SetParent(transform, true);
            nodes.Add(newNode.GetComponent<Rigidbody>());
        }
    }

    /// <summary>
    /// Move the snake immediatly if input is detected
    /// </summary>
    private void ForceMove()
    {
        counter = 0f;

        move = false;

        Move();
    }

    /// <summary>
    /// Add the snake head, node, and tail to a queue
    /// </summary>
    private void InitSnakeNodes()
    {
        nodes = new List<Rigidbody>();
        
        nodes.Add(tr.GetChild(0).GetComponent<Rigidbody>());    // Head
        nodes.Add(tr.GetChild(1).GetComponent<Rigidbody>());    // Body node
        nodes.Add(tr.GetChild(2).GetComponent<Rigidbody>());    // Tail

        headRB = nodes[0];
    }

    private void SetDirectionRandom()
    {
        int ranDirection = Random.Range(0, (int)PlayerDirection.Count);
        direction = (PlayerDirection)ranDirection;
    }

    public void SetInputDirection(PlayerDirection dir)
    {
        // .. Prevent movement in the opposite direction
        if (dir == PlayerDirection.Up && direction == PlayerDirection.Down ||
            dir == PlayerDirection.Down && direction == PlayerDirection.Up ||
            dir == PlayerDirection.Right && direction == PlayerDirection.Left ||
            dir == PlayerDirection.Left && direction == PlayerDirection.Right)
        {
            return;
        }

        direction = dir;

        ForceMove();     // Move the snake immediatly without waiting for the next movement frequency threshold
    }

    // Called when the snake eat a fruit
    private void OnFruitAte(int scoreAddition, Vector3 fruitPosition)
    {
        // .. Specify the new fruit position to be the last node position
        fruitNodePosition = nodes[nodes.Count - 1].position;

        createNodeAtTail = true;
    }

    /// <summary>
    /// Rearrange snake parts based on a random direction
    /// </summary>
    private void InitPlayer()
    {
        SetDirectionRandom();  // Set a random starting direction

        switch (direction)
        {
            case PlayerDirection.Right:   // shift the middle node and tail of the snake to the left of the snake's head
                nodes[1].position = nodes[0].position - new Vector3(Metrics.SNACK_NODE, 0f, 0f);
                nodes[2].position = nodes[0].position - new Vector3(Metrics.SNACK_NODE * 2, 0f, 0f);
                break;

            case PlayerDirection.Left:    // shift the middle node and tail of the snake to the right of the snake's head
                nodes[1].position = nodes[0].position + new Vector3(Metrics.SNACK_NODE, 0f, 0f);
                nodes[2].position = nodes[0].position + new Vector3(Metrics.SNACK_NODE*2, 0f, 0f);
                break;

            case PlayerDirection.Up:     // shift to down
                nodes[1].position = nodes[0].position - new Vector3(0f, Metrics.SNACK_NODE, 0f);
                nodes[2].position = nodes[0].position - new Vector3(0f, Metrics.SNACK_NODE*2f, 0f);
                break;

            case PlayerDirection.Down:     // shift to up
                nodes[1].position = nodes[0].position + new Vector3(0f, Metrics.SNACK_NODE, 0f);
                nodes[2].position = nodes[0].position + new Vector3(0f, Metrics.SNACK_NODE * 2f, 0f);
                break;
        }
    }

    /// <summary>
    /// Reset player to the starting state (3 nodes and at the middle of screen)
    /// </summary>
    private void OnReset()
    {
        createNodeAtTail = false;

        // Reset movement frequency
        movementFrequency = defaultMovementFreq;

        // .. Clear all extra nodes and leave only three nodes (head, body node, and tail)
        while (nodes.Count > 3)
        {
            Rigidbody node = nodes[3];

            nodes.Remove(node);       // Remove it from the list

            // .. Destroy the extra snake node
            ObjectsPoolManager.Instance.DestroyGameObjectWithPooledChildren(node.gameObject);
        }

        // .. Reposition snake's head a the center of screen
        nodes[0].transform.position = new Vector3(0f, 0f, 5.72f);

        // .. Rearrange snake nodes based on the new assigned random direction
        InitPlayer();
    }

    /// <summary>
    /// Called by the DifficultyPregression.cs to increase difficulty
    /// </summary>
    /// <param name="amount"></param>
    public void DecreaseMovementFrequency(float amount, float minMovementFreq)
    {
        float newFreq = movementFrequency - amount;

        movementFrequency = Mathf.Max(newFreq, minMovementFreq);
    }
}
