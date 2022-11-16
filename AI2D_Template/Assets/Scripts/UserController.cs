/*
UserController

Manages a system of features 
and states that allows the 
player to control an object, 
such as a character.

Copyright John M. Quick
*/

using UnityEngine;

public class UserController : MonoBehaviour {

    //components
    public UserMove move;
    public UserJump jump;

    /*
    With the exception of movement, the 
    object position is stored in pixels, 
    as an integer. Meanwhile, movement is 
    handled in world units, using float. 
    When the object's world position is 
    cast from float to int, the remainder 
    is stored. Later, when setting the 
    on-screen position of the object in 
    world space, this remainder is factored 
    back into the calculation. This helps 
    to create smooth movement despite the 
	differences between the design of the 
	game world and the game engine.
    */

    //remainder from position calculations
    public Vector2 remainderPos;

    //init
    public void Init() {

        //init variables
        remainderPos = Vector2.zero;

        //init components
        move.Init();
        jump.Init();
    }

    //movement
    //calculate one step on the x axis
    public int[] StepX(int[] thePos) {

        //Debug.Log("[UserController] ===== START X STEP =====");

        //store position
        int[] movePos = new int[] { thePos[0], thePos[1] };

        //Debug.Log("[UserController] Pixel Pos Before X Step: (" + movePos[0] + ", " + movePos[1] + ")");

        //check input
        move.CheckInput();

        //update position based on movement
        move.Move(movePos, remainderPos);

        //Debug.Log("[UserController] Pixel Pos After X Step: (" + movePos[0] + ", " + movePos[1] + ")");

        //Debug.Log("[UserController] ===== END X STEP =====");

        //return
        return movePos;
    }

    //calculate one step on the y axis
    public int[] StepY(int[] thePos) {

        //Debug.Log("[User] ===== START Y STEP =====");

        //check input
        jump.CheckInput();

        //store position
        int[] jumpPos = new int[] { thePos[0], thePos[1] };

        //Debug.Log("[UserController] Pixel Pos Before Y Step: (" + jumpPos[0] + ", " + jumpPos[1] + ")");

        //update position based on movement
        jump.Jump(jumpPos, remainderPos);

        //Debug.Log("[UserController] Pixel Pos After Y Step: (" + jumpPos[0] + ", " + jumpPos[1] + ")");

        //Debug.Log("[UserController] ===== END Y STEP =====");

        //return
        return jumpPos;
    }

} //end class