using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortRenderingOrder : MonoBehaviour {

    public Room actualRoom;
    private float posA, posB;

    void Update ()
    {

        actualRoom.toSort.Sort(delegate (GameObject a, GameObject b)
        {
            if (a.GetComponent<Character>() != null)
                posA = a.GetComponent<Character>().RealOffset + a.transform.position.y;
            else
                posA = a.transform.position.y + 0.1f;

            if (b.GetComponent<Character>() != null)
                posB = b.GetComponent<Character>().RealOffset + b.transform.position.y;
            else
                posB = b.transform.position.y + 0.1f;

            if (posA < posB)
                return 1;
            else if (posA == posB)
                return 0;
            else
                return -1;
        });

        for (int i = 0; i < actualRoom.toSort.Count; i++)
        {
            actualRoom.toSort[i].GetComponent<SpriteRenderer>().sortingOrder = i;
        }
    }

}
