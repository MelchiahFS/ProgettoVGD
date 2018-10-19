using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorHealthBar : MonoBehaviour {


    RectTransform thisRT, healthBarRT, sliderRT;
    Vector3 hbLocalScale, parentLocalScale;
    GameObject healthBar, slider;

    void Start ()
    {
        healthBar = transform.Find("HealthBar").gameObject;
        slider = healthBar.transform.Find("Slider").gameObject;

        healthBarRT = healthBar.GetComponent<RectTransform>();
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

}
