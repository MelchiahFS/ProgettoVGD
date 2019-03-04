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

    public AudioClip explosion;
    public AudioSource source;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
		
        destroyTimer += Time.deltaTime;

		//se il proiettile è in scena per troppo tempo lo distruggo
        if (destroyTimer > 5)
        {
            rb.velocity = Vector2.zero;
            anim.Play("Bullet explosion");
        }

		//se il proiettile raggiunge il range dell'arma attuale lo distruggo
        if (Vector3.Distance(playerPosition, transform.position) > weapon.range)
        {
            rb.velocity = Vector2.zero;
            anim.Play("Bullet explosion");
        }

    }

	//imposta i riferimenti all'arma da cui il proiettile è stato sparato e la posizione del player al momento dello sparo
    public void SetStats(ItemStats weapon, Vector3 playerPosition)
    {
        this.weapon = weapon;
        this.playerPosition = playerPosition;
    }

    void OnTriggerEnter2D(Collider2D coll)
    {
		//se il proiettile ha colpito il triggerCollider di un nemico
        if (coll.gameObject.tag == "Enemy")
        {
            if (coll.isTrigger)
            {
                GetComponent<Collider2D>().enabled = false;
				//infliggo il danno al nemico
                coll.gameObject.SendMessage("TakeDamage", weapon.damage);

				//se l'arma ha proiettili che infliggono status
                if (weapon.bulletType != ItemStats.BulletType.normal)
                {
                    if (rand.Next(0, 5) == 0) //ho il 20% di probabilità di sparare un proiettile modificato
                        coll.gameObject.SendMessage("ApplyModifier", weapon.bulletType);
                }

				//infine distruggo il proiettile
                rb.velocity = Vector2.zero;
                anim.Play("Bullet explosion");
            }               
        }
		//se invece colpisce un muro o un ostacolo distruggo il proiettile
        else if (coll.gameObject.tag != "Player" && coll.gameObject.tag != "DoorUp" && coll.gameObject.tag != "Item")
        {
            rb.velocity = Vector2.zero;
            anim.Play("Bullet explosion");
        }
        

    }

	//se il proiettile colpisce il margine superiore della porta superiore lo distruggo; serve a simulare la profondità
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
		//rimuovo il proiettile dalla lista degli oggetti da riordinare della stanza attuale
        Room actualRoom = GameManager.manager.ActualRoom;
        actualRoom.toSort.Remove(gameObject);
		//infine lo distruggo
        Destroy(gameObject);

    }

    //Viene chiamata da un animation event all'inizio dell'animazione Bullet explosion
    public void BulletSplit()
    {
        source.PlayOneShot(explosion);
		//se l'arma spara proiettili splitShot chiamo la funzione SplitBullet di Weapon
        if (weapon.fireType == ItemStats.FireType.splitShot)
        {
            GameObject.Find("EquippedWeapon").GetComponent<Weapon>().SplitBullet(weapon, transform.position);
        }
    }

}
