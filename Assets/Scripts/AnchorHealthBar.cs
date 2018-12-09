using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorHealthBar : MonoBehaviour {


    private RectTransform poisonRect, fastRect, slowRect, burnRect, healthBarRT, sliderRT;
    private Vector3 hbLocalScale, parentLocalScale;
    public GameObject healthBar;

    [System.NonSerialized]
    public GameObject slider, fire, slow, poison;
    private List<GameObject> iconList = new List<GameObject>();
    private GameObject hb;


    void Awake()
    {
        //imposto la healthBar come gameObject figlio
        hb = Instantiate(healthBar, transform) as GameObject;
        hb.name = healthBar.name;
        hb.transform.SetParent(transform, false);

    }

    void Start ()
    {
        
        slider = hb.transform.Find("Slider").gameObject;

        healthBarRT = hb.GetComponent<RectTransform>();
        sliderRT = slider.GetComponent<RectTransform>();

        //posiziono la barra sopra la testa del nemico
        sliderRT.anchorMin = new Vector2(0.5f, 0.8f);
        sliderRT.anchorMax = new Vector2(0.5f, 0.8f);
        sliderRT.anchoredPosition = Vector3.zero;

        //imposto l'anchor point e la posizione del canvas che conterrà la barra della vita in corrispondenza del nemico
        healthBarRT.anchorMin = new Vector2(0.5f, 0.5f);
        healthBarRT.anchorMax = new Vector2(0.5f, 0.5f);
        healthBarRT.localPosition = Vector3.zero;

        //lo scale di base per la barra della vita
        healthBarRT.localScale = new Vector3(0.015f, 0.015f, 0.015f);

        //imposto la barra in modo che mantenga sempre la stessa dimensione a prescindere dal valore di localScale del parent
        hbLocalScale = healthBarRT.localScale;
        parentLocalScale = transform.localScale;
        hbLocalScale.x /= parentLocalScale.x;
        hbLocalScale.y /= parentLocalScale.y;
        hbLocalScale.z /= parentLocalScale.z;
        healthBarRT.localScale = hbLocalScale;

    }

    //attiva o disattiva le icone di status, e ne imposta dinamicamente la posizione
    public void SetIconPosition(GameObject o, bool add)
    {
        if (add)
        { 
            if(!iconList.Contains(o))
            {
                iconList.Add(o);
                o.GetComponent<SpriteRenderer>().enabled = true;
            }
        }
        else
        {
            iconList.Remove(o);
            o.GetComponent<SpriteRenderer>().enabled = false;
        }

        if (iconList.Count == 1)
            iconList[0].GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -30, 0);
        else if (iconList.Count == 2)
        {
            iconList[0].GetComponent<RectTransform>().anchoredPosition = new Vector3(-20, -30, 0);
            iconList[1].GetComponent<RectTransform>().anchoredPosition = new Vector3(20, -30, 0);
        }
        else if (iconList.Count == 3)
        {
            iconList[0].GetComponent<RectTransform>().anchoredPosition = new Vector3(-40, -30, 0);
            iconList[1].GetComponent<RectTransform>().anchoredPosition = new Vector3(0, -30, 0);
            iconList[2].GetComponent<RectTransform>().anchoredPosition = new Vector3(40, -30, 0);
        }
    }


}
