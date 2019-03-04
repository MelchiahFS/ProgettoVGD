using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour {

    private float damage, range;
    private Vector3 enemyPosition;
    private bool flying;
	private Room actualRoom;

	void Start()
	{
		actualRoom = GameManager.manager.ActualRoom;
	}

	void Update ()
    {
		//se il proiettile supera il range lo distruggo
        if (Vector3.Distance(enemyPosition, transform.position) > range)
        {
            actualRoom.toSort.Remove(gameObject);
            Destroy(gameObject);
        }
    }

	//imposta i valori del proiettile (chiamata dai vari script di attacco)
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
		//se colpisce il triggerCollider del player infligge danno
        if (coll.gameObject.tag == "Player")
        {
            if (coll.isTrigger)
            {
                coll.gameObject.SendMessage("TakeDamage", damage);
                actualRoom.toSort.Remove(gameObject);
                Destroy(gameObject);
            }
        }
		//se colpisce un ostacolo, distruggo il proiettile solo se il nemico non vola
        else if (coll.gameObject.tag == "Obstacle")
        {
            if (!flying)
            {
                actualRoom.toSort.Remove(gameObject);
                Destroy(gameObject);
            }       
        }
		//infine distruggo il proiettile se colpisco qualsiasi cosa diversa da un altro nemico o la porta superiore
        else if (coll.gameObject.tag != "Enemy" && coll.gameObject.tag != "DoorUp")
        {
            actualRoom.toSort.Remove(gameObject);
            Destroy(gameObject);
        }

    }

	//distruggo il proiettile solo quando tocca il bordo superiore della porta (quando esce da essa)
    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "DoorUp")
        {
            actualRoom.toSort.Remove(gameObject);
            Destroy(gameObject);
        }

    }
}
