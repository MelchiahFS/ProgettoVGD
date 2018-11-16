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
    private Vector3 playerPosition;

    private float posX, posY;
    private Vector3 movementDirection, lastFramePosition;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
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
            rb.velocity = Vector2.zero;
            anim.Play("Bullet explosion");
        }

        if (Vector3.Distance(playerPosition, transform.position) > range)
        {
            rb.velocity = Vector2.zero;
            anim.Play("Bullet explosion");
        }

    }

    public void SetStats(float damage, float range, Sprite bulletSprite, Vector3 playerPosition)
    {
        this.damage = damage;
        this.range = range;
        this.playerPosition = playerPosition;
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

    //Viene chiamata da un animation event alla fine dhooell'animazione Bullet explosion
    public void BulletDestruction()
    {
        Destroy(gameObject);
    }

}
