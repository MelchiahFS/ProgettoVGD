using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShootPlayer : ShootAbstract
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
	private Vector3 centerPoint, direction, shotStartPoint;

	void Start ()
    {

		playerTransform = GameManager.manager.playerReference.transform;
		sprite = GetComponent<EnemyHealth>().bulletSprite;
		damage = GetComponent<EnemyHealth>().bulletDamage;
		counter = fireRate;
	}
	

	void Update ()
    {
		if (Vector3.Distance(playerTransform.position, transform.position) <= distance)
        {
            if (counter >= fireRate)
            {

                centerPoint = transform.position + new Vector3(0, GetComponent<EnemyController>().RealOffset, 0);
                direction = playerTransform.position - centerPoint;
                shotStartPoint = centerPoint + direction.normalized / 2;

				hit = Physics2D.Raycast(centerPoint, direction, range, ~LayerMask.GetMask("Enemy"));
				if ((hit.collider != null && hit.collider.gameObject.tag == "Player") || GetComponent<EnemyController>().flying)
                {
                    GameObject bullet = Instantiate(bulletPrefab, shotStartPoint, Quaternion.identity) as GameObject;
                    rb = bullet.GetComponent<Rigidbody2D>();
                    rb.velocity = direction.normalized * shotSpeed;
                    enemyBullet = bullet.GetComponent<EnemyBullet>();
					
                    enemyBullet.SetStats(damage, range, sprite, transform.position, GetComponent<EnemyController>().flying);

                    actualRoom = GameManager.manager.ActualRoom;
                    actualRoom.toSort.Add(bullet);

                    counter = 0;
                }
            }
        }

        counter += Time.deltaTime;
	}

	public void OnDrawGizmos()
	{
		if (hit.collider != null && hit.collider.gameObject.tag == "Player")
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(shotStartPoint, playerTransform.position);
		}
			
	}

    public void SetShotSpeed(float amount)
    {
        shotSpeed += amount;
    }

}