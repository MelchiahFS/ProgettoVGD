using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneObject : MonoBehaviour
{
    private float realOffset;

    void Start()
    {
        foreach (Collider2D collider in GetComponents<Collider2D>())
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
