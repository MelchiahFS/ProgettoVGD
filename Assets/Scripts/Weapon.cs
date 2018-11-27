using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public Sprite bulletSpriteUD, bulletSpriteLR;
    private SpriteRenderer bulletRend;
    public GameObject bulletPrefab;
    public GameObject bullet, straightBullet, diagBullet1, diagBullet2;
    private BulletController bulletStats;
    Collider2D[] hitObjects;
    public bool isAttacking = false, isShooting = false;
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
        //ItemStats meele = new ItemStats(ItemStats.ItemType.weapon, ItemStats.WeaponType.meele, 15);
        ItemStats meele = new ItemStats(ItemStats.ItemType.weapon, ItemStats.WeaponType.ranged, ItemStats.FireType.multiple, ItemStats.BulletType.normal, 3, 7, 0.5f, 5.5f);
        weaponsStats.Add(meele);
        ItemStats ranged = new ItemStats(ItemStats.ItemType.weapon, ItemStats.WeaponType.ranged, ItemStats.FireType.bidirectional, ItemStats.BulletType.split, 3, 7, 0.5f, 5.5f);
        weaponsStats.Add(ranged);
        actualWeapon = weaponsStats[0]; //l'arma predefinita è l'arma meele base
        attackTimer = attackDuration;
        shootTimer = actualWeapon.fireRate;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            if (weaponsStats[0] != null)
                actualWeapon = weaponsStats[0];
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
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

    //Arma da fuoco
    public void Shoot(string direction, Vector3 playerPos)
    {
        switch (actualWeapon.fireType)
        {
            case ItemStats.FireType.single:
                if (direction.Equals("left"))
                {
                    Quaternion rot = Quaternion.AngleAxis(90, Vector3.forward);
                    Vector3 dir = rot * Vector3.up;
                    bullet = Instantiate(bulletPrefab, playerPos + new Vector3(-0.5f, -0.3f, 0), rot) as GameObject;
                    bulletStats = bullet.GetComponent<BulletController>();
                    bulletStats.SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    bullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * actualWeapon.shotSpeed;
                }
                else if (direction.Equals("right"))
                {
                    Quaternion rot = Quaternion.AngleAxis(-90, Vector3.forward);
                    Vector3 dir = rot * Vector3.up;
                    bullet = Instantiate(bulletPrefab, playerPos + new Vector3(0.5f, -0.3f, 0), rot) as GameObject;
                    bulletStats = bullet.GetComponent<BulletController>();
                    bulletStats.SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    bullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * actualWeapon.shotSpeed;
                }
                else if (direction.Equals("up"))
                {
                    bullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, 0.75f, 0), Quaternion.identity) as GameObject;
                    bulletStats = bullet.GetComponent<BulletController>();
                    bulletStats.SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * actualWeapon.shotSpeed;
                }
                else if (direction.Equals("down"))
                {
                    bullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, -0.5f, 0), Quaternion.identity) as GameObject;
                    bulletStats = bullet.GetComponent<BulletController>();
                    bulletStats.SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    bullet.GetComponent<Rigidbody2D>().velocity = -bullet.transform.up * actualWeapon.shotSpeed;
                }
                actualRoom = GameManager.manager.ActualRoom;
                actualRoom.toSort.Add(bullet);
                break;

            case ItemStats.FireType.multiple:
                if (direction.Equals("left"))
                {
                    Quaternion rot1 = Quaternion.AngleAxis(90, Vector3.forward);
                    Quaternion rot2 = Quaternion.AngleAxis(70, Vector3.forward);
                    Quaternion rot3 = Quaternion.AngleAxis(110, Vector3.forward);
                    Vector3 dir1 = rot1 * Vector3.up;
                    Vector3 dir2 = rot2 * Vector3.up;
                    Vector3 dir3 = rot3 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(-0.5f, -0.3f, 0), rot1) as GameObject;
                    straightBullet.GetComponent<BulletController>().SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    straightBullet.GetComponent<Rigidbody2D>().velocity = dir1.normalized * actualWeapon.shotSpeed;

                    diagBullet1 = Instantiate(bulletPrefab, playerPos + new Vector3(-0.5f, -0.3f, 0), rot2) as GameObject;
                    diagBullet1.GetComponent<BulletController>().SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    diagBullet1.GetComponent<Rigidbody2D>().velocity = dir2.normalized * actualWeapon.shotSpeed;

                    diagBullet2 = Instantiate(bulletPrefab, playerPos + new Vector3(-0.5f, -0.3f, 0), rot3) as GameObject;
                    diagBullet2.GetComponent<BulletController>().SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    diagBullet2.GetComponent<Rigidbody2D>().velocity = dir3.normalized * actualWeapon.shotSpeed;
                }
                else if (direction.Equals("right"))
                {
                    Quaternion rot1 = Quaternion.AngleAxis(-90, Vector3.forward);
                    Quaternion rot2 = Quaternion.AngleAxis(-70, Vector3.forward);
                    Quaternion rot3 = Quaternion.AngleAxis(-110, Vector3.forward);
                    Vector3 dir1 = rot1 * Vector3.up;
                    Vector3 dir2 = rot2 * Vector3.up;
                    Vector3 dir3 = rot3 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(0.5f, -0.3f, 0), rot1) as GameObject;
                    straightBullet.GetComponent<BulletController>().SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    straightBullet.GetComponent<Rigidbody2D>().velocity = dir1.normalized * actualWeapon.shotSpeed;

                    diagBullet1 = Instantiate(bulletPrefab, playerPos + new Vector3(0.5f, -0.3f, 0), rot2) as GameObject;
                    diagBullet1.GetComponent<BulletController>().SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    diagBullet1.GetComponent<Rigidbody2D>().velocity = dir2.normalized * actualWeapon.shotSpeed;

                    diagBullet2 = Instantiate(bulletPrefab, playerPos + new Vector3(0.5f, -0.3f, 0), rot3) as GameObject;
                    diagBullet2.GetComponent<BulletController>().SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    diagBullet2.GetComponent<Rigidbody2D>().velocity = dir3.normalized * actualWeapon.shotSpeed;
                }
                else if (direction.Equals("up"))
                {
                    Quaternion rot1 = Quaternion.AngleAxis(0, Vector3.forward);
                    Quaternion rot2 = Quaternion.AngleAxis(-20, Vector3.forward);
                    Quaternion rot3 = Quaternion.AngleAxis(20, Vector3.forward);
                    Vector3 dir1 = rot1 * Vector3.up;
                    Vector3 dir2 = rot2 * Vector3.up;
                    Vector3 dir3 = rot3 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, 0.75f, 0), rot1) as GameObject;
                    straightBullet.GetComponent<BulletController>().SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    straightBullet.GetComponent<Rigidbody2D>().velocity = dir1.normalized * actualWeapon.shotSpeed;

                    diagBullet1 = Instantiate(bulletPrefab, playerPos + new Vector3(0, 0.75f, 0), rot2) as GameObject;
                    diagBullet1.GetComponent<BulletController>().SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    diagBullet1.GetComponent<Rigidbody2D>().velocity = dir2.normalized * actualWeapon.shotSpeed;

                    diagBullet2 = Instantiate(bulletPrefab, playerPos + new Vector3(0, 0.75f, 0), rot3) as GameObject;
                    diagBullet2.GetComponent<BulletController>().SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    diagBullet2.GetComponent<Rigidbody2D>().velocity = dir3.normalized * actualWeapon.shotSpeed;
                }
                else if (direction.Equals("down"))
                {
                    Quaternion rot1 = Quaternion.AngleAxis(180, Vector3.forward);
                    Quaternion rot2 = Quaternion.AngleAxis(160, Vector3.forward);
                    Quaternion rot3 = Quaternion.AngleAxis(-160, Vector3.forward);
                    Vector3 dir1 = rot1 * Vector3.up;
                    Vector3 dir2 = rot2 * Vector3.up;
                    Vector3 dir3 = rot3 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, -0.5f, 0), rot1) as GameObject;
                    straightBullet.GetComponent<BulletController>().SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    straightBullet.GetComponent<Rigidbody2D>().velocity = dir1.normalized * actualWeapon.shotSpeed;

                    diagBullet1 = Instantiate(bulletPrefab, playerPos + new Vector3(0, -0.5f, 0), rot2) as GameObject;
                    diagBullet1.GetComponent<BulletController>().SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    diagBullet1.GetComponent<Rigidbody2D>().velocity = dir2.normalized * actualWeapon.shotSpeed;

                    diagBullet2 = Instantiate(bulletPrefab, playerPos + new Vector3(0, -0.5f, 0), rot3) as GameObject;
                    diagBullet2.GetComponent<BulletController>().SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    diagBullet2.GetComponent<Rigidbody2D>().velocity = dir3.normalized * actualWeapon.shotSpeed;
                }

                actualRoom = GameManager.manager.ActualRoom;
                actualRoom.toSort.Add(straightBullet);
                actualRoom.toSort.Add(diagBullet1);
                actualRoom.toSort.Add(diagBullet2);
                break;
            case ItemStats.FireType.bidirectional:
                if (direction.Equals("left") || direction.Equals("right"))
                {
                    Quaternion rot1 = Quaternion.AngleAxis(90, Vector3.forward);
                    Quaternion rot2 = Quaternion.AngleAxis(-90, Vector3.forward);
                    Vector3 dir1 = rot1 * Vector3.up;
                    Vector3 dir2 = rot2 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(-0.5f, -0.3f, 0), rot1) as GameObject;
                    straightBullet.GetComponent<BulletController>().SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    straightBullet.GetComponent<Rigidbody2D>().velocity = dir1.normalized * actualWeapon.shotSpeed;

                    diagBullet1 = Instantiate(bulletPrefab, playerPos + new Vector3(0.5f, -0.3f, 0), rot2) as GameObject;
                    diagBullet1.GetComponent<BulletController>().SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    diagBullet1.GetComponent<Rigidbody2D>().velocity = dir2.normalized * actualWeapon.shotSpeed;
                }
                else if (direction.Equals("up") || direction.Equals("down"))
                {
                    Quaternion rot1 = Quaternion.AngleAxis(0, Vector3.forward);
                    Quaternion rot2 = Quaternion.AngleAxis(180, Vector3.forward);
                    Vector3 dir1 = rot1 * Vector3.up;
                    Vector3 dir2 = rot2 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, 0.75f, 0), rot1) as GameObject;
                    straightBullet.GetComponent<BulletController>().SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    straightBullet.GetComponent<Rigidbody2D>().velocity = dir1.normalized * actualWeapon.shotSpeed;

                    diagBullet1 = Instantiate(bulletPrefab, playerPos + new Vector3(0, -0.75f, 0), rot2) as GameObject;
                    diagBullet1.GetComponent<BulletController>().SetStats(actualWeapon.damage, actualWeapon.range, transform.position, actualWeapon.bulletType);
                    diagBullet1.GetComponent<Rigidbody2D>().velocity = dir2.normalized * actualWeapon.shotSpeed;
                }
                break;

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
                {
                    Vector3 dir = coll.gameObject.transform.position - transform.position;
                    RaycastHit2D hit = Physics2D.Raycast(transform.position, dir);
                    if (hit.collider.tag == "Enemy")
                    {
                        coll.gameObject.GetComponent<EnemyHealth>().TakeDamage(actualWeapon.damage);
                    }
                }
                    
            }
            hitObjects = null;
        }
  
    }

    public void SplitBullet(Vector3 position)
    {
        Quaternion rot1 = Quaternion.AngleAxis(-45, Vector3.forward);
        Quaternion rot2 = Quaternion.AngleAxis(-135, Vector3.forward);
        Quaternion rot3 = Quaternion.AngleAxis(-225, Vector3.forward);
        Quaternion rot4 = Quaternion.AngleAxis(-315, Vector3.forward);
        Vector3 dir1 = rot1 * Vector3.up;
        Vector3 dir2 = rot2 * Vector3.up;
        Vector3 dir3 = rot3 * Vector3.up;
        Vector3 dir4 = rot4 * Vector3.up;

        GameObject bullet1 = Instantiate(bulletPrefab, position + new Vector3(0.3f, 0.3f, 0), rot1) as GameObject;
        bullet1.GetComponent<BulletController>().SetStats(actualWeapon.damage / 2, actualWeapon.range / 2, position, ItemStats.BulletType.normal);
        bullet1.GetComponent<Rigidbody2D>().velocity = dir1.normalized * actualWeapon.shotSpeed;
        bullet1.name = "b1";
        

        GameObject bullet2 = Instantiate(bulletPrefab, position + new Vector3(0.3f, -0.3f ,0), rot2) as GameObject;
        bullet2.GetComponent<BulletController>().SetStats(actualWeapon.damage / 2, actualWeapon.range / 2, position, ItemStats.BulletType.normal);
        bullet2.GetComponent<Rigidbody2D>().velocity = dir2.normalized * actualWeapon.shotSpeed;
        bullet2.name = "b2";

        GameObject bullet3 = Instantiate(bulletPrefab, position + new Vector3(-0.3f, -0.3f, 0), rot3) as GameObject;
        bullet3.GetComponent<BulletController>().SetStats(actualWeapon.damage / 2, actualWeapon.range / 2, position, ItemStats.BulletType.normal);
        bullet3.GetComponent<Rigidbody2D>().velocity = dir3.normalized * actualWeapon.shotSpeed;
        bullet3.name = "b3";

        GameObject bullet4 = Instantiate(bulletPrefab, position + new Vector3(-0.3f, 0.3f, 0), rot4) as GameObject;
        bullet4.GetComponent<BulletController>().SetStats(actualWeapon.damage / 2, actualWeapon.range / 2, position, ItemStats.BulletType.normal);
        bullet4.GetComponent<Rigidbody2D>().velocity = dir4.normalized * actualWeapon.shotSpeed;
        bullet4.name = "b4";

        actualRoom = GameManager.manager.ActualRoom;
        actualRoom.toSort.Add(bullet1);
        actualRoom.toSort.Add(bullet2);
        actualRoom.toSort.Add(bullet3);
        actualRoom.toSort.Add(bullet4);
    }

}
