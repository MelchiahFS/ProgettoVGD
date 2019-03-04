using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShootBidirectional : ShootAbstract
{
	public float shotSpeed, fireRate, range; //caratteristiche dei proiettili specifiche di questo script
	private float damage; //danno del proiettile
	private float counter; //timer per l'implementazione del fire rate nemico
	private EnemyBullet enemyBullet; //script per il settaggio del proiettile
	public GameObject bulletPrefab; //prefab del proiettile
	private Rigidbody2D rb;
    private Room actualRoom;

	//vettori utili al calcolo del punto di partenza del proiettile e la sua direzione
	private Vector3 lastFramePosition, offset;

    void Start()
    {
        offset = new Vector3(0, GetComponent<EnemyController>().RealOffset, 0);
		sprite = GetComponent<EnemyHealth>().bulletSprite;
		damage = GetComponent<EnemyHealth>().bulletDamage;
		counter = 0;
        lastFramePosition = transform.position + offset;

	}


    void Update ()
    {
		//se il nemico può nuovamente attaccare
		if (counter >= fireRate)
        {
			//calcolo punto di partenza e direzione di proiettili
			Vector3 centerPoint = transform.position + offset;
            Vector3 direction1 = centerPoint - lastFramePosition;
            Vector3 direction2 = -direction1;
            Vector3 shotStartPoint1 = centerPoint + direction1.normalized / 2;
            Vector3 shotStartPoint2 = centerPoint + direction2.normalized / 2;

			//istanzio i proiettili e ne imposto le caratteristiche
			GameObject bullet1 = Instantiate(bulletPrefab, shotStartPoint1, Quaternion.identity) as GameObject;
            bullet1.GetComponent<EnemyBullet>().SetStats(damage, range, sprite, transform.position, GetComponent<EnemyController>().flying);
            bullet1.GetComponent<Rigidbody2D>().velocity = direction1.normalized * shotSpeed;

            GameObject bullet2 = Instantiate(bulletPrefab, shotStartPoint2, Quaternion.identity) as GameObject;
            bullet2.GetComponent<EnemyBullet>().SetStats(damage, range, sprite, transform.position, GetComponent<EnemyController>().flying);
            bullet2.GetComponent<Rigidbody2D>().velocity = direction2.normalized * shotSpeed;

            actualRoom = GameManager.manager.ActualRoom;
            actualRoom.toSort.Add(bullet1);
            actualRoom.toSort.Add(bullet2);

            counter = 0; //azzero il counter per i prossimi proiettili


		}
        lastFramePosition = transform.position + offset; //aggiorno l'ultima posizione occupata per determinare la direzione di fuoco
        counter += Time.deltaTime;
    }

	//Modifica la velocità dell'attacco in caso di status alterato (slowDown o speedUp)
	public void SetShotSpeed(float amount)
    {
        shotSpeed += amount;
    }
}
