using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public Sprite bulletSpriteUD, bulletSpriteLR;
    private SpriteRenderer bulletRend;
    public GameObject bulletPrefab;
    public GameObject bullet, straightBullet, diagBullet1, diagBullet2, rearBullet;
    private BulletController bulletStats;
    Collider2D[] hitObjects;
    public bool isAttacking = false, isShooting = false;
    private float attackDuration = 0.5f;
    private float shootTimer = 0;
    private float attackTimer = 0;
    public List<ItemStats> weaponsStats; //contiene le informazioni delle armi in possesso
    private ItemStats actualWeapon;
    private Room actualRoom;
    private int i = 0;
    private Quaternion rot, rot1, rot2, rot3;
    private Vector3 dir, dir1, dir2, dir3;
    private PlayerHealth ph;
    private Animator animator;
    public float radius = 0.75f;
    public Vector3 upDistance = new Vector3(0, 0.75f, 0), downDistance = new Vector3(0, -0.75f, 0), leftDistance = new Vector3(-0.75f, 0, 0), rightDistance = new Vector3(0.75f, 0, 0);
    private Vector3 attackStart;

    void Start()
    {
        animator = GetComponentInParent<Animator>();
        ph = GetComponentInParent<PlayerHealth>();
        weaponsStats = new List<ItemStats>();

        //si potrebber creare uno scriptableObject per le armi iniziali 
        ItemStats meele = new ItemStats(ItemStats.ItemType.weapon, ItemStats.WeaponType.meele, 15);
        weaponsStats.Add(meele);
        ItemStats ranged = new ItemStats(ItemStats.ItemType.weapon, ItemStats.WeaponType.ranged, ItemStats.FireType.multiple, ItemStats.BulletType.normal, 30, 7, 0.5f, 5.5f);
        weaponsStats.Add(ranged);
        ItemStats ranged2 = new ItemStats(ItemStats.ItemType.weapon, ItemStats.WeaponType.ranged, ItemStats.FireType.splitShot, ItemStats.BulletType.slowing, 3, 7, 0.5f, 5.5f);
        weaponsStats.Add(ranged2);
        ItemStats ranged3 = new ItemStats(ItemStats.ItemType.weapon, ItemStats.WeaponType.ranged, ItemStats.FireType.bidirectional, ItemStats.BulletType.normal, 3, 7, 0.5f, 5.5f);
        weaponsStats.Add(ranged3);
        ItemStats ranged4 = new ItemStats(ItemStats.ItemType.weapon, ItemStats.WeaponType.ranged, ItemStats.FireType.single, ItemStats.BulletType.slowing, 3, 7, 0.5f, 5.5f);
        weaponsStats.Add(ranged4);
        ItemStats ranged5 = new ItemStats(ItemStats.ItemType.weapon, ItemStats.WeaponType.ranged, ItemStats.FireType.single, ItemStats.BulletType.poisonous, 3, 7, 0.5f, 5.5f);
        weaponsStats.Add(ranged5);
        ItemStats ranged6 = new ItemStats(ItemStats.ItemType.weapon, ItemStats.WeaponType.ranged, ItemStats.FireType.single, ItemStats.BulletType.burning, 3, 7, 0.5f, 5.5f);
        weaponsStats.Add(ranged6);
        actualWeapon = weaponsStats[0]; //l'arma predefinita è l'arma meele base
        attackTimer = attackDuration;
        shootTimer = actualWeapon.fireRate;
    }

    void Update()
    {
        if (!GameManager.manager.gamePause)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                if (weaponsStats[i % weaponsStats.Count] != null)
                {
                    actualWeapon = weaponsStats[i % weaponsStats.Count];
                    if (actualWeapon.weaponType == ItemStats.WeaponType.meele)
                        Debug.Log(actualWeapon.weaponType.ToString());
                    else
                        Debug.Log(actualWeapon.fireType.ToString() + ", " + actualWeapon.bulletType.ToString());
                    i++;
                }
            }

            if (actualWeapon != null)
            {
                if (actualWeapon.weaponType == ItemStats.WeaponType.meele)
                {
                    if (Input.GetKeyDown("up"))
                    {
                        if (!ph.flipAtt)
                            Attack("up", transform.position);
                        else
                            Attack("down", transform.position);
                    }
                    else if (Input.GetKeyDown("down"))
                    {
                        if (!ph.flipAtt)
                            Attack("down", transform.position);
                        else
                            Attack("up", transform.position);
                    }
                    else if (Input.GetKeyDown("left"))
                    {
                        if (!ph.flipAtt)
                            Attack("left", transform.position);
                        else
                            Attack("right", transform.position);
                    }
                    else if (Input.GetKeyDown("right"))
                    {
                        if (!ph.flipAtt)
                            Attack("right", transform.position);
                        else
                            Attack("left", transform.position);
                    }
                }
                //la differenza è solamente che con le armi ranged posso tenere premuto il tasto per sparare in modo continuo
                else
                {
                    if (Input.GetKey("up"))
                    {
                        if (!ph.flipAtt)
                            Attack("up", transform.position);
                        else
                            Attack("down", transform.position);
                    }
                    else if (Input.GetKey("down"))
                    {
                        if (!ph.flipAtt)
                            Attack("down", transform.position);
                        else
                            Attack("up", transform.position);
                    }
                    else if (Input.GetKey("left"))
                    {
                        if (!ph.flipAtt)
                            Attack("left", transform.position);
                        else
                            Attack("right", transform.position);
                    }
                    else if (Input.GetKey("right"))
                    {
                        if (!ph.flipAtt)
                            Attack("right", transform.position);
                        else
                            Attack("left", transform.position);
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
                    if (ph.faster)
                        Shoot(direction, playerPos, actualWeapon.shotSpeed + 3);
                    else if (ph.slower)
                        Shoot(direction, playerPos, actualWeapon.shotSpeed -2);
                    else
                        Shoot(direction, playerPos, actualWeapon.shotSpeed);
                }
                break;
        }
    }

    //Arma da fuoco
    public void Shoot(string direction, Vector3 playerPos, float shotSpeed)
    {
        switch (actualWeapon.fireType)
        {
            case ItemStats.FireType.single:
            case ItemStats.FireType.splitShot:
                if (direction.Equals("left"))
                {
                    rot = Quaternion.AngleAxis(90, Vector3.forward);
                    dir = rot * Vector3.up;
                    bullet = Instantiate(bulletPrefab, playerPos + new Vector3(-0.5f, -0.3f, 0), rot) as GameObject;
                }
                else if (direction.Equals("right"))
                {
                    rot = Quaternion.AngleAxis(-90, Vector3.forward);
                    dir = rot * Vector3.up;
                    bullet = Instantiate(bulletPrefab, playerPos + new Vector3(0.5f, -0.3f, 0), rot) as GameObject;
                }
                else if (direction.Equals("up"))
                {
                    rot = Quaternion.AngleAxis(0, Vector3.forward);
                    dir = rot * Vector3.up;
                    bullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, 0.75f, 0), Quaternion.identity) as GameObject;
                }
                else if (direction.Equals("down"))
                {
                    rot = Quaternion.AngleAxis(180, Vector3.forward);
                    dir = rot * Vector3.up;
                    bullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, -0.5f, 0), Quaternion.identity) as GameObject;
                }


                bulletStats = bullet.GetComponent<BulletController>();
                bulletStats.SetStats(actualWeapon, transform.position);
                bullet.GetComponent<Rigidbody2D>().velocity = dir.normalized * shotSpeed;

                actualRoom = GameManager.manager.ActualRoom;
                actualRoom.toSort.Add(bullet);
                break;

            case ItemStats.FireType.multiple:
                if (direction.Equals("left"))
                {
                    rot1 = Quaternion.AngleAxis(90, Vector3.forward);
                    rot2 = Quaternion.AngleAxis(70, Vector3.forward);
                    rot3 = Quaternion.AngleAxis(110, Vector3.forward);
                    dir1 = rot1 * Vector3.up;
                    dir2 = rot2 * Vector3.up;
                    dir3 = rot3 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(-0.5f, -0.3f, 0), rot1) as GameObject;
                    diagBullet1 = Instantiate(bulletPrefab, playerPos + new Vector3(-0.5f, -0.3f, 0), rot2) as GameObject;
                    diagBullet2 = Instantiate(bulletPrefab, playerPos + new Vector3(-0.5f, -0.3f, 0), rot3) as GameObject;
                }
                else if (direction.Equals("right"))
                {
                    rot1 = Quaternion.AngleAxis(-90, Vector3.forward);
                    rot2 = Quaternion.AngleAxis(-70, Vector3.forward);
                    rot3 = Quaternion.AngleAxis(-110, Vector3.forward);
                    dir1 = rot1 * Vector3.up;
                    dir2 = rot2 * Vector3.up;
                    dir3 = rot3 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(0.5f, -0.3f, 0), rot1) as GameObject;
                    diagBullet1 = Instantiate(bulletPrefab, playerPos + new Vector3(0.5f, -0.3f, 0), rot2) as GameObject;
                    diagBullet2 = Instantiate(bulletPrefab, playerPos + new Vector3(0.5f, -0.3f, 0), rot3) as GameObject;
                }
                else if (direction.Equals("up"))
                {
                    rot1 = Quaternion.AngleAxis(0, Vector3.forward);
                    rot2 = Quaternion.AngleAxis(-20, Vector3.forward);
                    rot3 = Quaternion.AngleAxis(20, Vector3.forward);
                    dir1 = rot1 * Vector3.up;
                    dir2 = rot2 * Vector3.up;
                    dir3 = rot3 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, 0.75f, 0), rot1) as GameObject;
                    diagBullet1 = Instantiate(bulletPrefab, playerPos + new Vector3(0, 0.75f, 0), rot2) as GameObject;
                    diagBullet2 = Instantiate(bulletPrefab, playerPos + new Vector3(0, 0.75f, 0), rot3) as GameObject;
                }
                else if (direction.Equals("down"))
                {
                    rot1 = Quaternion.AngleAxis(180, Vector3.forward);
                    rot2 = Quaternion.AngleAxis(160, Vector3.forward);
                    rot3 = Quaternion.AngleAxis(-160, Vector3.forward);
                    dir1 = rot1 * Vector3.up;
                    dir2 = rot2 * Vector3.up;
                    dir3 = rot3 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, -0.5f, 0), rot1) as GameObject;
                    diagBullet1 = Instantiate(bulletPrefab, playerPos + new Vector3(0, -0.5f, 0), rot2) as GameObject;
                    diagBullet2 = Instantiate(bulletPrefab, playerPos + new Vector3(0, -0.5f, 0), rot3) as GameObject;
                }

                straightBullet.GetComponent<BulletController>().SetStats(actualWeapon, transform.position);
                straightBullet.GetComponent<Rigidbody2D>().velocity = dir1.normalized * shotSpeed;

                diagBullet1.GetComponent<BulletController>().SetStats(actualWeapon, transform.position);
                diagBullet1.GetComponent<Rigidbody2D>().velocity = dir2.normalized * shotSpeed;

                diagBullet2.GetComponent<BulletController>().SetStats(actualWeapon, transform.position);
                diagBullet2.GetComponent<Rigidbody2D>().velocity = dir3.normalized * shotSpeed;

                actualRoom = GameManager.manager.ActualRoom;
                actualRoom.toSort.Add(straightBullet);
                actualRoom.toSort.Add(diagBullet1);
                actualRoom.toSort.Add(diagBullet2);
                break;
            case ItemStats.FireType.bidirectional:
                if (direction.Equals("left") || direction.Equals("right"))
                {
                    rot1 = Quaternion.AngleAxis(90, Vector3.forward);
                    rot2 = Quaternion.AngleAxis(-90, Vector3.forward);
                    dir1 = rot1 * Vector3.up;
                    dir2 = rot2 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(-0.5f, -0.3f, 0), rot1) as GameObject;
                    rearBullet = Instantiate(bulletPrefab, playerPos + new Vector3(0.5f, -0.3f, 0), rot2) as GameObject;
                }
                else if (direction.Equals("up") || direction.Equals("down"))
                {
                    rot1 = Quaternion.AngleAxis(0, Vector3.forward);
                    rot2 = Quaternion.AngleAxis(180, Vector3.forward);
                    dir1 = rot1 * Vector3.up;
                    dir2 = rot2 * Vector3.up;

                    straightBullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, 0.75f, 0), rot1) as GameObject;
                    rearBullet = Instantiate(bulletPrefab, playerPos + new Vector3(0, -0.75f, 0), rot2) as GameObject;
                }

                straightBullet.GetComponent<BulletController>().SetStats(actualWeapon, transform.position);
                straightBullet.GetComponent<Rigidbody2D>().velocity = dir1.normalized * shotSpeed;

                rearBullet.GetComponent<BulletController>().SetStats(actualWeapon, transform.position);
                rearBullet.GetComponent<Rigidbody2D>().velocity = dir2.normalized * shotSpeed;
                break;

        }
           
    }

    public void Slash(string direction, Vector3 playerPos)
    {

        switch (direction)
        {
            case "up":
                animator.Play("SlashUp");
                attackStart = transform.position + new Vector3(0, GetComponentInParent<Character>().RealOffset, 0) + upDistance;
                hitObjects = Physics2D.OverlapCircleAll(attackStart, radius);
                break;
            case "down":
                animator.Play("SlashDown");
                attackStart = transform.position + new Vector3(0, GetComponentInParent<Character>().RealOffset, 0) + downDistance;
                hitObjects = Physics2D.OverlapCircleAll(attackStart, radius);
                break;
            case "left":
                animator.Play("SlashLeft");
                attackStart = transform.position + new Vector3(0, GetComponentInParent<Character>().RealOffset, 0) + leftDistance;
                hitObjects = Physics2D.OverlapCircleAll(attackStart, radius);
                break;
            case "right":
                animator.Play("SlashRight");
                attackStart = transform.position + new Vector3(0, GetComponentInParent<Character>().RealOffset, 0) + rightDistance;
                hitObjects = Physics2D.OverlapCircleAll(attackStart, radius);
                break;
        }


        if (hitObjects != null)
        {
            foreach (Collider2D coll in hitObjects)
            {
                if (coll.gameObject.tag == "Enemy" && coll.isTrigger)
                {
                    Vector3 dir = coll.gameObject.transform.position - (transform.position + new Vector3(0, GetComponentInParent<Character>().RealOffset, 0));
                    RaycastHit2D hit = Physics2D.Raycast(transform.position + new Vector3(0, GetComponentInParent<Character>().RealOffset, 0), dir);
                    if (hit.collider.tag == "Enemy")
                    {
                        coll.gameObject.GetComponent<EnemyHealth>().TakeDamage(actualWeapon.damage);
                    }
                }

            }
            hitObjects = null;
        }

    }

    public void SplitBullet(ItemStats weapon, Vector3 position)
    {
        Quaternion rot1 = Quaternion.AngleAxis(-45, Vector3.forward);
        Quaternion rot2 = Quaternion.AngleAxis(-135, Vector3.forward);
        Quaternion rot3 = Quaternion.AngleAxis(-225, Vector3.forward);
        Quaternion rot4 = Quaternion.AngleAxis(-315, Vector3.forward);
        Vector3 dir1 = rot1 * Vector3.up;
        Vector3 dir2 = rot2 * Vector3.up;
        Vector3 dir3 = rot3 * Vector3.up;
        Vector3 dir4 = rot4 * Vector3.up;
        float shotSpeed;

        if (ph.faster)
            shotSpeed = weapon.shotSpeed + 3;
        else if (ph.slower)
            shotSpeed  = weapon.shotSpeed - 2;
        else
            shotSpeed = weapon.shotSpeed;

        ItemStats newWeapon = new ItemStats(weapon.itemType, weapon.weaponType, ItemStats.FireType.single, weapon.bulletType, weapon.damage / 2, weapon.range / 2, weapon.fireRate, shotSpeed);

        GameObject bullet1 = Instantiate(bulletPrefab, position + new Vector3(0.3f, 0.3f, 0), rot1) as GameObject;
        bullet1.GetComponent<BulletController>().SetStats(newWeapon, position);
        bullet1.GetComponent<Rigidbody2D>().velocity = dir1.normalized * shotSpeed;
        bullet1.name = "b1";
        
        GameObject bullet2 = Instantiate(bulletPrefab, position + new Vector3(0.3f, -0.3f ,0), rot2) as GameObject;
        bullet2.GetComponent<BulletController>().SetStats(newWeapon, position);
        bullet2.GetComponent<Rigidbody2D>().velocity = dir2.normalized * shotSpeed;
        bullet2.name = "b2";

        GameObject bullet3 = Instantiate(bulletPrefab, position + new Vector3(-0.3f, -0.3f, 0), rot3) as GameObject;
        bullet3.GetComponent<BulletController>().SetStats(newWeapon, position);
        bullet3.GetComponent<Rigidbody2D>().velocity = dir3.normalized * shotSpeed;
        bullet3.name = "b3";

        GameObject bullet4 = Instantiate(bulletPrefab, position + new Vector3(-0.3f, 0.3f, 0), rot4) as GameObject;
        bullet4.GetComponent<BulletController>().SetStats(newWeapon, position);
        bullet4.GetComponent<Rigidbody2D>().velocity = dir4.normalized * shotSpeed;
        bullet4.name = "b4";

        actualRoom = GameManager.manager.ActualRoom;
        actualRoom.toSort.Add(bullet1);
        actualRoom.toSort.Add(bullet2);
        actualRoom.toSort.Add(bullet3);
        actualRoom.toSort.Add(bullet4);
    }

    void OnDrawGizmos()
    {
        if (isAttacking)
        {
            Gizmos.color = Color.white;
            Gizmos.DrawWireSphere(attackStart, radius);
        }
        
    }

    public void EquipWeapon(ItemStats weapon)
    {
        JsonUtility.FromJsonOverwrite(JsonUtility.ToJson(weapon), actualWeapon);
    }

}
