using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootMultiple : MonoBehaviour {

    public float shotSpeed, fireRate, damage, range, distance;
    private float counter;
    private EnemyBullet enemyBullet;
    private Transform playerTransform;
    public GameObject bulletPrefab;
    private Rigidbody2D rb;
    public Sprite sprite;
    private RaycastHit2D hit;
    private Room actualRoom;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        counter = fireRate;
    }


    void Update()
    {
        if (Vector3.Distance(playerTransform.position, transform.position) <= distance)
        {
            if (counter >= fireRate)
            {

                Vector3 centerPoint = transform.position + new Vector3(0, GetComponent<EnemyController>().RealOffset, 0);
                Vector3 direction1 = playerTransform.position - centerPoint;
                Vector3 shotStartPoint = centerPoint + direction1.normalized / 2;

                
                Vector3 direction2 = Quaternion.AngleAxis(-30, Vector3.forward) * direction1;
                Vector3 direction3 = Quaternion.AngleAxis(30, Vector3.forward) * direction1;
                

                //Debug.DrawRay(shotStartPoint, direction, Color.white);

                hit = Physics2D.Raycast(shotStartPoint, direction1, range, ~LayerMask.GetMask("Enemy"));
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        GameObject bullet1 = Instantiate(bulletPrefab, shotStartPoint, Quaternion.identity) as GameObject;
                        bullet1.GetComponent<EnemyBullet>().SetStats(damage, range, sprite, transform.position);
                        bullet1.GetComponent<Rigidbody2D>().velocity = direction1.normalized * shotSpeed;

                        GameObject bullet2 = Instantiate(bulletPrefab, shotStartPoint, Quaternion.identity) as GameObject;
                        bullet2.GetComponent<EnemyBullet>().SetStats(damage, range, sprite, transform.position);
                        bullet2.GetComponent<Rigidbody2D>().velocity = direction2.normalized * shotSpeed;

                        GameObject bullet3 = Instantiate(bulletPrefab, shotStartPoint, Quaternion.identity) as GameObject;
                        bullet3.GetComponent<EnemyBullet>().SetStats(damage, range, sprite, transform.position);
                        bullet3.GetComponent<Rigidbody2D>().velocity = direction3.normalized * shotSpeed;

                        actualRoom = GameManager.manager.ActualRoom;
                        actualRoom.toSort.Add(bullet1);
                        actualRoom.toSort.Add(bullet2);
                        actualRoom.toSort.Add(bullet3);

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
