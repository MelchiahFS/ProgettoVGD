using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    private float damage;
    private float shotSpeed;
    private Vector3 direction;
    private SpriteRenderer rend;

    private float destroyTimer = 0;

    void Update()
    {
        destroyTimer += Time.deltaTime;
        if (destroyTimer > 8)
            Destroy(gameObject);
    }

    public void SetStats(float damage, Sprite bulletSprite)
    {
        this.damage = damage;
        rend = GetComponent<SpriteRenderer>();
        rend.sprite = bulletSprite;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Enemy")
        {
            if (coll.isTrigger)
            {
                coll.gameObject.SendMessage("TakeDamage", damage);
                Destroy(gameObject);
            }               
        }
        else if (coll.gameObject.tag != "Player" && coll.gameObject.tag != "DoorUp")
            Destroy(gameObject);
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "DoorUp")
            Destroy(gameObject);
    }

}
