using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {

    private float damage, range;
    private Vector3 enemyPosition;
    private bool flying;


	void Update ()
    {
        if (Vector3.Distance(enemyPosition, transform.position) > range)
        {
            Room actualRoom = GameManager.manager.ActualRoom;
            actualRoom.toSort.Remove(gameObject);
            Destroy(gameObject);
        }
    }

    public void SetStats(float damage, float range, Sprite sprite, Vector3 enemyPosition, bool isFlying)
    {
        this.damage = damage;
        this.range = range;
        SpriteRenderer rend = GetComponent<SpriteRenderer>();
        rend.sprite = sprite;
        this.enemyPosition = enemyPosition;
        flying = isFlying;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Player")
        {
            if (coll.isTrigger)
            {
                coll.gameObject.SendMessage("TakeDamage", damage);
                Room actualRoom = GameManager.manager.ActualRoom;
                actualRoom.toSort.Remove(gameObject);
                Destroy(gameObject);
            }
        }
        else if (coll.gameObject.tag == "Obstacle")
        {
            if (!flying)
            {
                Room actualRoom = GameManager.manager.ActualRoom;
                actualRoom.toSort.Remove(gameObject);
                Destroy(gameObject);
            }       
        }
        else if (coll.gameObject.tag != "Enemy" && coll.gameObject.tag != "DoorUp")
        {
            Room actualRoom = GameManager.manager.ActualRoom;
            actualRoom.toSort.Remove(gameObject);
            Destroy(gameObject);
        }

    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "DoorUp")
        {
            Room actualRoom = GameManager.manager.ActualRoom;
            actualRoom.toSort.Remove(gameObject);
            Destroy(gameObject);
        }

    }
}
