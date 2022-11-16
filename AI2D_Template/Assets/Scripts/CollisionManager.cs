/*
CollisionManager

Manages collisions for all 
objects used throughout 
the game. 

Copyright John M. Quick
*/

using UnityEngine;
using System.Collections.Generic;

public class CollisionManager : MonoBehaviour {

    //check AABB collision between two objects
    public bool CheckCollisionAABB(int theTileSize, int[] thePosA, int[] thePosB) {

        /* 
        An axis-aligned bounding box (AABB) is always 
        parallel to our game world on the x and y axes, 
        despite any visuals that may be presented on 
        screen. Therefore, we can imagine that these 
        boxes surround all of our objects for the 
        purposes of collision detection. The size of 
        the box can be determined directly by the visual 
        of the object or be customized as necessary. 
        We know that two objects with AABBs collide when 
        they overlap in space. In calculations, we know 
        that an AABB collision occurs when the following 
        conditions are true.

        1. Object A's right edge exceeds Object B's left edge.  
        2. Object A's left edge exceeds Object B's right edge. 
        3. Object A's top edge exceeds Object B's bottom edge.
        4. Object A's bottom edge exceeds Object B's top edge.
        */

        //whether collision occurs
        bool isCollision = false;

        //store x-y coordinates
        int aX = thePosA[0];
        int aY = thePosA[1];
        int bX = thePosB[0];
        int bY = thePosB[1];

        //calculate edges
        int edgeTopA = aY + theTileSize / 2;
        int edgeBottomA = aY - theTileSize / 2;
        int edgeLeftA = aX - theTileSize / 2;
        int edgeRightA = aX + theTileSize / 2;
        int edgeTopB = bY + theTileSize / 2;
        int edgeBottomB = bY - theTileSize / 2;
        int edgeLeftB = bX - theTileSize / 2;
        int edgeRightB = bX + theTileSize / 2;

        //x axis
        if (
            edgeRightA >= edgeLeftB &&
            edgeLeftA <= edgeRightB &&
            edgeTopA >= edgeBottomB &&
            edgeBottomA <= edgeTopB
            ) {

            //update collision
            isCollision = true;
        }

        //return
        return isCollision;
    }

    //check AABB collision between two moving objects
    public bool CheckCollisionAABB(int theTileSize, int[] thePosInitialA, int[] thePosFinalA, int[] thePosInitialB, int[] thePosFinalB, int[] theSizeA = null, int[] theSizeB = null) {

        /*
        This function checks for collisions between two 
        moving objects. It accepts an initial and final 
        position for each object. From these positions, 
        the minimum and maximum values are used to 
        calculate a range of movement for each object. 
        These ranges are checked for overlap to determine 
        whether a collision occurred. The size parameters 
        account for detection with objects that are greater 
        than 1 tile size in width and/or height.
        */

        //whether collision occurs
        bool isCollision = false;

        //determine size of objects
        //if no size provided
        if (theSizeA == null) {

            //default to 1x1
            theSizeA = new int[] { 1, 1 };
        }

        //if no size provided
        if (theSizeB == null) {

            //default to 1x1
            theSizeB = new int[] { 1, 1 };
        }

        /*
		Regardless of the movement direction, 
		we want to find the minimum and maximum 
		x and y values that the objects achieved 
		during their movement. These values form 
		the ranges for each object.
		*/

        //determine ranges with offsets for tile size
        //x axis
        int aMinX =
            Mathf.Min(thePosInitialA[0], thePosFinalA[0]) -
            theSizeA[0] * theTileSize / 2;
        int aMaxX =
            Mathf.Max(thePosInitialA[0], thePosFinalA[0]) +
            theSizeA[0] * theTileSize / 2;
        int bMinX =
            Mathf.Min(thePosInitialB[0], thePosFinalB[0]) -
            theSizeB[0] * theTileSize / 2;
        int bMaxX =
            Mathf.Max(thePosInitialB[0], thePosFinalB[0]) +
            theSizeB[0] * theTileSize / 2;

        //y axis
        int aMinY =
            Mathf.Min(thePosInitialA[1], thePosFinalA[1]) -
            theSizeA[1] * theTileSize / 2;
        int aMaxY =
            Mathf.Max(thePosInitialA[1], thePosFinalA[1]) +
            theSizeA[1] * theTileSize / 2;
        int bMinY =
            Mathf.Min(thePosInitialB[1], thePosFinalB[1]) -
            theSizeB[1] * theTileSize / 2;
        int bMaxY =
            Mathf.Max(thePosInitialB[1], thePosFinalB[1]) +
            theSizeB[1] * theTileSize / 2;

        /*
		To determine whether a collision occurred, 
		we examine the edges of both ranges. If they 
		overlap at any point, a collision occurred. 
		*/

        //if overlap
        if (aMinX <= bMaxX && aMaxX >= bMinX &&
            aMinY <= bMaxY && aMaxY >= bMinY) {

            //update collision
            isCollision = true;
        }

        //return
        return isCollision;
    }

    //check boundary collisions
    public int[] CheckBounds(int[] thePos, bool theIsX = true, bool theIsY = true) {

        /*
        We want to make sure the object stays within 
        the boundaries of the game world. We do this 
        by checking its position against the edges of 
        the screen. Since the object has a center 
        origin point, we must offset our calculations 
        by one half the width/height as appropriate. 
        */

        //x axis
        //if checking x axis
        if (theIsX == true) {

            //calculate object dimensions
            int halfObjW = Constants.TILE_SIZE / 2;

            //calculate screen dimensions
            int halfScreenW = Constants.SCREEN_W / 2;

            //if object's left edge is less than screen's left edge
            if (thePos[0] - halfObjW < -halfScreenW) {

                //stop at edge
                thePos[0] = halfObjW - halfScreenW;
            }

            //if object's right edge is greater than screen's right edge
            else if (thePos[0] + halfObjW > halfScreenW) {

                //stop at edge
                thePos[0] = halfScreenW - halfObjW;
            }
        }

        //y axis
        //if checking y axis
        if (theIsY == true) {

            //calculate object dimensions
            int halfObjH = Constants.TILE_SIZE / 2;

            //calculate screen dimensions
            int halfScreenH = Constants.SCREEN_H / 2;

            //if object's top edge is greater than screen's top edge
            if (thePos[1] + halfObjH > halfScreenH) {

                //stop at edge
                thePos[1] = halfScreenH - halfObjH;
            }

            //if object's bottom edge is less than screen's bottom edge
            else if (thePos[1] - halfObjH < -halfScreenH) {

                //stop at edge
                thePos[1] = halfObjH - halfScreenH;
            }
        }

        //return
        return thePos;
    }

    //check object collisions
    public List<GameObject> CheckObjects(int[] theUserPosInitial, int[] theUserPosFinal, List<GameObject> theObjects, int theScrollDist = 0) {

        //store collisions
        List<GameObject> collisions = new List<GameObject>();

        //iterate through objects
        for (int i = 0; i < theObjects.Count; i++) {

            //retrieve user's world position as float
            Vector2 worldPos = theObjects[i].transform.position;

            //convert world position to pixels
            float pixelX = worldPos.x * Constants.PIXELS_TO_UNITS;
            float pixelY = worldPos.y * Constants.PIXELS_TO_UNITS;

            //cast pixel position to int
            int[] pixelPosFinal = new int[] {
                Mathf.RoundToInt(pixelX),
                Mathf.RoundToInt(pixelY)
            };

            //determine inital position
            int[] pixelPosInitial = new int[] {
                pixelPosFinal[0] + Mathf.Abs(theScrollDist),
                pixelPosFinal[1]
            };

            //check collision
            bool isCollision = CheckCollisionAABB(Constants.TILE_SIZE, theUserPosInitial, theUserPosFinal, pixelPosInitial, pixelPosFinal);

            //if collision found
            if (isCollision == true) {

                //add to collection
                collisions.Add(theObjects[i]);
            }
        }

        //return
        return collisions;
    }

    //check fall to platform
    public int CheckFallToPlatform(int[] theUserPosInitial, int[] theUserPosFinal, List<GameObject> theObjects, int theScrollDist) {

        /*
        A one-way platform is implemented in which the 
        object can jump up to the platform from below, 
        but cannot fall through the platform from above. 
        Hence, if a collision is found, we also ensure 
		ensure that the object's initial position was 
		fully above the platform. Thus, the object's 
		position is only updated if a valid collision 
		occurred and the object fell from above the 
		platform. Otherwise, the object is allowed to 
		fall to its previously-calculated final position.
        */

        //store y position that object will fall to
        int fallY = theUserPosFinal[1];

        //iterate through platforms
        for (int i = 0; i < theObjects.Count; i++) {

            //retrieve user's world position as float
            Vector2 worldPos = theObjects[i].transform.position;

            //convert world position to pixels
            float pixelX = worldPos.x * Constants.PIXELS_TO_UNITS;
            float pixelY = worldPos.y * Constants.PIXELS_TO_UNITS;

            //cast pixel position to int
            int[] pixelPosFinal = new int[] {
                Mathf.RoundToInt(pixelX),
                Mathf.RoundToInt(pixelY)
            };

            //determine inital position
            int[] pixelPosInitial = new int[] {
                pixelPosFinal[0] + Mathf.Abs(theScrollDist),
                pixelPosFinal[1]
            };

            /*
            Our AABB collision check function assumes that 
            the objects are composed of a single tile. Since 
            our platforms are composed of multiple tiles, we 
            pass the size of the platform into our function. 
            Thus, the entire platform is taken into account 
            when detecting collisions.
            */

            //retrieve number of tiles in object
            int numTiles = Mathf.Max(1, theObjects[i].transform.childCount);

            //store size of object
            int[] size = new int[] { numTiles, 1 };

            //check collision
            bool isCollision = CheckCollisionAABB(Constants.TILE_SIZE, theUserPosInitial, theUserPosFinal, pixelPosInitial, pixelPosFinal, null, size);

            //if collision found
            if (isCollision == true) {

                //calculate user's bottom edge
                int userEdgeB = theUserPosInitial[1] - Constants.TILE_SIZE / 2;

                //calculate platform's top edge
                int platformEdgeT = pixelPosInitial[1] + Constants.TILE_SIZE / 2;

                //calculate position atop platform
                //offset for user's center origin
                int userOnPlatformY = platformEdgeT + Constants.TILE_SIZE / 2;

                //if object fell from above platform
                if (userEdgeB >= platformEdgeT) {

                    //update fall position
                    fallY = userOnPlatformY;
                }
            }
        }

        //return
        return fallY;
    }

    //check fall from platform
    public bool CheckFallFromPlatform(int[] theUserPosInitial, int[] theUserPosFinal, List<GameObject> theObjects, int theScrollDist) {

        /*
        If the object is grounded, it has the 
        potential to fall from an edge. To 
        detect this, we determine whether the 
        object remains collided with a platform  
        tile after incorporating the latest 
        movement. This implementation allows 
        the character to overhang an edge and
        appear to be partially suspended in air. 
        An alternative method would be to cause 
        the object to instantly tip over the 
        edge once it reaches a certain point.
        */

        //whether object fell from platform
        bool isFalling = true;

        //iterate through platforms
        for (int i = 0; i < theObjects.Count; i++) {

            //retrieve user's world position as float
            Vector2 worldPos = theObjects[i].transform.position;

            //convert world position to pixels
            float pixelX = worldPos.x * Constants.PIXELS_TO_UNITS;
            float pixelY = worldPos.y * Constants.PIXELS_TO_UNITS;

            //cast pixel position to int
            //floors value to nearest pixel
            int[] pixelPosFinal = new int[] {
                Mathf.RoundToInt(pixelX),
                Mathf.RoundToInt(pixelY)
            };

            //determine inital position
            int[] pixelPosInitial = new int[] {
                pixelPosFinal[0] + Mathf.Abs(theScrollDist),
                pixelPosFinal[1]
            };

            //retrieve number of tiles in object
            int numTiles = Mathf.Max(1, theObjects[i].transform.childCount);

            //store size of object
            int[] size = new int[] { numTiles, 1 };

            //check collision
            bool isCollision = CheckCollisionAABB(Constants.TILE_SIZE, theUserPosInitial, theUserPosFinal, pixelPosFinal, pixelPosInitial, null, size);

            //if collision found and user is above object
            if (isCollision == true && theUserPosInitial[1] > pixelPosInitial[1]) {

                //toggle flag
                isFalling = false;

                //exit loop
                break;
            }
        }

        //return
        return isFalling;
    }

} //end class