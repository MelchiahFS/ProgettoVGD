﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootBurst : MonoBehaviour {

    public float shotSpeed, timeBetweenBursts, bulletNumber, fireRate, damage, range, distance;
    private float counter, burstCounter;
    private EnemyBullet enemyBullet;
    private Transform playerTransform;
    public GameObject bulletPrefab;
    private Rigidbody2D rb;
    public Sprite sprite;
    private RaycastHit2D hit;
    private Room actualRoom;
    private bool shooting;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        burstCounter = timeBetweenBursts;
    }


    void Update()
    {
        if (Vector3.Distance(playerTransform.position, transform.position) <= distance)
        {
            if (burstCounter >= timeBetweenBursts && !shooting)
            {

                Vector3 centerPoint = transform.position + new Vector3(0, GetComponent<EnemyController>().RealOffset, 0);
                Vector3 direction = playerTransform.position - centerPoint;
                Vector3 shotStartPoint = centerPoint + direction.normalized / 2;

                hit = Physics2D.Raycast(shotStartPoint, direction, range, ~LayerMask.GetMask("Enemy"));
                if (hit.collider != null)
                {
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        StartCoroutine(Burst());
                    }
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
            Vector3 centerPoint = transform.position + new Vector3(0, GetComponent<EnemyController>().RealOffset, 0);
            Vector3 direction = playerTransform.position - centerPoint;
            Vector3 shotStartPoint = centerPoint + direction.normalized / 2;

            GameObject bullet = Instantiate(bulletPrefab, shotStartPoint, Quaternion.identity) as GameObject;
            bullet.GetComponent<EnemyBullet>().SetStats(damage, range, sprite, transform.position);
            bullet.GetComponent<Rigidbody2D>().velocity = direction.normalized * shotSpeed;

            actualRoom = GameManager.manager.ActualRoom;
            actualRoom.toSort.Add(bullet);

            yield return new WaitForSeconds(fireRate);    
        }

        burstCounter = 0;
        shooting = false;
        yield break;
    }
}