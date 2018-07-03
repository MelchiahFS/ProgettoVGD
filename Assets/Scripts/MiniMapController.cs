using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour {

    public GameObject newRoom, visitedRoom;
    private Camera minimapCamera;
    public bool isCameraSet = false;


    public void SetEnterRoom(Room room)
    {
        if (!isCameraSet)
        {
            minimapCamera = GetComponentInChildren<Camera>();
            isCameraSet = true;
        }
        room.visitedMapSprite.SetActive(false);
        room.actualMapSprite.SetActive(true);
        minimapCamera.transform.position = room.actualMapSprite.transform.position;

    }

    public void SetExitRoom(Room room)
    {
        room.visitedMapSprite.SetActive(true);
        room.actualMapSprite.SetActive(false);
    }

}
