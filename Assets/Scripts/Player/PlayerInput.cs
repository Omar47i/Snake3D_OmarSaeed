using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PlayerInput : MonoBehaviour
{
    private PlayerController playerController;

    // .. Cach gameplay variables
    private int horizontal = 0;
    private int vertical = 0;
    private Vector2 touchOrigin = -Vector2.one;   //Used to store location of screen touch origin for mobile controls.

    void Start()
    {
        playerController = GetComponent<PlayerController>();
        
        // .. Listen to the game over and reset events
        GameManager.Instance.GameOverEvent.AddListener(OnGameOver);
        GameManager.Instance.ResetEvent.AddListener(OnReset);
    }

    private void Update()
    {
        horizontal = 0;     //Used to store the horizontal move direction.
        vertical = 0;       //Used to store the vertical move direction.

#if UNITY_EDITOR || UNITY_STANDALONE //Check if we are running either in the Unity editor or in a standalone build.
        GetKeyboardInput();
#else                                //Check if we are running on iOS, Android, Windows Phone 8 or Unity iPhone
        GetTouchInput();
#endif
        SetMovement();            // Tell the PlayerController.cs to move if movement exists
    }

    /// <summary>
    /// Issue a movement command in a specific direction to the PlayerController.cs
    /// </summary>
    private void SetMovement()
    {
        if (vertical != 0)
        {
            playerController.SetInputDirection( (vertical == 1) ? PlayerDirection.Up : PlayerDirection.Down );
        }
        else if (horizontal != 0)
        {
            playerController.SetInputDirection( (horizontal == 1) ? PlayerDirection.Right : PlayerDirection.Left );
        }
    }

    private void GetTouchInput()
    {
        //Check if Input has registered more than zero touches
        if (Input.touchCount > 0)
        {
            //Store the first touch detected.
            Touch myTouch = Input.touches[0];

            //Check if the phase of that touch equals Began
            if (myTouch.phase == TouchPhase.Began)
            {
                //If so, set touchOrigin to the position of that touch
                touchOrigin = myTouch.position;
            }

            //If the touch phase is not Began, and instead is equal to Ended and the x of touchOrigin is greater or equal to zero:
            else if (myTouch.phase == TouchPhase.Ended && touchOrigin.x >= 0)
            {
                //Set touchEnd to equal the position of this touch
                Vector2 touchEnd = myTouch.position;

                //Calculate the difference between the beginning and end of the touch on the x axis.
                float x = touchEnd.x - touchOrigin.x;

                //Calculate the difference between the beginning and end of the touch on the y axis.
                float y = touchEnd.y - touchOrigin.y;

                //Set touchOrigin.x to -1 so that our else if statement will evaluate false and not repeat immediately.
                touchOrigin.x = -1;

                //Check if the difference along the x axis is greater than the difference along the y axis.
                if (Mathf.Abs(x) > Mathf.Abs(y))
                    //If x is greater than zero, set horizontal to 1, otherwise set it to -1
                    horizontal = x > 0 ? 1 : -1;
                else
                    //If y is greater than zero, set horizontal to 1, otherwise set it to -1
                    vertical = y > 0 ? 1 : -1;
            }
        }
    }

    private void GetKeyboardInput()
    {
        //Get axis input from the input manager
        horizontal = GetAxisRaw(Axis.Horizontal);
        vertical = GetAxisRaw(Axis.Vertical);

        //Check if moving horizontally, if so set vertical to zero.
        if (horizontal != 0)
        {
            vertical = 0;
        }
    }

    private int GetAxisRaw(Axis axis)
    {
        if (axis == Axis.Horizontal)
        {
            bool left = Input.GetKeyDown(KeyCode.LeftArrow);
            bool right = Input.GetKeyDown(KeyCode.RightArrow);

            if (left)
                return -1;

            if (right)
                return 1;

            return 0;
        }
        else if (axis == Axis.Vertical)
        {
            bool up = Input.GetKeyDown(KeyCode.UpArrow);
            bool down = Input.GetKeyDown(KeyCode.DownArrow);

            if (down)
                return -1;

            if (up)
                return 1;

            return 0;
        }

        return 0;
    }

    /// <summary>
    /// Disable input and movement when player die
    /// </summary>
    private void OnGameOver()
    {
        enabled = false;
        //playerController.enabled = false;
    }

    private void OnReset()
    {
        enabled = true;
        //playerController.enabled = true;
    }
}

public enum Axis
{
    Vertical,
    Horizontal
};