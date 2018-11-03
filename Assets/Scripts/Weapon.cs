using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour {

    public enum WeaponType { meele, ranged}; //il tipo di arma
    public enum FireType { single, multiple, burst, charge}; //se weaponType è ranged allora viene controllato il tipo di fuoco

    public float damage;
    public float range;
    public float fireRate;
    public float shotSpeed;
    public WeaponType weaponType; //arma bianca o arma da fuoco
    public FireType fireType;
    private List<Sprite> shortRangeWeapons = null;
    private List<Sprite> longRangeWeapons = null;
    public Sprite bulletSpriteUD, bulletSpriteLR;
    private SpriteRenderer bulletRend;
    public GameObject bulletPrefab;
    private BulletController bulletStats;
    private GameObject player;


    //viene usata dai nemici per sparare al player
    public void ShootToPlayer()
    {

    }

    //usata dal player, spara in una delle quattro direzioni
    public void ShootToEnemy(char direction)
    {

    }

    public void Shoot(string direction, Vector3 playerPos)
    {
        if (direction.Equals("left"))
        {
            GameObject bullet = Instantiate(bulletPrefab, playerPos + Vector3.left, Quaternion.identity) as GameObject;
            bulletStats = bullet.GetComponent<BulletController>();
            bulletStats.SetStats(damage, bulletSpriteLR);
            bullet.GetComponent<Rigidbody2D>().velocity = -bullet.transform.right * shotSpeed;
        }
        else if (direction.Equals("right"))
        {
            GameObject bullet = Instantiate(bulletPrefab, playerPos + Vector3.right, Quaternion.identity) as GameObject;
            bulletStats = bullet.GetComponent<BulletController>();
            bulletStats.SetStats(damage, bulletSpriteLR);
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.right * shotSpeed;
        }
        else if (direction.Equals("up"))
        {
            GameObject bullet = Instantiate(bulletPrefab, playerPos + Vector3.up, Quaternion.identity) as GameObject;
            bulletStats = bullet.GetComponent<BulletController>();
            bulletStats.SetStats(damage, bulletSpriteUD);
            bullet.GetComponent<Rigidbody2D>().velocity = bullet.transform.up * shotSpeed;
        }
        else if (direction.Equals("down"))
        {
            GameObject bullet = Instantiate(bulletPrefab, playerPos + Vector3.down, Quaternion.identity) as GameObject;
            bulletStats = bullet.GetComponent<BulletController>();
            bulletStats.SetStats(damage, bulletSpriteUD);
            bullet.GetComponent<Rigidbody2D>().velocity = -bullet.transform.up * shotSpeed;
        }
    }

}
