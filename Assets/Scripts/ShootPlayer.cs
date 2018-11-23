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
    private RaycastHit2D hit;
    private Room actualRoom;

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

                Vector3 centerPoint = transform.position + new Vector3(0, GetComponent<EnemyController>().RealOffset, 0);
                Vector3 direction = playerTransform.position - centerPoint;
                Vector3 shotStartPoint = centerPoint + direction.normalized / 2;

                //Debug.DrawRay(shotStartPoint, direction, Color.white);

                hit = Physics2D.Raycast(shotStartPoint, direction, range, ~LayerMask.GetMask("Enemy"));
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        GameObject bullet = Instantiate(bulletPrefab, shotStartPoint, Quaternion.identity) as GameObject;
                        rb = bullet.GetComponent<Rigidbody2D>();
                        rb.velocity = direction.normalized * shotSpeed;
                        enemyBullet = bullet.GetComponent<EnemyBullet>();
                        enemyBullet.SetStats(damage, range, sprite, transform.position);

                        actualRoom = GameManager.manager.ActualRoom;
                        actualRoom.toSort.Add(bullet);

                        counter = 0;
                    }
                    //else if (hit.collider.gameObject.tag == "Enemy")
                    //{
                    //    Debug.Log("yay");
                    //}
                }
                else
                {
                    Debug.Log("boh");
                }
            }
        }

        counter += Time.deltaTime;
	}

}