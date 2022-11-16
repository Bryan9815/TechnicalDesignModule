/*
AIController

PLACE ALL CODE NECESSARY TO RUN
YOUR AI IN THIS SCRIPT.

IF NECESSARY, YOU MAY CREATE
ADDITIONAL SCRIPTS TO IMPLEMENT
YOUR AI. PUT ALL SCRIPTS INSIDE
THE AI FOLDER.

DO NOT MODIFY ANY OTHER SCRIPTS.
*/

using UnityEngine;
using System.Collections.Generic;

public class AIController : MonoBehaviour {

	public Transform Waypoint;
	private List<Transform> waypoints = new();
	private int currWaypoint;
	private int targetWaypoint;
	public Vector2 remainderPos;
	public GameManager gameManager;

	private enum AI_State
	{
		GOING_UP,
		GOING_DOWN
	};
	private AI_State state;
	public AIMove aiMove;
	public AIJump aiJump;
	private Vector2 targetDirection;

	void Start()
	{
		// Get all waypoints into a list
		foreach (Transform child in Waypoint)
		{
			waypoints.Add(child);
		}

		#region Find the waypoint that's closest to the AI's starting position
		Vector3 dist, closestDist;
		int closestWaypoint = 0;
		dist = closestDist = waypoints[0].position - transform.position;

		if (closestDist.x < 0)
			closestDist.x *= -1;
		if (closestDist.y < 0)
			closestDist.y *= -1;

		for (int idx = 0; idx < waypoints.Count; idx++)
		{
			dist = waypoints[idx].position - transform.position;

			if (dist.x < 0)
				dist.x *= -1;
			if (dist.y < 0)
				dist.y *= -1;

			//Debug.Log("Index " + idx + ", waypoint name = " + waypoints[idx].name);
			//Debug.Log("dist = (" + dist + ")");
			//Debug.Log("closestDist = (" + closestDist + ")");

			if (dist.y < 1 && dist.x < closestDist.x)
			{
				closestWaypoint = idx;
				closestDist = dist;
				targetWaypoint = closestWaypoint;
			}
		}
		//Debug.Log("Closest waypoint: wp (" + closestWaypoint + "), name = " + waypoints[closestWaypoint].name);
		#endregion
		// if the closest waypoint is in bottom half, state = GOING_DOWN
		if (closestWaypoint + 1 < waypoints.Count / 2)
		{
			state = AI_State.GOING_DOWN;
		}
		else // if the closest waypoint is in upper half, state = GOING_UP
		{
			state = AI_State.GOING_UP;
		}

		aiMove.Init();
		aiJump.Init();

		targetDirection.x = (waypoints[targetWaypoint].position.x > transform.position.x) ? 1 : -1;
		targetDirection.y = (waypoints[targetWaypoint].position.y > transform.position.y) ? 1 : -1;
	}

	void Update()
	{
		if (gameManager.IsGameStarted())
		{
			#region change direction based on target waypoint
			Vector2 previousTargetDirection = targetDirection;
			targetDirection.x = (waypoints[targetWaypoint].position.x > transform.position.x) ? 1 : -1;
			targetDirection.y = (waypoints[targetWaypoint].position.y > transform.position.y) ? 1 : -1;

			// If AI has passed the previous waypoint
			if (targetDirection != previousTargetDirection)
			{
				currWaypoint = targetWaypoint;
				if (state == AI_State.GOING_DOWN)
				{
					if (targetWaypoint == 0)
					{
						state = AI_State.GOING_UP;
						targetWaypoint++;
					}
					else
						targetWaypoint--;

					targetDirection.x = (waypoints[targetWaypoint].position.x > transform.position.x) ? 1 : -1;
					targetDirection.y = (waypoints[targetWaypoint].position.y > transform.position.y) ? 1 : -1;
				}
				else if (state == AI_State.GOING_UP)
				{
					if (targetWaypoint == waypoints.Count)
					{
						state = AI_State.GOING_DOWN;
						targetWaypoint--;
					}
					else
						targetWaypoint++;

					targetDirection.x = (waypoints[targetWaypoint].position.x > transform.position.x) ? 1 : -1;
					targetDirection.y = (waypoints[targetWaypoint].position.y > transform.position.y) ? 1 : -1;
				}
			}
			#endregion

			#region Calculate movement

			/*
			The position is retrieved in world space,  
			which has a center origin. The world position 
			is converted into pixels at a ratio of 1 
			world unit = 100 pixels. The remainder from 
			the float to int conversion is stored.
			*/

			//retrieve user's world position as float
			Vector2 aiWorldPos = transform.position;

			//convert world position to pixels
			float aiPixelX = transform.position.x * Constants.PIXELS_TO_UNITS;
			float aiPixelY = transform.position.y * Constants.PIXELS_TO_UNITS;

			//cast pixel position to int
			//floors value to nearest pixel
			int[] aiPixelPos = new int[] {
			Mathf.RoundToInt(aiPixelX),
			Mathf.RoundToInt(aiPixelY)
			};

			//store remainder from conversion
			remainderPos.x = Mathf.Abs(aiPixelX - aiPixelPos[0]);
			remainderPos.y = Mathf.Abs(aiPixelY - aiPixelPos[1]);

			//calculate a step on the x axis
			int[] userStepXPos = StepX(aiPixelPos);

			//check collisions
			aiPixelPos = CheckCollisionX(userStepXPos);

			//calculate a step on the y axis
			int[] userStepYPos = StepY(aiPixelPos);

			//check collisions
			aiPixelPos = CheckCollisionY(gameObject, aiPixelPos, userStepYPos, gameManager.platforms, 0);

			/*
			The ultimate position calculation is converted 
			back into world units. The remainder from any 
			previous conversions is added back to the 
			position. With the calculations complete, the 
			on-screen position is updated.
			*/

			//convert pixels to world position
			//user
			aiWorldPos.x =
				(aiPixelPos[0] + remainderPos.x * aiMove.dir) /
				Constants.PIXELS_TO_UNITS;
			aiWorldPos.y =
				(aiPixelPos[1] + remainderPos.y * aiJump.dir) /
				Constants.PIXELS_TO_UNITS;

			#endregion

			//update AI position on screen
			transform.position = aiWorldPos;
		}
	}

	//movement
	//calculate one step on the x axis
	public int[] StepX(int[] thePos)
	{

		//Debug.Log("[UserController] ===== START X STEP =====");

		//store position
		int[] movePos = new int[] { thePos[0], thePos[1] };

		//Debug.Log("[UserController] Pixel Pos Before X Step: (" + movePos[0] + ", " + movePos[1] + ")");

		//check input
		//move.CheckInput();

		//update position based on movement
		aiMove.Move(movePos, remainderPos);

		//Debug.Log("[UserController] Pixel Pos After X Step: (" + movePos[0] + ", " + movePos[1] + ")");

		//Debug.Log("[UserController] ===== END X STEP =====");

		//return
		return movePos;
	}

	//calculate one step on the y axis
	public int[] StepY(int[] thePos)
	{

		//Debug.Log("[User] ===== START Y STEP =====");

		//check input
		//aiJump.CheckInput();

		//store position
		int[] jumpPos = new int[] { thePos[0], thePos[1] };

		//Debug.Log("[UserController] Pixel Pos Before Y Step: (" + jumpPos[0] + ", " + jumpPos[1] + ")");

		//update position based on movement
		aiJump.Jump(jumpPos, remainderPos);

		//Debug.Log("[UserController] Pixel Pos After Y Step: (" + jumpPos[0] + ", " + jumpPos[1] + ")");

		//Debug.Log("[UserController] ===== END Y STEP =====");

		//return
		return jumpPos;
	}

	//check x-axis collisions
	private int[] CheckCollisionX(int[] thePos)
	{

		//store position
		int[] pos = new int[] { thePos[0], thePos[1] };

		//boundary
		pos = gameManager.collision.CheckBounds(pos, true, false);

		//if boundary collision found
		if (pos[0] != thePos[0])
		{
			//discard remainder
			remainderPos.x = 0;
		}

		//return
		return pos;
	}

	//check y-axis collisions
	private int[] CheckCollisionY(GameObject theObj, int[] theUserPosInitial, int[] theUserPosFinal, List<GameObject> theObjects, int theScrollDist)
	{
		//store position
		int[] pos = new int[] { theUserPosFinal[0], theUserPosFinal[1] };

		//store y position that the object will fall to
		int fallY = pos[1];

		//retrieve object tag
		string objTag = theObj.tag;

		//USER MOVEMENT
		//if falling
		if (aiJump.CurrentState == AIJump.JumpState.Fall)
		{

			//check fall to platform
			fallY = gameManager.collision.CheckFallToPlatform(theUserPosInitial, theUserPosFinal, gameManager.platforms, theScrollDist);

			//add optional buffer to allow for extended fall off screen
			int offScreenYBuffer = Constants.TILE_SIZE * 30;

			//calculate off-screen fall position
			int offScreenY = -Constants.SCREEN_H / 2 - Constants.TILE_SIZE / 2 - offScreenYBuffer;

			//if a collision was found
			if (fallY != pos[1])
			{

				//update position
				pos[1] = fallY;

				//update state
				aiJump.JumpGround();
			}

			//if fell off screen
			else if (fallY < offScreenY)
			{
				//reset game
				gameObject.SetActive(false);
			}
		}

		//if grounded
		else if (aiJump.CurrentState == AIJump.JumpState.Ground)
		{

			//whether object should fall based on movement
			bool isFalling = false;

			//check fall from platform
			isFalling = gameManager.collision.CheckFallFromPlatform(theUserPosInitial, theUserPosFinal, gameManager.platforms, theScrollDist);

			//if object is falling
			if (isFalling == true)
			{
				//update state
				aiJump.JumpFall();
			}
		}

		//return
		return pos;
	}
} //end class