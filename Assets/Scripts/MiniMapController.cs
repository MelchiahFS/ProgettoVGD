using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour {

    public GameObject actualRoom, visitedRoom, actualBossRoom, visitedBossRoom, actualShopRoom, visitedShopRoom, unknownRoom;
    private Camera minimapCamera;
    public bool isCameraSet = false;


    public void SetEnterRoom(Room room)
    {
        if (!isCameraSet)
        {
            minimapCamera = GetComponentInChildren<Camera>();
            isCameraSet = true;
        }

        if (room.bossRoom)
        {
            room.actualBossMapSprite.SetActive(true);
            room.visitedBossMapSprite.SetActive(false);
            minimapCamera.transform.position = room.actualBossMapSprite.transform.position;
        }
        else if (room.shopRoom)
        {
            room.actualShopMapSprite.SetActive(true);
            room.visitedShopMapSprite.SetActive(false);
            minimapCamera.transform.position = room.actualShopMapSprite.transform.position;
        }
        else
        {
            room.unknownMapSprite.SetActive(false);
            room.visitedMapSprite.SetActive(false);
            room.actualMapSprite.SetActive(true);
            minimapCamera.transform.position = room.actualMapSprite.transform.position;
        }

        Room leftRoom = GameManager.manager.GetAdiacentRoom('l');
        Room rightRoom = GameManager.manager.GetAdiacentRoom('r');
        Room upRoom = GameManager.manager.GetAdiacentRoom('u');
        Room downRoom = GameManager.manager.GetAdiacentRoom('d');

        if (upRoom != null && !upRoom.visited)
        {
            if (upRoom.bossRoom)
            {
                upRoom.actualBossMapSprite.SetActive(true);
            }
            else if (upRoom.shopRoom)
            {
                upRoom.actualShopMapSprite.SetActive(true);
            }
            else
            {
                upRoom.unknownMapSprite.SetActive(true);
            }
        }
        if (downRoom != null && !downRoom.visited)
        {
            if (downRoom.bossRoom)
            {
                downRoom.actualBossMapSprite.SetActive(true);
            }
            else if (downRoom.shopRoom)
            {
                downRoom.actualShopMapSprite.SetActive(true);
            }
            else
            {
                downRoom.unknownMapSprite.SetActive(true);
            }
        }
        if (leftRoom != null && !leftRoom.visited)
        {
            if (leftRoom.bossRoom)
            {
                leftRoom.actualBossMapSprite.SetActive(true);
            }
            else if (leftRoom.shopRoom)
            {
                leftRoom.actualShopMapSprite.SetActive(true);
            }
            else
            {
                leftRoom.unknownMapSprite.SetActive(true);
            }
        }
        if (rightRoom != null && !rightRoom.visited)
        {
            if (rightRoom.bossRoom)
            {
                rightRoom.actualBossMapSprite.SetActive(true);
            }
            else if (rightRoom.shopRoom)
            {
                rightRoom.actualShopMapSprite.SetActive(true);
            }
            else
            {
                rightRoom.unknownMapSprite.SetActive(true);
            }
        }

    }

    public void SetExitRoom(Room room)
    {

        if (room.bossRoom)
        {
            room.actualBossMapSprite.SetActive(false);
            room.visitedBossMapSprite.SetActive(true);
        }
        else if (room.shopRoom)
        {
            room.actualShopMapSprite.SetActive(false);
            room.visitedShopMapSprite.SetActive(true);
        }
        else
        {
            room.visitedMapSprite.SetActive(true);
            room.actualMapSprite.SetActive(false);
        }
    }

}
