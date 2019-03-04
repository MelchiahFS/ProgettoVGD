using System;
using UnityEngine;

public class TileSpriteSelector : MonoBehaviour
{
	public static TileSpriteSelector tss;

    //tiles relative al pavimento
    public Sprite outerUpLeftCorner,
        outerDownLeftCorner, 
        outerUpRightCorner, 
        outerDownRightCorner, 
        upFloor, 
        downFloor,
        leftFloor, 
        rightFloor, 
        floorTile,
        doorFloorLeft,
        doorFloorRight,
        doorFloorUp,
        doorFloorDown;

    //tiles relative ai muri
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

    //tiles relative ai passaggi tra le stanze
    public Sprite innerLeftDownWallCorner,
        innerLeftUpWallCorner,
        innerRightDownWallCorner,
        innerRightUpWallCorner,
        horizontalPass,
        verticalPass;

	//sprites relative alle porte
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

    //sprite del passaggio al livello seguente
    public Sprite stairs;
    
	void Awake()
	{
		if (tss == null)
		{
			tss = this;
		}
		else if (tss != this)
		{
			Destroy(gameObject);
		}
	}
   
}
