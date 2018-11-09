using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public enum WeaponType { meele, ranged}; //il tipo di arma
    public enum FireType { single, multiple, burst, charge}; //se weaponType è ranged allora viene controllato il tipo di fuoco

    public WeaponType weaponType; //arma bianca o arma da fuoco
    public FireType fireType;
    public Sprite bulletSpriteUD, bulletSpriteLR;
    private SpriteRenderer bulletRend;
    public GameObject bulletPrefab;
    private BulletController bulletStats;
    Collider2D[] hitObjects;
    public bool isAttacking;
    //private int meleeDamage = 30;
    private float attackDuration = 0.5f;
    private float attackTimer = 0;
    public List<ItemStats> weaponsStats; //contiene le informazioni delle armi in possesso
    private ItemStats actualWeapon;

    private Animator animator;

    void Start()
    {
        animator = GetComponentInParent<Animator>();

        weaponsStats = new List<ItemStats>();

        //si potrebber creare uno scriptableObject per le armi iniziali 
        ItemStats meele = new ItemStats(ItemStats.ItemType.weapon, ItemStats.WeaponType.meele, 15);
        weaponsStats.Add(meele);
        ItemStats ranged = new ItemStats(ItemStats.ItemType.weapon, ItemStats.WeaponType.ranged, ItemStats.FireType.single, 3, 7, 3, 5.1f);
        weaponsStats.Add(ranged);
        actualWeapon = weaponsStats[0]; //l'arma predefinita è l'arma meele base
        attackTimer = attackDuration;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            //animator.Play("Movement");
            if (weaponsStats[0] != null)
                actualWeapon = weaponsStats[0];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            //animator.Play("Movement
            if (weaponsStats[1] != null)
                actualWeapon = weaponsStats[1];
        }

        if (actualWeapon != null)
        {

            if (Input.GetKeyDown("up"))
            {
                Attack("up", transform.position);
            }
            else if (Input.GetKeyDown("down"))
            {
                Attack("down", transform.position);

            }
            else if (Input.GetKeyDown("left"))
            {
                Attack("left", transform.position);

            }
            else if (Input.GetKeyDown("right"))
            {
                Attack("right", transform.position);
            }
        }

        attackTimer += Time.deltaTime;
        if (attackTimer >= attackDuration)
            isAttacking = false;
    }


    public void Attack(string direction, Vector3 playerPos)
    {
        switch (actualWeapon.weaponType)
        {
            case ItemStats.WeaponType.meele:
                if (attackTimer >= attackDuration)
                {
                    attackTimer = 0;
                    isAttacking = true;
                    Slash(direction, playerPos);
                }   
                break;
            case ItemStats.WeaponType.ranged:
                Shoot(direction, playerPos);
                break;
        }
    }

    //Arma da fuoco a sparo singolo
    public void Shoot(string direction, Vector3 playerPos)
    {
        if (direction.Equals("left"))
        {
            GameObject bullet = Instantiate(bulletPrefab, playerPos + Vector3.left, Quaternion.identity) as GameObject;
            bulletStats = bullet.GetComponent<BulletController>();
            bulletStats.SetStats(actualWeapon.damage, bulletSpriteLR);
            bullet.GetComponent<Rigidbody2D>().velocity = -bullet.transform.right * actualWeapon.shotSpeed;
        }
        else if (direction.Equals("right"))
        {
            GameObject bullet = Instantiate(bulletPrefab, playerPos + Vector3.right, Quaternion.identity) as GameObject;
            bulletStats = bullet.GetComponent<BulletController>();
            bulletStats.SetStats(actualWeapon.damage, bulletSpriteLR);
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * actualWeapon.shotSpeed;
        }
        else if (direction.Equals("up"))
        {
            GameObject bullet = Instantiate(bulletPrefab, playerPos + Vector3.up, Quaternion.identity) as GameObject;
            bulletStats = bullet.GetComponent<BulletController>();
            bulletStats.SetStats(actualWeapon.damage, bulletSpriteUD);
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * actualWeapon.shotSpeed;
        }
        else if (direction.Equals("down"))
        {
            GameObject bullet = Instantiate(bulletPrefab, playerPos + Vector3.down, Quaternion.identity) as GameObject;
            bulletStats = bullet.GetComponent<BulletController>();
            bulletStats.SetStats(actualWeapon.damage, bulletSpriteUD);
            bullet.GetComponent<Rigidbody2D>().velocity = -bullet.transform.up * actualWeapon.shotSpeed;
        }
    }



    public void Slash(string direction, Vector3 playerPos)
    {

        switch (direction)
        {
            case "up":
                animator.Play("SlashUp");
                hitObjects = Physics2D.OverlapCircleAll(transform.position + new Vector3(0, 0.5f, 0), 0.75f);
                break;
            case "down":
                animator.Play("SlashDown");
                hitObjects = Physics2D.OverlapCircleAll(transform.position + new Vector3(0, -0.5f, 0), 0.75f);
                break;
            case "left":
                animator.Play("SlashLeft");
                hitObjects = Physics2D.OverlapCircleAll(transform.position + new Vector3(-0.5f, 0, 0), 0.75f);
                break;
            case "right":
                animator.Play("SlashRight");
                hitObjects = Physics2D.OverlapCircleAll(transform.position + new Vector3(0.5f, 0, 0), 0.75f);
                break;
        }
                

        if (hitObjects != null)
        {
            Debug.Log(hitObjects.Length);
            foreach (Collider2D coll in hitObjects)
            {
                if (coll.gameObject.tag == "Enemy" && coll.isTrigger)
                    coll.gameObject.GetComponent<EnemyHealth>().TakeDamage(actualWeapon.damage);
            }
            hitObjects = null;
        }
  
    }


}
