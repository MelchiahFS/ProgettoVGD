using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {

    private float damage, range;
    private Vector3 enemyPosition;


	void Update ()
    {
        if (Vector3.Distance(enemyPosition, transform.position) > range)
        {
            Destroy(gameObject);
        }
    }

    public void SetStats(float damage, float range, Sprite sprite, Vector3 enemyPosition)
    {
        this.damage = damage;
        this.range = range;
        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        rend.sprite = sprite;
        this.enemyPosition = enemyPosition;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            if (coll.isTrigger)
            {
                coll.gameObject.SendMessage("TakeDamage", damage);
                Destroy(gameObject);
            }
        }
        else if (coll.gameObject.tag != "Enemy" && coll.gameObject.tag != "DoorUp")
        {
            Destroy(gameObject);
        }

    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "DoorUp")
        {
            Destroy(gameObject);
        }

    }
}
