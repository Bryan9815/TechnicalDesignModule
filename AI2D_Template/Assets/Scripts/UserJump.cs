/*
UserJump

Implements user input that 
allows the player to jump.

Attach to an object that 
the player can control, 
such as a character.

Copyright John M. Quick
*/

using UnityEngine;

public class UserJump : MonoBehaviour {

    /*
    When jumping, the duration is the 
    amount of time, in seconds, that the 
    object takes to complete a full rise 
    and fall cycle. It defines the 
    maximum possible jump that the object 
    can make.

    For example, a character may 
    jump from a standing position. 
    The character rises and falls to 
    land back in the original position, 
    completing a full jump cycle. The 
    duration defines how long the cycle 
    takes to complete.

    During actual play, cycles 
    may be interrupted and never fully 
    completed. For instance, a character 
    may jump up to a higher platform and
    land on it, thereby never falling 
    back down to the height of the 
    original jump position. Or, the 
	character may fall from a platform, 
	in which case the rise portion of 
	the jump is skipped. 
    */

    //the jump duration, in seconds
    public float duration;

    //maximum jump extend duration, in seconds
    public float maxDurationExtend;

    //limits on how fast object moves while jumping
    //in pixels per second
    public float minSpeed, maxSpeed;

    //the object's current jump speed
    private float _currentSpeed;

    //the direction of jumping
    //0 = ground, 1 = rise; -1 = fall
    public int dir;

    //time at which the jump began
    private float _startTime;

    //time at which the jump extension began
    private float _startTimeExtend;

    //current state
    private JumpState _currentState;

    //possible states
    public enum JumpState {
        Ground = 0,
        Rise,
        Fall,
    }

    //whether listening for input
    private bool _isListening;

    //init
    public void Init() {

        //update state
        JumpFall();
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
        The W (Up Arrow) key is used for jumping.
        */

        //if listening
        if (_isListening == true) {

            //jump
            //if key is pressed from static state
            if (
                _currentState == JumpState.Ground &&
                (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                ) {

                //rise
                JumpRise();
            }

            //if key is released from rise state
            else if (
                _currentState == JumpState.Rise &&
                (Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.UpArrow))
                ) {

                //stop listening
                _isListening = false;
            }

            //if key is held from rise state
            else if (
                _currentState == JumpState.Rise &&
                (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
                ) {

                /*
                If the user continues to hold the 
                jump key, the duration of the jump 
                is extended up to a certain time 
                limit. 
                */

                //calculate cumulative hold duration
                float cumulativeHold = Time.time - _startTimeExtend;

                //if cumulative hold time exceeds limit
                if (cumulativeHold >= maxDurationExtend) {

                    //stop listening
                    _isListening = false;
                }

                //otherwise, extend jump
                else {

                    //update start time
                    _startTime = Time.time;
                }
            }
        }
    }

    //calculate jump position based on movement
    public void Jump(int[] thePos, Vector2 theRemainder) {

        /*
        To execute the jump, we calculate how much of the 
        total duration has been completed thus far. Prior 
        to the halfway point, the object is rising. After 
        the halfway point, the object is falling. 

        Since the rise and fall states only occupy half of 
        the total duration each, the percentage complete is 
        calculated based on half of the duration. This 
        ensures that the object reaches the speed limits  
        within the time allotted to each state. 
        */

        //if jumping
        if (_currentState != JumpState.Ground) {

            //calculate cumulative duration
            float cumulativeDuration = Time.time - _startTime;

            //calculate perentage of duration completed
            float pctDuration =
                Mathf.Clamp01(cumulativeDuration / (duration / 2));

            //y-axis movement
            //check state
            switch (_currentState) {

                //rise
                case JumpState.Rise:

                    //rise
                    Rise(thePos, theRemainder, pctDuration);

                    break;

                //fall
                case JumpState.Fall:

                    //fall
                    Fall(thePos, theRemainder, pctDuration);

                    break;

                //default
                default:
                    Debug.Log("[UserJump] Jump state not recognized");
                    break;
            }
        }
    }

    //calculate jump position based on movement
    //rising half of jump
    private void Rise(int[] thePos, Vector2 theRemainder, float thePctComplete) {

        /*
        While rising, the object's speed decelerates from 
        the maximum to the minimum while the y position is 
        increased. While falling, the object's speed 
        accelerates from the minimum to the maximum while 
        the y position is decreased. This acceleration 
        creates a curved path for the jump. Without 
        acceleration, it would appear triangular. The fall 
        state will continue until a collision changes it.

        Time.deltaTime is used in calculations to achieve 
        frame-rate independence.
        */

        //calculate speed based on acceleration
        _currentSpeed = maxSpeed - thePctComplete * (maxSpeed - minSpeed);

        //calculate change in movement
        float fDeltaY = _currentSpeed * dir * Time.deltaTime;

        //cast to int
        int deltaY = Mathf.RoundToInt(fDeltaY);

        //update position
        thePos[1] += deltaY;

        //update remainder
        theRemainder.y += Mathf.Abs(fDeltaY - deltaY);

        //once rise state is complete
        if (thePctComplete >= 1.0f) {

            //fall
            JumpFall();
        }
    }

    //falling half of jump
    private void Fall(int[] thePos, Vector2 theRemainder, float thePctComplete) {

        //calculate speed based on acceleration
        _currentSpeed = minSpeed + thePctComplete * (maxSpeed - minSpeed);

        //calculate change in movement
        float fDeltaY = _currentSpeed * dir * Time.deltaTime;

        //cast to int
        int deltaY = Mathf.RoundToInt(fDeltaY);

        //update position
        thePos[1] += deltaY;

        //update remainder
        theRemainder.y += Mathf.Abs(fDeltaY - deltaY);
    }

    //update jump state
    //ground
    public void JumpGround() {

        //if not already in state
        if (_currentState != JumpState.Ground) {

            //update state
            _currentState = JumpState.Ground;

            //update direction
            dir = 0;

            //start listening
            _isListening = true;
        }
    }

    //rise
    public void JumpRise() {

        //if not already in state
        if (_currentState != JumpState.Rise) {

            //update state
            _currentState = JumpState.Rise;

            //update direction
            dir = 1;

            //update start time
            _startTime = Time.time;

            //update hold time
            _startTimeExtend = Time.time;
        }
    }

    //fall
    public void JumpFall() {

        //if not already in state
        if (_currentState != JumpState.Fall) {

            //stop listening
            _isListening = false;

            //update state
            _currentState = JumpState.Fall;

            //update direction
            dir = -1;

            //update start time
            _startTime = Time.time;
        }
    }

    //accessors
    public JumpState CurrentState {
        get {
            return _currentState;
        }
    }

} //end class