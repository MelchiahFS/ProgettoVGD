using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootCircle : MonoBehaviour {

    private Vector3[] directions = new Vector3[]
    {
        new Vector3(1,0,0), new Vector3(1,1,0), new Vector3(0,1,0), new Vector3(-1,1,0),
        new Vector3(-1,0,0), new Vector3(-1,-1,0), new Vector3(0,-1,0), new Vector3(1,-1,0)
    };
    public float shotSpeed, fireRate, damage, range;
    private float counter;
    private EnemyBullet enemyBullet;
    public GameObject bulletPrefab;
    private Rigidbody2D rb;
    public Sprite sprite;
    private int index;
    private Room actualRoom;

    void Start ()
    {
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
}
