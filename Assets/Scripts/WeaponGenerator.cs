using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class WeaponGenerator : MonoBehaviour {

    private static System.Random rnd = new System.Random((int)DateTime.Now.Ticks);
    public float minMeeleDmg, maxMeeleDmg, minRangedDmg, maxRangedDmg, minRng, maxRng, minFR, maxFR, minSP, maxSP; //minimo e massimo danno (per meele e ranged), range, fire rate e shot speed
    private Array enumValues;
    private List<Item> consumables;
    public GameObject sceneItemPrefab;
    private SceneItem si;

    private Room actualRoom;

    void Start()
    {
        consumables = new List<Item>();
        consumables.Add(Resources.Load<Item>("Items/Scroll of Health"));
    }

    public void InstantiateWeapon(Vector3 pos)
    {
        GameObject weapon = Instantiate(sceneItemPrefab, pos, Quaternion.identity) as GameObject;
        si = weapon.GetComponent<SceneItem>();
        SetWeaponStats(si);
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
            
            i.Info.damage = rnd.Next((int)minMeeleDmg * 100, (int)maxMeeleDmg * 100) / 100;
        }
        else
        {
            i.Info.sprite = itemSelect.rangedWeapons[rnd.Next(itemSelect.rangedWeapons.Count)];
            i.GetComponent<SpriteRenderer>().sprite = i.Info.sprite;
            i.GetComponent<SpriteRenderer>().sortingLayerName = "Items";

            enumValues = Enum.GetValues(typeof(ItemStats.FireType));
            i.Info.fireType = (ItemStats.FireType)enumValues.GetValue(rnd.Next(enumValues.Length));

            enumValues = Enum.GetValues(typeof(ItemStats.BulletType));
            i.Info.bulletType = (ItemStats.BulletType)enumValues.GetValue(rnd.Next(enumValues.Length));

            i.Info.damage = rnd.Next((int)minRangedDmg * 100, (int)maxRangedDmg * 100) / 100;
            i.Info.range = rnd.Next((int)minRng * 100, (int)maxRng * 100) / 100;
            i.Info.fireRate = rnd.Next((int)minFR * 100, (int)maxFR * 100) / 100;
            i.Info.shotSpeed = rnd.Next((int)minSP * 100, (int)maxSP * 100) / 100;

        }
        
    }

}
