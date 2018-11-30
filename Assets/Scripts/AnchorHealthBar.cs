using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorHealthBar : MonoBehaviour {


    private RectTransform rect, healthBarRT, sliderRT;
    private Vector3 hbLocalScale, parentLocalScale;
    public GameObject healthBar;

    [System.NonSerialized]
    public GameObject slider, fire, slow, poison;
    private GameObject go;

    void Awake()
    {
        go = Instantiate(healthBar, transform) as GameObject;
        go.name = healthBar.name;
        go.transform.SetParent(transform, false);

    }

    void Start ()
    {
        
        slider = go.transform.Find("Slider").gameObject;

        healthBarRT = go.GetComponent<RectTransform>();
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


        fire = go.transform.Find("Fire").gameObject;
        rect = fire.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.anchoredPosition = new Vector3(-40, -30, 0);
        rect.sizeDelta = new Vector2(40, 60);
        fire.GetComponent<SpriteRenderer>().enabled = false;

        slow = go.transform.Find("Slow").gameObject;
        rect = slow.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.anchoredPosition = new Vector3(0, -30, 0);
        rect.sizeDelta = new Vector2(40, 60);
        slow.GetComponent<SpriteRenderer>().enabled = false;

        poison = go.transform.Find("Poison").gameObject;
        rect = poison.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 1);
        rect.anchorMax = new Vector2(0.5f, 1);
        rect.anchoredPosition = new Vector3(40, -30, 0);
        rect.sizeDelta = new Vector2(40, 60);
        poison.GetComponent<SpriteRenderer>().enabled = false;


    }

}
