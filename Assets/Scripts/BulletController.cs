using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BulletController : MonoBehaviour {

    public float damage;
    private float shotSpeed;
    private float range;
    private Vector3 direction;

    private float destroyTimer = 0;
    private Animator anim;
    private Vector3 playerPosition;
    private ItemStats weapon;

    private float posX, posY;
    private Rigidbody2D rb;

    private ItemStats.BulletType bulletType;

    private System.Random rand = new System.Random((int)DateTime.Now.Ticks);

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

        if (Vector3.Distance(playerPosition, transform.position) > weapon.range)
        {
            rb.velocity = Vector2.zero;
            anim.Play("Bullet explosion");
        }

    }


    public void SetStats(ItemStats weapon, Vector3 playerPosition)
    {
        this.weapon = weapon;
        this.playerPosition = playerPosition;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "Enemy")
        {
            if (coll.isTrigger)
            {
                GetComponent<Collider2D>().enabled = false;
                coll.gameObject.SendMessage("TakeDamage", weapon.damage);
                if (weapon.bulletType != ItemStats.BulletType.normal)
                {
                    if (rand.Next(0, 5) == 0)
                        coll.gameObject.SendMessage("ApplyModifier", weapon.bulletType);
                }
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
        if (weapon.fireType == ItemStats.FireType.splitShot)
        {
            GameObject.Find("EquippedWeapon").GetComponent<Weapon>().SplitBullet(weapon, transform.position);
        }
    }

}
