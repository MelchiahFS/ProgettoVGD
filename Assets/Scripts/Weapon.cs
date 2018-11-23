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
    public GameObject bullet;
    private BulletController bulletStats;
    Collider2D[] hitObjects;
    public bool isAttacking = false, isShooting = false;
    //private int meleeDamage = 30;
    private float attackDuration = 0.5f;
    private float shootTimer = 0;
    private float attackTimer = 0;
    public List<ItemStats> weaponsStats; //contiene le informazioni delle armi in possesso
    private ItemStats actualWeapon;
    private Room actualRoom;

    private Animator animator;

    void Start()
    {
        animator = GetComponentInParent<Animator>();

        weaponsStats = new List<ItemStats>();

        //si potrebber creare uno scriptableObject per le armi iniziali 
        ItemStats meele = new ItemStats(ItemStats.ItemType.weapon, ItemStats.WeaponType.meele, 15);
        weaponsStats.Add(meele);
        ItemStats ranged = new ItemStats(ItemStats.ItemType.weapon, ItemStats.WeaponType.ranged, ItemStats.FireType.single, 3, 7, 0.5f, 5.5f);
        weaponsStats.Add(ranged);
        actualWeapon = weaponsStats[0]; //l'arma predefinita è l'arma meele base
        attackTimer = attackDuration;
        shootTimer = actualWeapon.fireRate;
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
            if (actualWeapon.weaponType == ItemStats.WeaponType.meele)
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
            else
            {
                if (Input.GetKey("up"))
                {
                    Attack("up", transform.position);
                }
                else if (Input.GetKey("down"))
                {
                    Attack("down", transform.position);

                }
                else if (Input.GetKey("left"))
                {
                    Attack("left", transform.position);

                }
                else if (Input.GetKey("right"))
                {
                    Attack("right", transform.position);
                }
            }

        }

        attackTimer += Time.deltaTime;
        shootTimer += Time.deltaTime;
        if (actualWeapon.weaponType == ItemStats.WeaponType.ranged && shootTimer >= actualWeapon.fireRate)
            isShooting = false;

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
                if (shootTimer >= actualWeapon.fireRate)
                {
                    isShooting = true;
                    shootTimer = 0;
                    Shoot(direction, playerPos);
                }
                break;
        }
    }

    //Arma da fuoco a sparo singolo
    public void Shoot(string direction, Vector3 playerPos)
    {
        if (direction.Equals("left"))
        {
            bullet = Instantiate(bulletPrefab, playerPos + new Vector3(-0.5f, -0.3f, 0), Quaternion.identity) as GameObject;
            bulletStats = bullet.GetComponent<BulletController>();
            bulletStats.SetStats(actualWeapon.damage, actualWeapon.range, bulletSpriteLR, transform.position);
            bullet.GetComponent<Rigidbody2D>().velocity = -bullet.transform.right * actualWeapon.shotSpeed;
        }
        else if (direction.Equals("right"))
        {
            bullet = Instantiate(bulletPrefab, playerPos + new Vector3(0.5f, -0.3f, 0), Quaternion.identity) as GameObject;
            bulletStats = bullet.GetComponent<BulletController>();
            bulletStats.SetStats(actualWeapon.damage, actualWeapon.range, bulletSpriteLR, transform.position);
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * actualWeapon.shotSpeed;
        }
        else if (direction.Equals("up"))
        {
            bullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, 0.75f, 0), Quaternion.identity) as GameObject;
            bulletStats = bullet.GetComponent<BulletController>();
            bulletStats.SetStats(actualWeapon.damage, actualWeapon.range, bulletSpriteUD, transform.position);
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * actualWeapon.shotSpeed;
        }
        else if (direction.Equals("down"))
        {
            bullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, -0.5f, 0), Quaternion.identity) as GameObject;
            bulletStats = bullet.GetComponent<BulletController>();
            bulletStats.SetStats(actualWeapon.damage, actualWeapon.range, bulletSpriteUD, transform.position);
            bullet.GetComponent<Rigidbody2D>().velocity = -bullet.transform.up * actualWeapon.shotSpeed;
        }

        actualRoom = GameManager.manager.ActualRoom;
        actualRoom.toSort.Add(bullet);
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
                {
                    Vector3 dir = coll.gameObject.transform.position - transform.position;
                    if (Physics2D.Raycast(transform.position, dir).collider.tag == "Enemy")
                    {
                        coll.gameObject.GetComponent<EnemyHealth>().TakeDamage(actualWeapon.damage);
                    }
                }
                    
            }
            hitObjects = null;
        }
  
    }


}
