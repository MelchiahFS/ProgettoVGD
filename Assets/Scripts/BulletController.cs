using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour {

    private float damage;
    private float shotSpeed;
    private float range;
    private Vector3 direction;
    private SpriteRenderer rend;

    private float destroyTimer = 0;
    private Animator anim;
    private Transform playerTransform;

    private float posX, posY;
    private Vector3 movementDirection, lastFramePosition;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        lastFramePosition = transform.position;
    }

    void Update()
    {
        movementDirection = transform.position - lastFramePosition;

        posX = movementDirection.x;
        posY = movementDirection.y;

        anim.SetFloat("PosX", posX);
        anim.SetFloat("PosY", posY);

        lastFramePosition = transform.position;

        destroyTimer += Time.deltaTime;

        if (destroyTimer > 8)
        {
            anim.Play("Bullet explosion");
            rb.velocity = Vector2.zero;
        }
            
        if (Vector3.Distance(playerTransform.position, transform.position) > range)
        {
            anim.Play("Bullet explosion");
            rb.velocity = Vector2.zero;
        }
            
    }

    public void SetStats(float damage, float range, Sprite bulletSprite)
    {
        this.damage = damage;
        this.range = range;
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
                anim.Play("Bullet explosion");
                rb.velocity = Vector2.zero;
            }               
        }
        else if (coll.gameObject.tag != "Player" && coll.gameObject.tag != "DoorUp")
        {
            anim.Play("Bullet explosion");
            rb.velocity = Vector2.zero;
        }
            
    }

    void OnTriggerExit2D(Collider2D coll)
    {
        if (coll.gameObject.tag == "DoorUp")
        {
            anim.Play("Bullet explosion");
            rb.velocity = Vector2.zero;
        }
            
    }

    //Viene chiamata da un animation event alla fine dell'animazione Bullet explosion
    public void BulletDestruction()
    {
        Destroy(gameObject);
    }

}
