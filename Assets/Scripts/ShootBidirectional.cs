using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBidirectional : MonoBehaviour {

    public float shotSpeed, fireRate, damage, range;
    private float counter;
    private EnemyBullet enemyBullet;
    private Transform playerTransform;
    public GameObject bulletPrefab;
    private Rigidbody2D rb;
    public Sprite sprite;
    private Room actualRoom;
    private Vector3 lastFramePosition;
    private Vector3 offset;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        offset = new Vector3(0, GetComponent<EnemyController>().RealOffset, 0);
        counter = 0;
        lastFramePosition = transform.position + offset;
    }


    void Update ()
    {
        if (counter >= fireRate)
        {

            Vector3 centerPoint = transform.position + offset;
            Vector3 direction1 = centerPoint - lastFramePosition;
            Vector3 direction2 = -direction1;
            Vector3 shotStartPoint1 = centerPoint + direction1.normalized / 2;
            Vector3 shotStartPoint2 = centerPoint + direction2.normalized / 2;

            
            
            GameObject bullet1 = Instantiate(bulletPrefab, shotStartPoint1, Quaternion.identity) as GameObject;
            bullet1.GetComponent<EnemyBullet>().SetStats(damage, range, sprite, transform.position, GetComponent<EnemyController>().flying);
            bullet1.GetComponent<Rigidbody2D>().velocity = direction1.normalized * shotSpeed;

            GameObject bullet2 = Instantiate(bulletPrefab, shotStartPoint2, Quaternion.identity) as GameObject;
            bullet2.GetComponent<EnemyBullet>().SetStats(damage, range, sprite, transform.position, GetComponent<EnemyController>().flying);
            bullet2.GetComponent<Rigidbody2D>().velocity = direction2.normalized * shotSpeed;

            actualRoom = GameManager.manager.ActualRoom;
            actualRoom.toSort.Add(bullet1);
            actualRoom.toSort.Add(bullet2);

            counter = 0;
                
            
        }
        lastFramePosition = transform.position + offset;
        counter += Time.deltaTime;
    }

    public void SetShotSpeed(float amount)
    {
        shotSpeed += amount;
    }
}
