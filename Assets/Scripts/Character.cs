using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//classe da cui derivano PlayerController ed EnemyController; le proprietà comuni ad entrambi servono
//sia agli algoritmi di AI per determinare i movimenti sia per il corretto rendering in base alla posizione sull'asse Y

public abstract class Character : MonoBehaviour
{
    private float realOffset;

	//determina dove sono i "piedi" del character
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

	//assegna o restituisce il valore di offset
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
