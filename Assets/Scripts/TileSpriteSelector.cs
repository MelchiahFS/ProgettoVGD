using System;
using UnityEngine;

public class TileSpriteSelector : MonoBehaviour
{
    //queste tiles sono tutte relative al pavimento
    public Sprite outerUpLeftCorner,
        //innerUpLeftCorner, 
        outerDownLeftCorner, 
        //innerDownLeftCorner, 
        outerUpRightCorner, 
        //innerUpRightCorner, 
        outerDownRightCorner, 
        //innerDownRightCorner, 
        upFloor, 
        downFloor,
        leftFloor, 
        rightFloor, 
        floorTile,
        doorFloorLeft,
        doorFloorRight,
        doorFloorUp,
        doorFloorDown;

    //queste tiles sono tutte relative ai muri
    public Sprite leftWall,
        rightWall,
        upWall,
        downWall,
        upDownWall,
        leftRightWall,
        downLeftCorner,
        downRightCorner,
        upLeftCorner,
        upRightCorner,
        upCorners,
        downCorners,
        leftCorners,
        rightCorners,
        upDownLeftCorner,
        upDownRightCorner,
        downUpRightCorner,
        downUpLeftCorner,
        quadCorner,
        doorWallLeft,
        doorWallRight,
        doorWallUp,
        doorWallDown,
        innerWallLeft,
        innerWallCenter,
        innerWallRight;

    //queste tile sono relative ai passaggi tra le stanze
    public Sprite innerLeftDownWallCorner,
        innerLeftUpWallCorner,
        innerRightDownWallCorner,
        innerRightUpWallCorner,
        horizontalPass,
        verticalPass;

    public Sprite closedDoorUp,
        closedDoorDown,
        closedDoorLeft,
        closedDoorRight,
        openDoorUp,
        openDoorDown,
        openDoorLeft,
        openDoorRight;
    
    //sprite degli ostacoli all'interno delle stanze
    public Sprite obstacles;

    //il passaggio al livello seguente
    public Sprite stairs;

	public Sprite greyBlock;
    
   
}
