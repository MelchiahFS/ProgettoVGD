using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class LootGenerator : MonoBehaviour {

    private static System.Random rnd = new System.Random((int)DateTime.Now.Ticks);
    public float minMeeleDmg, maxMeeleDmg, minRangedDmg, maxRangedDmg, minRng, maxRng, minFR, maxFR, minSP, maxSP; //minimo e massimo danno (per meele e ranged), range, fire rate e shot speed
    private Array enumValues;
    public List<Item> consumablesSO;
    public List<ItemStats> consumables;
    public GameObject sceneItemPrefab;
    private SceneItem si;
    private ItemSpriteSelector select;
    private SpriteRenderer render;
    private List<ItemStats> items = new List<ItemStats>();
    private ItemStats actualItem;
    public Room actualRoom;
    private int ind = 0;

   
    void Start()
    {
        select = GetComponent<ItemSpriteSelector>();
        consumables = new List<ItemStats>();
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
        foreach (ItemStats i in consumables)
        {
            switch (i.consumableType)
            {
                //case ItemStats.ConsumableType.healthUp25: //ok
                //    items.Add(i);
                //    break;

                //case ItemStats.ConsumableType.healthUp50: //ok
                //    items.Add(i);
                //    break;

                //case ItemStats.ConsumableType.slowDownAll: //ok
                //    items.Add(i);
                //    break;

                //case ItemStats.ConsumableType.slowDownSelf: //ok
                //    items.Add(i);
                //    break;

                case ItemStats.ConsumableType.poisonAll: //ok
                    items.Add(i);
                    break;

                    //case ItemStats.ConsumableType.poisonSelf: //ok
                    //    items.Add(i);
                    //    break;

                    //case ItemStats.ConsumableType.damageAll: //ok
                    //    items.Add(i);
                    //    break;

                    //case ItemStats.ConsumableType.damageSelf: //ok
                    //    items.Add(i);
                    //    break;

                    //case ItemStats.ConsumableType.flipMovement: //ok
                    //    items.Add(i);
                    //    break;

                    //case ItemStats.ConsumableType.invincible: //ok
                    //    items.Add(i);
                    //    break;

                    //case ItemStats.ConsumableType.speedUpSelf: //ok
                    //    items.Add(i);
                    //    break;

                    //case ItemStats.ConsumableType.speedUpAll:
                    //    items.Add(i);
                    //    break;
            }
        }

        actualRoom = GameManager.manager.ActualRoom;
    }

    void Update()
    {

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            if (items.Count > 0 && items[ind % items.Count] != null)
            {
                actualItem = items[ind % items.Count];
                Debug.Log(actualItem.consumableType.ToString());
                GetComponentInParent<PlayerHealth>().ApplyEffect(actualItem.consumableType);
                items.Remove(items[ind % items.Count]);
            }
            
        }

        //se la stanza non ha nemici e la ricompensa non è ancora stata generata
        if (!actualRoom.hasGenReward && actualRoom.enemyCounter == 0)
        {
            //e se il player è effettivamente dentro la stanza allora genero una ricompensa
            if (transform.position.x > actualRoom.gridPos.x && transform.position.x < (actualRoom.gridPos.x + GameManager.manager.lvlManager.roomSizeX)
                && transform.position.y > actualRoom.gridPos.y && transform.position.y < (actualRoom.gridPos.y + GameManager.manager.lvlManager.roomSizeY))
            {
                //ho il 50% di ottenere o no una ricompensa
                if (rnd.Next(2) == 1)
                {
                    int seed = rnd.Next(100);
                    if (seed < 30)
                        InstantiateWeapon(actualRoom.freePositions[rnd.Next(actualRoom.freePositions.Count)]);
                    else
                        InstantiateConsumable(actualRoom.freePositions[rnd.Next(actualRoom.freePositions.Count)]);
                }
                actualRoom.hasGenReward = true;
            }   
        }
    }

    public void InstantiateConsumable(Vector3 pos)
    {
        GameObject cons = Instantiate(sceneItemPrefab, pos, Quaternion.identity) as GameObject;
        si = cons.GetComponent<SceneItem>();
        si.Info = consumables[rnd.Next(consumables.Count)];
        render = cons.GetComponent<SpriteRenderer>();
        render.sprite = si.Info.sprite;

        //imposto i consumable come trasparenti, per poi fare un effetto di fade-in quando verranno attivati 
        Color color = render.color;
        color.a = 0;
        render.color = color;

        StartCoroutine(GameManager.manager.lvlManager.FadeIn(render, 0.3f));
    }

    public void InstantiateWeapon(Vector3 pos)
    {
        GameObject weapon = Instantiate(sceneItemPrefab, pos, Quaternion.identity) as GameObject;
        si = weapon.GetComponent<SceneItem>();
        render = weapon.GetComponent<SpriteRenderer>();
        SetWeaponStats(si);

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

}
