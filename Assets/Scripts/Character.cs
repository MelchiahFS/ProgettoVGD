using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    private float realOffset;

    public void SetRealOffset(GameObject g)
    {
        foreach (Collider2D collider in g.GetComponents<Collider2D>())
        {
            //cerco il collider ai piedi del nemico e lo uso come posizione reale
            if (!collider.isTrigger)
            {
                RealOffset = collider.offset.y * transform.localScale.y;
                break;
            }
        }
    }

    public float RealOffset
    {
        get
        {
            return realOffset;
        }
        set
        {
            realOffset = value;
        }
    }
}
