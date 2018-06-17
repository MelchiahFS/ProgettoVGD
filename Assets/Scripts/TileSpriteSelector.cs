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


    //il passaggio al livello seguente
    public Sprite stairs;
    public bool exit;
        


    public bool up, down, left, right;
    
    //usato momentaneamente per testare i collegamenti tra stanze
    public bool doorUp, doorDown, doorLeft, doorRight;

    public bool floor, wall, innerWall, passageHor, passageVer;
    private SpriteRenderer rend;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        if (passageHor)
            PickSpriteHorizontalPassage();
        else if (passageVer)
            PickSpriteVerticalPassage();
        else
        {
            if (floor)
                PickSpriteFloor();
            else if (wall)
                PickSpriteWall();
        }
    }

    void PickSpriteFloor()
    {
        if (doorUp)
        {
            rend.sprite = doorFloorUp;
        }
        else if (doorDown)
        {
            rend.sprite = doorFloorDown;
        }
        else if (doorLeft)
        {
            rend.sprite = doorFloorLeft;
        }
        else if (doorRight)
        {
            rend.sprite = doorFloorRight;
        }
        else if (up)
        {
            if (left)
            {
                rend.sprite = outerUpLeftCorner;
            }
            else if (right)
            {
                rend.sprite = outerUpRightCorner;
            }
            else
            {
                rend.sprite = upFloor;
            }
        }
        else if (down)
        {
            if (left)
            {
                rend.sprite = outerDownLeftCorner;
            }
            else if (right)
            {
                rend.sprite = outerDownRightCorner;
            }
            else
            {
                rend.sprite = downFloor;
            }
        }
        else if (left)
        {
            rend.sprite = leftFloor;
        }
        else if (right)
        {
            rend.sprite = rightFloor;
        }
        else
        {
            rend.sprite = floorTile;
        }
                
    }

    void PickSpriteWall()
    {
        if (innerWall)
        {
            if (exit)
            {
                rend.sprite = stairs;
            }
            else if (left)
            {
                rend.sprite = innerWallLeft;
            }
            else if (right)
            {
                rend.sprite = innerWallRight;
            }
            else
            {
                rend.sprite = innerWallCenter;
            }
        }
        else if (up)
        {
            if (left)
            {
                rend.sprite = upLeftCorner;
            }
            else if (right)
            {
                rend.sprite = upRightCorner;
            }
            else
            {
                rend.sprite = upWall;
            }
        }
        else if (down)
        {
            if (left)
            {
                rend.sprite = downLeftCorner;
            }
            else if (right)
            {
                rend.sprite = downRightCorner;
            }
            else
            {
                rend.sprite = downWall;
            }
        }
        else if (left)
        {
            rend.sprite = leftWall;
        }
        else if (right)
        {
            rend.sprite = rightWall;
        }
    }

    void PickSpriteHorizontalPassage()
    {
        if (wall)
        {
            if (up)
            {
                if (left)
                    rend.sprite = innerLeftDownWallCorner;
                else if (right)
                    rend.sprite = innerRightDownWallCorner;
                else
                    rend.sprite = upWall;

            }
            else if (down)
            {
                if (left)
                    rend.sprite = innerLeftUpWallCorner;
                else if (right)
                    rend.sprite = innerRightUpWallCorner;
                else
                    rend.sprite = downWall;
            }
        }
        else if (innerWall)
        {
            if (left)
                rend.sprite = innerWallLeft;
            else if (right)
                rend.sprite = innerWallRight;
            else
                rend.sprite = innerWallCenter;
        }
        else
            rend.sprite = horizontalPass;
    }

    void PickSpriteVerticalPassage()
    {
        if (wall)
        {
            if (left)
            {
                if (up)
                {
                    rend.sprite = innerRightUpWallCorner;
                }
                else if (down)
                {
                    rend.sprite = innerRightDownWallCorner;
                }
                else
                {
                    rend.sprite = leftWall;
                }
            }
            else if (right)
            {
                if (up)
                {
                    rend.sprite = innerLeftUpWallCorner;
                }
                else if (down)
                {
                    rend.sprite = innerLeftDownWallCorner;
                }
                else
                {
                    rend.sprite = rightWall;
                }
            }
        }
        else if (innerWall)
        {
            if (left)
            {
                rend.sprite = innerWallRight;
            }
            else if (right)
            {
                rend.sprite = innerWallLeft;
            }
        }
        else
        {
            rend.sprite = verticalPass;
        }
    }
}
