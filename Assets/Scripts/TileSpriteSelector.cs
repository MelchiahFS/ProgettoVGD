using System;
using UnityEngine;

public class TileSpriteSelector : MonoBehaviour
{
    public Sprite outerUpLeftCorner,
        innerUpLeftCorner, 
        outerDownLeftCorner, 
        innerDownLeftCorner, 
        outerUpRightCorner, 
        innerUpRightCorner, 
        outerDownRightCorner, 
        innerDownRightCorner, 
        upWall, 
        downWall,
        leftWall, 
        rightWall, 
        floorTile; 

    public bool up, down, left, right;
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
                rend.sprite = upWall;
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
        else
        {
            rend.sprite = floorTile;
        }
                
    }
}
