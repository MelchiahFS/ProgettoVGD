using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShootMultiple : ShootAbstract
{
	public float shotSpeed, fireRate, range, distance;
	private float damage;
	private float counter;
    private EnemyBullet enemyBullet;
    private Transform playerTransform;
    public GameObject bulletPrefab;
    private Rigidbody2D rb;
    private RaycastHit2D hit;
    private Room actualRoom;
	private Vector3 centerPoint, direction1, direction2, direction3, shotStartPoint;

	void Start()
    {
		playerTransform = GameManager.manager.playerReference.transform;
		sprite = GetComponent<EnemyHealth>().bulletSprite;
		damage = GetComponent<EnemyHealth>().bulletDamage;
		counter = fireRate;

	}


    void Update()
    {
        if (Vector3.Distance(playerTransform.position, transform.position) <= distance)
        {
            if (counter >= fireRate)
            {

                centerPoint = transform.position + new Vector3(0, GetComponent<EnemyController>().RealOffset, 0);
                direction1 = playerTransform.position - centerPoint;
                shotStartPoint = centerPoint + direction1.normalized / 2;

                
                direction2 = Quaternion.AngleAxis(-30, Vector3.forward) * direction1;
                direction3 = Quaternion.AngleAxis(30, Vector3.forward) * direction1;                

                hit = Physics2D.Raycast(centerPoint, direction1, range, ~LayerMask.GetMask("Enemy"));
                if ((hit.collider != null && hit.collider.gameObject.tag == "Player") || GetComponent<EnemyController>().flying)
                {
                    GameObject bullet1 = Instantiate(bulletPrefab, shotStartPoint, Quaternion.identity) as GameObject;
                    bullet1.GetComponent<EnemyBullet>().SetStats(damage, range, sprite, transform.position, GetComponent<EnemyController>().flying);
                    bullet1.GetComponent<Rigidbody2D>().velocity = direction1.normalized * shotSpeed;

                    GameObject bullet2 = Instantiate(bulletPrefab, shotStartPoint, Quaternion.identity) as GameObject;
                    bullet2.GetComponent<EnemyBullet>().SetStats(damage, range, sprite, transform.position, GetComponent<EnemyController>().flying);
                    bullet2.GetComponent<Rigidbody2D>().velocity = direction2.normalized * shotSpeed;

                    GameObject bullet3 = Instantiate(bulletPrefab, shotStartPoint, Quaternion.identity) as GameObject;
                    bullet3.GetComponent<EnemyBullet>().SetStats(damage, range, sprite, transform.position, GetComponent<EnemyController>().flying);
                    bullet3.GetComponent<Rigidbody2D>().velocity = direction3.normalized * shotSpeed;

                    actualRoom = GameManager.manager.ActualRoom;
                    actualRoom.toSort.Add(bullet1);
                    actualRoom.toSort.Add(bullet2);
                    actualRoom.toSort.Add(bullet3);

                    counter = 0;
                }
            }
        }

        counter += Time.deltaTime;
    }

    public void SetShotSpeed(float amount)
    {
        shotSpeed += amount;
    }
}
