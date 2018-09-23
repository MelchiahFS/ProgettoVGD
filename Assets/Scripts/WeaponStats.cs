using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour {

    public float damage;
    public float range;
    public float shotDelay;
    public string weaponType; //arma bianca o arma da fuoco
    public List<Sprite> shortRangeWeapons = null;
    public List<Sprite> longRangeWeapons = null;
    public Sprite slash;
    public GameObject hitLeft, hitRight, hitUp, hitDown;
    public BoxCollider2D attackLeft, attackRight, attackUp, attackDown;

    public WeaponStats()
    {
        //hitLeft = gameObject.transform.Find("hitLeft").gameObject;
        //hitRight = gameObject.transform.Find("hitRight").gameObject;
        //hitUp = gameObject.transform.Find("hitUp").gameObject;
        //hitDown = gameObject.transform.Find("hitDown").gameObject;

        //attackLeft = hitLeft.GetComponent<BoxCollider2D>();
        //attackRight = hitRight.GetComponent<BoxCollider2D>();
        //attackUp = hitUp.GetComponent<BoxCollider2D>();
        //attackDown = hitDown.GetComponent<BoxCollider2D>();

        //damage = a random value

        //a seconda del tipo di arma sceglie la sprite casualmente da una serie di sprite dell'arma
    }

}
