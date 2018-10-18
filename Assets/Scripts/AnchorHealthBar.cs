using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnchorHealthBar : MonoBehaviour {

	void Start ()
    {

        GameObject healthBar = transform.Find("HealthBar").gameObject;
        GameObject slider = healthBar.transform.Find("Slider").gameObject;

        //aggiungo il componente al parent
        RectTransform rt = gameObject.AddComponent(typeof(RectTransform)) as RectTransform;
        RectTransform healthBarRT = healthBar.GetComponent<RectTransform>();
        RectTransform sliderRT = slider.GetComponent<RectTransform>();

        //imposto l'anchor point e la posizione del canvas che conterrà la barra della vita in corrispondenza del nemico
        healthBarRT.anchorMin = new Vector2(0.5f, 0.5f);
        healthBarRT.anchorMax = new Vector2(0.5f, 0.5f);
        healthBarRT.localPosition = Vector3.zero;

        //imposto la barra in modo che mantenga sempre la stessa dimensione a prescindere dal valore di localScale del parent
        Vector3 hbLocalScale = healthBarRT.localScale;
        Vector3 parentLocalScale = transform.localScale;
        hbLocalScale.x /= parentLocalScale.x;
        hbLocalScale.y /= parentLocalScale.y;
        hbLocalScale.z /= parentLocalScale.z;
        healthBarRT.localScale = hbLocalScale;

        //posiziono la barra sopra la testa del nemico
        sliderRT.anchorMin = new Vector2(0.5f, 0.5f);
        sliderRT.anchorMax = new Vector2(0.5f, 0.5f);
        sliderRT.localPosition = new Vector3(0,60,0);

    }

}
