using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemStats {

    public enum ItemType { emptyslot, basic, weapon, consumable, statBooster }; //il tipo di oggetto
    public enum ConsumableType { healthUp25, healthUp50, healthDown, slowDownAll, slowDownSelf, poisonAll, poisonSelf, damageAll, damageSelf,
        flipAttack, flipMovement, invincible, speedUpSelf, speedUpAll, doubleDamage, halfDamage, doubleDamageSelf};
    public enum WeaponType { meele, ranged }; //il tipo di arma
    public enum FireType { single, multiple, splitShot, bidirectional}; //se l'arma è ranged allora viene controllato il tipo di fuoco
    public enum BulletType { normal, poisonous, slowing, burning }; //il tipo di proiettile;

    public ItemType itemType;
    public ConsumableType consumableType;
    public WeaponType weaponType;
    public FireType fireType;
    public BulletType bulletType;

    public Sprite sprite;
    public string itemName;
    public string description;
    public int currentStack = 1;
    public int maxStack = 1;

    public float damage;
    public float range;
    public float fireRate;
    public float shotSpeed;

    public float hp = 20;
    public float ammo = 25; 

    public ItemStats() { }

    //usato per creare specifiche armi ranged
    public ItemStats(ItemType iType, WeaponType wType, FireType fType, BulletType bType, float damage, float range, float fireRate, float shotSpeed)
    {
        itemType = iType;
        weaponType = wType;
        fireType = fType;
        bulletType = bType;
        this.damage = damage;
        this.range = range;
        this.fireRate = fireRate;
        this.shotSpeed = shotSpeed;
    }
     //usato per creare specifiche armi meele
    public ItemStats(ItemType iType, WeaponType wType, float damage)
    {
        itemType = iType;
        weaponType = wType;
        this.damage = damage;
    }
}
