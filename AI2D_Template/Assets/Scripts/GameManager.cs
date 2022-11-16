/*
GameManager

Manages the main game loop.

Copyright John M. Quick
*/

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

    //components
    public UserController user;
    public CollisionManager collision;
    public List<GameObject> platforms;

    //whether game has started
    private bool _isGameStarted;

    //init
    void Awake() {

        //init components
        user.Init();
        platforms = new List<GameObject>();

        //find all of the platforms placed in the scene
        platforms.AddRange(GameObject.FindGameObjectsWithTag("Platform"));

        //whether game has started
        _isGameStarted = false;
    }

    //update
    void Update() {

        //check for start button to be pressed
        if (Input.GetKeyUp(KeyCode.Space) && _isGameStarted == false) {

            //toggle flag
            _isGameStarted = true;
        }

        //if game has started
        if (_isGameStarted == true) {

            //Debug.Log("[GameController] ===== START LOOP =====");

            //USER MOVEMENT

            /*
            The position is retrieved in world space,  
            which has a center origin. The world position 
            is converted into pixels at a ratio of 1 
            world unit = 100 pixels. The remainder from 
            the float to int conversion is stored.
            */

            //retrieve user's world position as float
            Vector2 userWorldPos = user.gameObject.transform.position;

            //convert world position to pixels
            float userPixelX = userWorldPos.x * Constants.PIXELS_TO_UNITS;
            float userPixelY = userWorldPos.y * Constants.PIXELS_TO_UNITS;

            //cast pixel position to int
            //floors value to nearest pixel
            int[] userPixelPos = new int[] {
            Mathf.RoundToInt(userPixelX),
            Mathf.RoundToInt(userPixelY)
            };

            //store remainder from conversion
            user.remainderPos.x = Mathf.Abs(userPixelX - userPixelPos[0]);
            user.remainderPos.y = Mathf.Abs(userPixelY - userPixelPos[1]);

            //calculate a step on the x axis
            int[] userStepXPos = user.StepX(userPixelPos);

            //check collisions
            userPixelPos = CheckCollisionX(userStepXPos);

            //calculate a step on the y axis
            int[] userStepYPos = user.StepY(userPixelPos);

            //check collisions
            userPixelPos = CheckCollisionY(user.gameObject, userPixelPos, userStepYPos, platforms, 0);

            /*
            The ultimate position calculation is converted 
            back into world units. The remainder from any 
            previous conversions is added back to the 
            position. With the calculations complete, the 
            on-screen position is updated.
            */

            //convert pixels to world position
            //user
            userWorldPos.x =
                (userPixelPos[0] + user.remainderPos.x * user.move.dir) /
                Constants.PIXELS_TO_UNITS;
            userWorldPos.y =
                (userPixelPos[1] + user.remainderPos.y * user.jump.dir) /
                Constants.PIXELS_TO_UNITS;

            //update user position on screen
            user.gameObject.transform.position = userWorldPos;

            //Debug.Log("[GameController] ===== END LOOP =====");
        }
    }

    //reset game
    public void ResetGame() {

        //retrieve current scene name
        string sceneName = SceneManager.GetActiveScene().name;

        //load scene
        StateManager.Instance.SwitchSceneTo(sceneName);
    }

    //check x-axis collisions
    private int[] CheckCollisionX(int[] thePos) {

        //store position
        int[] pos = new int[] { thePos[0], thePos[1] };

        //boundary
        pos = collision.CheckBounds(pos, true, false);

        //if boundary collision found
        if (pos[0] != thePos[0]) {

            //discard remainder
            user.remainderPos.x = 0;
        }

        //return
        return pos;
    }

    //check y-axis collisions
    private int[] CheckCollisionY(GameObject theObj, int[] theUserPosInitial, int[] theUserPosFinal, List<GameObject> theObjects, int theScrollDist) {

        //store position
        int[] pos = new int[] { theUserPosFinal[0], theUserPosFinal[1] };

        //store y position that the object will fall to
        int fallY = pos[1];

        //store platforms
        List<GameObject> platforms = new List<GameObject>();

        //iterate through objects
        for (int i = 0; i < theObjects.Count; i++) {

            //retrieve tag
            string tag = theObjects[i].tag;

            //if platform
            if (tag == "Platform") {

                //add to collection
                platforms.Add(theObjects[i]);
            }
        }

        //retrieve object tag
        string objTag = theObj.tag;

        //USER MOVEMENT
        if (objTag == "Player") {

            //if falling
            if (user.jump.CurrentState == UserJump.JumpState.Fall) {

                //check fall to platform
                fallY = collision.CheckFallToPlatform(theUserPosInitial, theUserPosFinal, platforms, theScrollDist);

                //add optional buffer to allow for extended fall off screen
                int offScreenYBuffer = Constants.TILE_SIZE * 30;

                //calculate off-screen fall position
                int offScreenY = -Constants.SCREEN_H / 2 - Constants.TILE_SIZE / 2 - offScreenYBuffer;

                //if a collision was found
                if (fallY != pos[1]) {

                    //update position
                    pos[1] = fallY;

                    //update state
                    user.jump.JumpGround();
                }

                //if fell off screen
                else if (fallY < offScreenY) {

                    //reset game
                    ResetGame();
                }
            }

            //if grounded
            else if (user.jump.CurrentState == UserJump.JumpState.Ground) {

                //whether object should fall based on movement
                bool isFalling = false;

                //check fall from platform
                isFalling = collision.CheckFallFromPlatform(theUserPosInitial, theUserPosFinal, platforms, theScrollDist);

                //if object is falling
                if (isFalling == true) {

                    //update state
                    user.jump.JumpFall();
                }
            }
        }       

        //return
        return pos;
    }

    public bool IsGameStarted()
	{
        return _isGameStarted;
	}
} //end class