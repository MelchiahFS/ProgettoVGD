using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ItemStats {

    public enum ItemType { emptyslot, basic, weapon, consumable, key, money}; //elenco dei tipi di oggetto
	//elenco dei tipi di consumables
    public enum ConsumableType { healthUp25, healthUp50, slowDownAll, slowDownSelf, poisonAll, poisonSelf, damageAll, damageSelf,
        flipAttack, flipMovement, invincible, speedUpSelf, speedUpAll, doubleDamage, halfDamage, getDoubleDamage};
    public enum WeaponType { meele, ranged }; //elenco dei tipi di arma
    public enum FireType { single, multiple, splitShot, bidirectional}; //elenco dei tipi di fuoco (arma ranged)
    public enum BulletType { normal, poisonous, slowing, burning }; //elenco dei tipi di proiettile (arma ranged);


    public ItemType itemType;
    public ConsumableType consumableType;
    public WeaponType weaponType;
    public FireType fireType;
    public BulletType bulletType;

	//informazioni sull'oggetto per la gestione dell'inventario
    public Sprite sprite; 
    public string itemName;
    public string description;
    public int currentStack = 1;
    public int maxStack = 5;
	public bool used = false; //indica se il consumable è già stato utilizzato dal player nella partita corrente (se sì mostra le informazioni nell'inventario)

	//valore relativo al quantitativo di soldi
	public int moneyAmount;

	//valori relativi alle statistiche delle armi
    public float damage;
    public float range;
    public float fireRate;
    public float shotSpeed;

	//valori per la gestione dello shop
    public bool toBuy;
    public int price;

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
