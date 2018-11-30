using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    public float damage;
    private float shotSpeed;
    private float range;
    private Vector3 direction;

    private float destroyTimer = 0;
    private Animator anim;
    private Vector3 playerPosition;

    private float posX, posY;
    private Rigidbody2D rb;

    private ItemStats.BulletType bulletType;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {

        destroyTimer += Time.deltaTime;

        if (destroyTimer > 8)
        {
            rb.velocity = Vector2.zero;
            anim.Play("Bullet explosion");
        }

        if (Vector3.Distance(playerPosition, transform.position) > range)
        {
            rb.velocity = Vector2.zero;
            anim.Play("Bullet explosion");
        }

    }

    public void SetStats(float damage, float range, Vector3 playerPosition, ItemStats.BulletType bType)
    {
        this.damage = damage;
        this.range = range;
        this.playerPosition = playerPosition;
        bulletType = bType;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Enemy")
        {
            if (coll.isTrigger)
            {
                coll.gameObject.SendMessage("TakeDamage", damage);
                if (bulletType != ItemStats.BulletType.normal)
                    coll.gameObject.SendMessage("ApplyModifier", bulletType);
                rb.velocity = Vector2.zero;
                anim.Play("Bullet explosion");
            }               
        }
        else if (coll.gameObject.tag != "Player" && coll.gameObject.tag != "DoorUp")
        {
            rb.velocity = Vector2.zero;
            anim.Play("Bullet explosion");
        }
            
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "DoorUp")
        {
            rb.velocity = Vector2.zero;
            anim.Play("Bullet explosion");
        }
            
    }

    //Viene chiamata da un animation event alla fine dell'animazione Bullet explosion
    public void BulletDestruction()
    {
        Room actualRoom = GameManager.manager.ActualRoom;
        actualRoom.toSort.Remove(gameObject);
        Destroy(gameObject);

    }

    //Viene chiamata da un animation event all'inizio dell'animazione Bullet explosion
    public void BulletSplit()
    {
        if (bulletType == ItemStats.BulletType.split)
        {
            GameObject.Find("EquippedWeapon").SendMessage("SplitBullet", transform.position);
        }
    }

}
