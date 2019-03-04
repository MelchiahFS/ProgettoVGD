using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SortRenderingOrder : MonoBehaviour {

    public Room actualRoom;
    private float posA, posB;

	void Start()
	{
		actualRoom = GameManager.manager.ActualRoom;
	}

    void Update ()
    {
		//se c'è più di un object da riordinare nella lista
        if (actualRoom.toSort.Count > 0)
        {
			//creo la funzione di Sort tramite un delegato
            actualRoom.toSort.Sort(delegate (GameObject a, GameObject b)
            {
                if (a == null && b == null)
                {
                    return 0;
                }
                else if (a == null)
                {
                    return -1;
                }
                else if (b == null)
                {
                    return 1;
                }

				//se gli oggetti sono Character calcolo la posizione effettiva considerando l'offset
                if (a.GetComponent<Character>() != null)
                    posA = a.GetComponent<Character>().RealOffset + a.transform.position.y;
                else
                    posA = a.transform.position.y + 0.1f;

                if (b.GetComponent<Character>() != null)
                    posB = b.GetComponent<Character>().RealOffset + b.transform.position.y;
                else
                    posB = b.transform.position.y + 0.1f;

				//dopo aver calcolato le posizioni riordino la lista in base ad essa
                if (posA < posB)
                    return 1;
                else if (posA == posB)
                    return 0;
                else
                    return -1;
            });

			//assegno i valori ottenuti alla variabile sortingOrder dei vari SpriteRenderer
            for (int i = 0; i < actualRoom.toSort.Count; i++)
            {
                actualRoom.toSort[i].GetComponent<SpriteRenderer>().sortingOrder = i;
            }
        }
        
    }

}
