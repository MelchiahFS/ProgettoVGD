using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnchorIcons : MonoBehaviour
{
    private List<GameObject> iconList = new List<GameObject>(); //lista delle icone di status del player
    public Vector3 startingPoint = new Vector3(30,-50,0), offset = new Vector3(27, 0, 0); //punto di attivazione della prima icona di status

	 
    //attiva o disattiva le icone di status, e ne imposta dinamicamente la posizione
    public void SetIconPosition(GameObject o, bool add)
    {
        if (add)
        {
            if (!iconList.Contains(o))
            {
                iconList.Add(o);
                o.GetComponent<Image>().enabled = true;
            }
        }
        else
        {
            if (iconList.Contains(o))
            {
                iconList.Remove(o);
                o.GetComponent<Image>().enabled = false;
            }
        }

		//a seconda della quantità di oggetti calcola l'offset per ognuno degli oggetti
        for (int i = 0; i < iconList.Count; i++)
        {
            if (i == 0)
                iconList[i].GetComponent<RectTransform>().anchoredPosition = startingPoint;
            else
                iconList[i].GetComponent<RectTransform>().anchoredPosition = startingPoint + offset * i;
        }

    }
}
