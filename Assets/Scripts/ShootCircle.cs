using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShootCircle : ShootAbstract
{

	private Vector3[] directions = new Vector3[]
    {
        new Vector3(1,0,0), new Vector3(1,1,0), new Vector3(0,1,0), new Vector3(-1,1,0),
        new Vector3(-1,0,0), new Vector3(-1,-1,0), new Vector3(0,-1,0), new Vector3(1,-1,0)
    };

    public float shotSpeed, fireRate, range;
	private float damage;
	private float counter;
    private EnemyBullet enemyBullet;
    public GameObject bulletPrefab;
    private Rigidbody2D rb;
    private int index;
    private Room actualRoom;

    void Start ()
    {
		sprite = GetComponent<EnemyHealth>().bulletSprite;
		damage = GetComponent<EnemyHealth>().bulletDamage;
		counter = fireRate;
        index = 0;

	}
	
	void Update ()
    { 
        if (counter >= fireRate)
        {
            GameObject bullet = Instantiate(bulletPrefab, transform.position + new Vector3(0, GetComponent<EnemyController>().RealOffset, 0) + directions[index].normalized / 2, Quaternion.identity) as GameObject;
            rb = bullet.GetComponent<Rigidbody2D>();
            rb.velocity = directions[index].normalized * shotSpeed;
            enemyBullet = bullet.GetComponent<EnemyBullet>();
            enemyBullet.SetStats(damage, range, sprite, transform.position, GetComponent<EnemyController>().flying);

            actualRoom = GameManager.manager.ActualRoom;
            actualRoom.toSort.Add(bullet);

            counter = 0;
            index++;
            index %= directions.Length;
        }

        counter += Time.deltaTime;
    }

    public void SetShotSpeed(float amount)
    {
        shotSpeed += amount;
    }
}
