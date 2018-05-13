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
        floorTile;

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
        quadCorner;


    public bool up, down, left, right;
    
    //usato momentaneamente per testare i collegamenti tra stanze
    public bool door;
    private SpriteRenderer rend;

    private void Start()
    {
        rend = GetComponent<SpriteRenderer>();
        PickSprite();
    }

    void PickSprite()
    {
        if (door)
        {
            rend.sprite = floorTile;
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
}
