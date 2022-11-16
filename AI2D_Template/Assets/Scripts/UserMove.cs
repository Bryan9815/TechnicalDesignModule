/*
UserMove

Implements user input that 
allows the player to move 
an object.

Attach to an object that 
the player can control, 
such as a character.

Copyright John M. Quick
*/

using UnityEngine;

public class UserMove : MonoBehaviour {

    /* 
    Movement is allowed along the x axis 
    at a specified speed, in pixels per 
    second, and in a direction determined 
    by user input.
    */

    //movement
    //the speed at which to move the object
    //in pixels per second
    public float speed;

    //the direction of movement
    //0 = stop, -1 = left; 1 = right
    public int dir;

    //current state
    private MoveState _currentState;

    //possible states
    public enum MoveState {
        Stop = 0,
        Left,
        Right
    }

    //whether listening for input
    private bool _isListening;

    //init
    public void Init() {

        //update state
        MoveRight();
    }

    //check input
    public void CheckInput() {

        /*
        Unity Input Notes 
        GetKey() returns true while key is held.
        GetKeyDown() returns true only when the key is initially pressed.
        GetKeyUp() returns true only when the key is initially released.
        */

        /*
        The A (Left Arrow) key is used for leftward movement, 
        The D (Right Arrow) key is used for rightward movement.
        */

        //if previous input ended
        if (
            (_currentState == MoveState.Left && (Input.GetKeyUp(KeyCode.A) || Input.GetKeyUp(KeyCode.LeftArrow))) ||
            (_currentState == MoveState.Right && (Input.GetKeyUp(KeyCode.D) || Input.GetKeyUp(KeyCode.RightArrow)))
            ) {

            //update state
            MoveStop();
        }

        //no ongoing input
        else if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.LeftArrow) && !Input.GetKey(KeyCode.RightArrow)) {

            //update state
            MoveStop();
        }

        //if key is held and listening for input
        //move left
        if ((Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) && _isListening == true) {

            //update state
            MoveLeft();
        }

        //move right
        else if ((Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)) && _isListening == true) {

            //update state
            MoveRight();
        }
    }

    //movement 
    //move object in direction at speed
    public void Move(int[] thePos, Vector2 theRemainder) {

        /*
        The movement is equal to the speed  
        times the direction. Time.deltaTime is
        used to ensure frame-rate independence. 
        Since the movement is stored as an int to 
		match the pixel position of the object, 
        the remainder of the cast from float to int  
        is stored. That way, the ultimate world 
		position of the object can be smoothed by 
		accounting for the remainder.
        */

        //if moving
        if (_currentState != MoveState.Stop) {

            //calculate change in movement
            float fDeltaX = speed * dir * Time.deltaTime;

            //cast to int
            int deltaX = Mathf.RoundToInt(fDeltaX);

            //update position
            thePos[0] += deltaX;

            //update remainder
            theRemainder.x += Mathf.Abs(fDeltaX - deltaX);
        }
    }

    //update move state
    //stop
    public void MoveStop() {

        //if not already in state
        if (_currentState != MoveState.Stop) {

            //update state
            _currentState = MoveState.Stop;

            //update direction
            dir = 0;

            //start listening
            _isListening = true;
        }
    }

    //left
    public void MoveLeft() {

        //if not already in state
        if (_currentState != MoveState.Left) {

            //stop listening
            _isListening = false;

            //update state
            _currentState = MoveState.Left;

            //update direction
            dir = -1;
        }
    }

    //right
    public void MoveRight() {

        //if not already in state
        if (_currentState != MoveState.Right) {

            //stop listening
            _isListening = false;

            //update input
            _currentState = MoveState.Right;

            //update direction
            dir = 1;
        }
    }

} //end class
