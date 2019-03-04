using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour {

    public GameObject actualRoom, visitedRoom, actualBossRoom, actualShopRoom, unknownRoom;
    private Camera minimapCamera;
    public bool isCameraSet = false;

	//Imposta la sprite della minimappa per la stanza corrente e quelle adiacenti
    public void SetEnterRoom(Room room)
    {
		//recupero la minimapCamera
        if (!isCameraSet)
        {
            minimapCamera = GetComponentInChildren<Camera>();
            isCameraSet = true;
        }

		//imposto la sprite per la stanza attuale
        if (room.bossRoom)
        {
            room.actualBossMapSprite.SetActive(true);
            minimapCamera.transform.position = room.actualBossMapSprite.transform.position;
        }
        else if (room.shopRoom)
        {
            room.actualShopMapSprite.SetActive(true);
            minimapCamera.transform.position = room.actualShopMapSprite.transform.position;
        }
        else
        {
            room.unknownMapSprite.SetActive(false);
            room.visitedMapSprite.SetActive(false);
            room.actualMapSprite.SetActive(true);
            minimapCamera.transform.position = room.actualMapSprite.transform.position;
        }

		//recupero le stanze adiacenti
        Room leftRoom = GameManager.manager.GetAdiacentRoom('l');
        Room rightRoom = GameManager.manager.GetAdiacentRoom('r');
        Room upRoom = GameManager.manager.GetAdiacentRoom('u');
        Room downRoom = GameManager.manager.GetAdiacentRoom('d');

		//imposto le sprite delle stanze adiacenti a seconda che siano state visitate o no
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

	//imposto la sprite della stanza visitata per la stanza attuala
    public void SetExitRoom(Room room)
    {
		if (!room.bossRoom && !room.shopRoom)
        {
            room.visitedMapSprite.SetActive(true);
            room.actualMapSprite.SetActive(false);
        }
    }

}
