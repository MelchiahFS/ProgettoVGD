using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootPlayer : MonoBehaviour {

    public float shotSpeed, fireRate, damage, range, distance;
    private float counter;
    private EnemyBullet enemyBullet;
    private Transform playerTransform;
    public GameObject bulletPrefab;
    private Rigidbody2D rb;
    public Sprite sprite;

	void Start ()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        counter = fireRate;
	}
	

	void Update ()
    {
		if (Vector3.Distance(playerTransform.position, transform.position) <= distance)
        {
            if (counter >= fireRate)
            {
                Vector3 direction = playerTransform.position - transform.position;
                GameObject bullet = Instantiate(bulletPrefab, transform.position + direction.normalized / 2 + new Vector3(0,-0.25f,0), Quaternion.identity) as GameObject;
                rb = bullet.GetComponent<Rigidbody2D>();
                rb.velocity = direction.normalized * shotSpeed;
                enemyBullet = bullet.GetComponent<EnemyBullet>();
                enemyBullet.SetStats(damage, range, sprite, transform.position);

                counter = 0;
            }
        }

        counter += Time.deltaTime;
	}

}
