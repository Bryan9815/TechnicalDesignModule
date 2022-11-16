/*
AIMove

Implements user input that 
allows the player to move 
an object.

Attach to an object that 
the player can control, 
such as a character.

Copyright John M. Quick
*/

using UnityEngine;

public class AIMove : MonoBehaviour
{

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
    public enum MoveState
    {
        Stop = 0,
        Left,
        Right
    }

    //init
    public void Init()
    {

        //update state
        MoveRight();
    }

    //movement 
    //move object in direction at speed
    public void Move(int[] thePos, Vector2 theRemainder)
    {

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
        if (_currentState != MoveState.Stop)
        {

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
    public void MoveStop()
    {

        //if not already in state
        if (_currentState != MoveState.Stop)
        {

            //update state
            _currentState = MoveState.Stop;

            //update direction
            dir = 0;
        }
    }

    //left
    public void MoveLeft()
    {

        //if not already in state
        if (_currentState != MoveState.Left)
        {
            //update state
            _currentState = MoveState.Left;

            //update direction
            dir = -1;
        }
    }

    //right
    public void MoveRight()
    {

        //if not already in state
        if (_currentState != MoveState.Right)
        {
            //update input
            _currentState = MoveState.Right;

            //update direction
            dir = 1;
        }
    }

} //end class
