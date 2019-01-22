﻿using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class LootGenerator : MonoBehaviour {

    private static System.Random rnd = new System.Random((int)DateTime.Now.Ticks);
    public float minMeeleDmg, maxMeeleDmg, minRangedDmg, maxRangedDmg, minRng, maxRng, minFR, maxFR, minSP, maxSP; //minimo e massimo danno (per meele e ranged), range, fire rate e shot speed
    private Array enumValues;
    public List<Item> consumablesSO;
    public List<ItemStats> consumables;
    private List<ItemStats> items = new List<ItemStats>();
    public GameObject sceneItemPrefab;
    private SceneItem si;
    private ItemSpriteSelector select;
    private SpriteRenderer render;
    private ItemStats actualItem;
    public Room actualRoom;
    private PlayerHealth ph;
    public GameObject pricePrefab;
    private bool keyGen = false;

    private int roomsWithEnemies = 0;
   
    void Start()
    {
        roomsWithEnemies = GameManager.manager.lvlManager.roomNumber - 3; //tolgo la stanza iniziale, lo shop e la stanza del boss
        maxMeeleDmg = 100;


        ph = GetComponentInParent<PlayerHealth>();

        select = GetComponent<ItemSpriteSelector>();
        consumables = new List<ItemStats>();

        //converto gli scriptableObject in oggetti utilizzabili nel gioco
        foreach (Item c in consumablesSO)
        {
            ItemStats item = new ItemStats();

            if (c.itemName == "Little Health Potion" || c.itemName == "Big Health Potion")
            {
                item.itemName = c.itemName;
                item.sprite = c.icon;
            }
            else
            {
                if (rnd.Next(2) == 0)
                {
                    item.itemName = "Book of " + c.itemName;
                    item.sprite = select.books[rnd.Next(select.books.Count)];
                }
                else
                {
                    item.itemName = "Scroll of " + c.itemName;
                    item.sprite = select.scrolls[rnd.Next(select.scrolls.Count)];
                }
            }
            item.description = c.itemDescription;
            item.itemType = c.type;
            item.consumableType = c.consumableType;
            consumables.Add(item);
        }

        actualRoom = GameManager.manager.ActualRoom;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (consumables.Count > 0)
            {
                actualItem = consumables[0];
                Debug.Log(actualItem.consumableType.ToString());
                
                ApplyEffect(actualItem.consumableType);
                consumables.Remove(consumables[0]);
            }
            
        }
        //se la stanza è lo shop
        if (actualRoom.shopRoom && !actualRoom.hasGenReward)
        {
            //e se il player è effettivamente dentro la stanza allora genero una ricompensa
            if (transform.position.x > actualRoom.gridPos.x && transform.position.x < (actualRoom.gridPos.x + GameManager.manager.lvlManager.roomSizeX)
                && transform.position.y > actualRoom.gridPos.y && transform.position.y < (actualRoom.gridPos.y + GameManager.manager.lvlManager.roomSizeY))
            {
                int i = 0;
                foreach (Vector2 pos in actualRoom.freePositions)
                {
                    if (i < 3)
                        InstantiateWeapon(pos, true);
                    else
                        InstantiateConsumable(pos, true);
                    i++;
                }
                i = 0;
                actualRoom.hasGenReward = true;
            }
            
        }

        //se la stanza non ha nemici e la ricompensa non è ancora stata generata
        else if (!actualRoom.startRoom && !actualRoom.hasGenReward && actualRoom.enemyNumber == 0 && actualRoom.enemyWaves == 0)
        {
            //e se il player è effettivamente dentro la stanza allora genero una ricompensa
            if (transform.position.x > actualRoom.gridPos.x && transform.position.x < (actualRoom.gridPos.x + GameManager.manager.lvlManager.roomSizeX)
                && transform.position.y > actualRoom.gridPos.y && transform.position.y < (actualRoom.gridPos.y + GameManager.manager.lvlManager.roomSizeY))
            {
                if (!keyGen)
                {
                    float keyProb = (float) 1 / roomsWithEnemies * 100;
                    int randomValue = rnd.Next(100);
                    if (randomValue <= (int)keyProb)
                    {
                        InstantiateKey(actualRoom.freePositions[rnd.Next(actualRoom.freePositions.Count)]);
                        keyGen = true;
                    }
                    else
                    {
                        roomsWithEnemies--;
                    }
                }
                //ho il 50% di ottenere o no una ricompensa
                if (rnd.Next(100) >= 0)
                {
                    int seed = rnd.Next(100);
                    if (seed < 100)
                        InstantiateWeapon(actualRoom.freePositions[rnd.Next(actualRoom.freePositions.Count)], false);
                    else
                        InstantiateConsumable(actualRoom.freePositions[rnd.Next(actualRoom.freePositions.Count)], false);
                }
                actualRoom.hasGenReward = true;
            }   
        }
    }

    public void InstantiateConsumable(Vector3 pos, bool isShop)
    {
        GameObject cons = Instantiate(sceneItemPrefab, pos, Quaternion.identity) as GameObject;
        si = cons.GetComponent<SceneItem>();
        si.Info = consumables[rnd.Next(consumables.Count)];
        render = cons.GetComponent<SpriteRenderer>();
        render.sprite = si.Info.sprite;
        if (isShop)
        {
            GameObject price = Instantiate(pricePrefab, cons.transform) as GameObject;
            si.Info.toBuy = true;
            si.Info.price = 30;
            price.GetComponentInChildren<Text>().text = si.Info.price.ToString();
            price.transform.SetParent(cons.transform, false);
        }

        //imposto i consumable come trasparenti, per poi fare un effetto di fade-in quando verranno attivati 
        Color color = render.color;
        color.a = 0;
        render.color = color;

        StartCoroutine(GameManager.manager.lvlManager.FadeIn(render, 0.3f));
    }

    public void InstantiateWeapon(Vector3 pos, bool isShop)
    {
        GameObject weapon = Instantiate(sceneItemPrefab, pos, Quaternion.identity) as GameObject;
        si = weapon.GetComponent<SceneItem>();
        render = weapon.GetComponent<SpriteRenderer>();
        SetWeaponStats(si);
        if (isShop)
        {
            GameObject price = Instantiate(pricePrefab, weapon.transform) as GameObject;
            si.Info.toBuy = true;
            si.Info.price = 100;
            price.GetComponentInChildren<Text>().text = si.Info.price.ToString();
            price.transform.SetParent(weapon.transform, false);
        }

        //imposto i consumable come trasparenti, per poi fare un effetto di fade-in quando verranno attivati 
        Color color = render.color;
        color.a = 0;
        render.color = color;

        StartCoroutine(GameManager.manager.lvlManager.FadeIn(render, 0.3f));
    }

    private void SetWeaponStats(SceneItem i)
    {
        i.Info = new ItemStats();

        i.Info.itemType = ItemStats.ItemType.weapon;
        Array enumValues = Enum.GetValues(typeof(ItemStats.WeaponType));
        i.Info.weaponType = (ItemStats.WeaponType) enumValues.GetValue(rnd.Next(enumValues.Length));
        ItemSpriteSelector itemSelect = GetComponent<ItemSpriteSelector>();

        if (i.Info.weaponType == ItemStats.WeaponType.meele)
        {
            i.Info.sprite = itemSelect.meeleWeapons[rnd.Next(itemSelect.meeleWeapons.Count)];
            i.GetComponent<SpriteRenderer>().sprite = i.Info.sprite;
            i.GetComponent<SpriteRenderer>().sortingLayerName = "Items";

            i.Info.itemName = "Sword";
            i.Info.damage = rnd.Next((int)minMeeleDmg * 100, (int)maxMeeleDmg * 100) / 100;
            i.Info.description = "Damage: " + i.Info.damage;
        }
        else
        {
            i.Info.sprite = itemSelect.rangedWeapons[rnd.Next(itemSelect.rangedWeapons.Count)];
            i.GetComponent<SpriteRenderer>().sprite = i.Info.sprite;
            i.GetComponent<SpriteRenderer>().sortingLayerName = "Items";

            i.Info.itemName = "Magic Staff";

            enumValues = Enum.GetValues(typeof(ItemStats.FireType));
            i.Info.fireType = (ItemStats.FireType)enumValues.GetValue(rnd.Next(enumValues.Length));

            enumValues = Enum.GetValues(typeof(ItemStats.BulletType));
            i.Info.bulletType = (ItemStats.BulletType)enumValues.GetValue(rnd.Next(enumValues.Length));

            i.Info.damage = rnd.Next((int)minRangedDmg * 100, (int)maxRangedDmg * 100) / 100;
            i.Info.range = rnd.Next((int)minRng * 100, (int)maxRng * 100) / 100;
            i.Info.fireRate = rnd.Next((int)minFR * 100, (int)maxFR * 100) / 100;
            i.Info.shotSpeed = rnd.Next((int)minSP * 100, (int)maxSP * 100) / 100;

            if (i.Info.fireType == ItemStats.FireType.single)
                i.Info.description += "Fire Type: Single Shot";
            else if (i.Info.fireType == ItemStats.FireType.multiple)
                i.Info.description += "Fire Type: Triple Shot";
            else if (i.Info.fireType == ItemStats.FireType.splitShot)
                i.Info.description += "Fire Type: Split Shot";
            else if (i.Info.fireType == ItemStats.FireType.bidirectional)
                i.Info.description += "Fire Type: Bidirectional Shot";

            if (i.Info.bulletType == ItemStats.BulletType.normal)
                i.Info.description += "\r\nBullet Type: Standard";
            else if (i.Info.bulletType == ItemStats.BulletType.poisonous)
                i.Info.description += "\r\nBullet Type: Poisonous";
            else if (i.Info.bulletType == ItemStats.BulletType.burning)
                i.Info.description += "\r\nBullet Type: Burning";
            else if (i.Info.bulletType == ItemStats.BulletType.slowing)
                i.Info.description += "\r\nBullet Type: Slowing";

            i.Info.description +=  "\r\nDamage: " + i.Info.damage + "\r\nRange: " + i.Info.range + "\r\nFire Rate: " + i.Info.fireRate + "\r\nShot Speed: " + i.Info.shotSpeed ;

        }
        
    }

    private void InstantiateKey(Vector3 pos)
    {
        GameObject key = Instantiate(sceneItemPrefab, pos, Quaternion.identity) as GameObject;
        si = key.GetComponent<SceneItem>();
        ItemSpriteSelector selector = GetComponent<ItemSpriteSelector>();
        render = key.GetComponent<SpriteRenderer>();
        si.Info = new ItemStats();
        si.Info.itemType = ItemStats.ItemType.key;
        si.Info.sprite = selector.keys[rnd.Next(selector.keys.Count)];
        si.GetComponent<SpriteRenderer>().sprite = si.Info.sprite;
        si.GetComponent<SpriteRenderer>().sortingLayerName = "Items";
        si.Info.itemName = "Boss Key";
        //imposto i consumable come trasparenti, per poi fare un effetto di fade-in quando verranno attivati 
        Color color = render.color;
        color.a = 0;
        render.color = color;

        StartCoroutine(GameManager.manager.lvlManager.FadeIn(render, 0.3f));
    }

    //------------------------------
    //Effetti relativi ai consumables

    public void ApplyEffect(ItemStats.ConsumableType consumable)
    {
        Room actualRoom = GameManager.manager.ActualRoom;

        switch (consumable)
        {
            case ItemStats.ConsumableType.healthUp25: //ok
                ph.HealthUp(25);
                break;

            case ItemStats.ConsumableType.healthUp50: //ok
                ph.HealthUp(50);
                break;

            case ItemStats.ConsumableType.slowDownAll: //ok
                foreach (GameObject en in actualRoom.enemies)
                {
                    EnemyHealth eh = en.GetComponent<EnemyHealth>();
                    if (eh.faster)
                        eh.SetNormalSpeed();
                    else
                        //chiamo startCoroutine in enemyHealth per poterlo fermare da lì, in modo da non ottenere il bug di Unity "Coroutine continue failure"
                        eh.slowCO = eh.StartCoroutine(eh.SlowDown());
                }
                break;

            case ItemStats.ConsumableType.speedUpAll: //ok
                foreach (GameObject en in actualRoom.enemies)
                {
                    EnemyHealth eh = en.GetComponent<EnemyHealth>();
                    if (eh.slowed)
                        eh.SetNormalSpeed();
                    else
                        eh.fastCO = eh.StartCoroutine(eh.SpeedUp());
                }
                break;

            case ItemStats.ConsumableType.poisonAll: //ok
                foreach (GameObject en in actualRoom.enemies)
                {
                    EnemyHealth eh = en.GetComponent<EnemyHealth>();
                    eh.poisonCO = eh.StartCoroutine(eh.Poisoned());
                }
                break;

            case ItemStats.ConsumableType.poisonSelf: //ok
                ph.poisonCO = ph. StartCoroutine(ph.Poisoned());
                break;

            case ItemStats.ConsumableType.damageAll: //ok
                foreach (GameObject en in actualRoom.enemies)
                {
                    en.GetComponent<EnemyHealth>().TakeDamage(20);
                }
                break;

            case ItemStats.ConsumableType.damageSelf: //ok
                ph.ConsumableDamage(20);
                break;

            case ItemStats.ConsumableType.flipMovement: //ok
                ph.flipMovCO = ph.StartCoroutine(ph.FlipMovement());
                break;

            case ItemStats.ConsumableType.invincible: //ok
                ph.invCO = ph.StartCoroutine(ph.Invincible());
                break;

            case ItemStats.ConsumableType.getDoubleDamage: //ok
                //se il player è invincibile il consumable non ha effetto
                if (!ph.invincible)
                    ph.gddCO = ph.StartCoroutine(ph.GetDoubleDamage());
                break;

            case ItemStats.ConsumableType.slowDownSelf: //ok
                if (ph.faster)
                    ph.SetNormalSpeed();
                else
                    ph.slowDownCO = ph.StartCoroutine(ph.SlowDown());
                break;

            case ItemStats.ConsumableType.speedUpSelf: //ok
                if (ph.slower)
                    ph.SetNormalSpeed();
                else
                    ph.speedUpCO = ph.StartCoroutine(ph.SpeedUp());
                break;

            case ItemStats.ConsumableType.doubleDamage: //ok
                //non posso duplicare più volte il danno
                if (!ph.dd)
                {
                    if (ph.hd)
                        ph.SetNormalDamage();
                    else
                        ph.ddCO = ph.StartCoroutine(ph.DoubleDamage());
                }
                break;

            case ItemStats.ConsumableType.halfDamage: //ok
                if (!ph.hd)
                {
                    if (ph.dd)
                        ph.SetNormalDamage();
                    else
                        ph.hdCO = ph.StartCoroutine(ph.HalfDamage());
                }
                break;

            case ItemStats.ConsumableType.flipAttack: //ok
                ph.flipAttCO = ph.StartCoroutine(ph.FlipAttack());
                break;
        }
    }

}
