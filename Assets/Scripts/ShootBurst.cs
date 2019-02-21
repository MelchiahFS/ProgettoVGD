using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShootBurst : ShootAbstract
{
	public float shotSpeed, timeBetweenBursts, bulletNumber, fireRate, range, distance;
	private float damage;
	private float counter, burstCounter;
    private EnemyBullet enemyBullet;
    private Transform playerTransform;
    public GameObject bulletPrefab;
    private Rigidbody2D rb;
    private RaycastHit2D hit;
    private Room actualRoom;
    private bool shooting;
	private Vector3 centerPoint, direction, shotStartPoint;


	void Start()
    {
		playerTransform = GameManager.manager.playerReference.transform;
		sprite = GetComponent<EnemyHealth>().bulletSprite;
		damage = GetComponent<EnemyHealth>().bulletDamage;
		burstCounter = timeBetweenBursts;
	}


    void Update()
    {
        if (Vector3.Distance(playerTransform.position, transform.position) <= distance)
        {
            if (burstCounter >= timeBetweenBursts && !shooting)
            {

                centerPoint = transform.position + new Vector3(0, GetComponent<EnemyController>().RealOffset, 0);
                direction = playerTransform.position - centerPoint;
                shotStartPoint = centerPoint + direction.normalized / 2;

                hit = Physics2D.Raycast(centerPoint, direction, range, ~LayerMask.GetMask("Enemy"));
                if ((hit.collider != null && hit.collider.gameObject.tag == "Player") || GetComponent<EnemyController>().flying)
                {
                    StartCoroutine(Burst());
                }
            }
        }
        if (!shooting)
        {
            burstCounter += Time.deltaTime;
        }
    }

    private IEnumerator Burst()
    {
        shooting = true;

        for (int i = 0; i < bulletNumber; i++)
        {
            centerPoint = transform.position + new Vector3(0, GetComponent<EnemyController>().RealOffset, 0);
            direction = playerTransform.position - centerPoint;
            shotStartPoint = centerPoint + direction.normalized / 2;

            GameObject bullet = Instantiate(bulletPrefab, shotStartPoint, Quaternion.identity) as GameObject;
            bullet.GetComponent<EnemyBullet>().SetStats(damage, range, sprite, transform.position, GetComponent<EnemyController>().flying);
            bullet.GetComponent<Rigidbody2D>().velocity = direction.normalized * shotSpeed;

            actualRoom = GameManager.manager.ActualRoom;
            actualRoom.toSort.Add(bullet);

            yield return new WaitForSeconds(fireRate);    
        }

        burstCounter = 0;
        shooting = false;
        yield break;
    }

    public void SetShotSpeed(float amount)
    {
        shotSpeed += amount;
    }
}
