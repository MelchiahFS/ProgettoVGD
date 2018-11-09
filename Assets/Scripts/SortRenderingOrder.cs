using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortRenderingOrder : MonoBehaviour {

    public Room actualRoom;
    private float offsetA, offsetB;

    void Update ()
    {

        actualRoom.enemies.Sort(delegate (GameObject a, GameObject b)
        {
            offsetA = a.GetComponent<Character>().RealOffset;
            offsetB = b.GetComponent<Character>().RealOffset;

            if (a.transform.position.y + offsetA < b.transform.position.y + offsetB)
                return 1;
            else if (a.transform.position.y + offsetA == b.transform.position.y + offsetB)
                return 0;
            else
                return -1;
        });

        for (int i = 0; i < actualRoom.enemies.Count; i++)
        {
            actualRoom.enemies[i].GetComponent<SpriteRenderer>().sortingOrder = i;
        }
    }

}
