using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShootCircle : ShootAbstract
{
	//elenco delle direzioni in cui sparerà ciclicamente il nemico
	private Vector3[] directions = new Vector3[]
    {
        new Vector3(1,0,0), new Vector3(1,1,0), new Vector3(0,1,0), new Vector3(-1,1,0),
        new Vector3(-1,0,0), new Vector3(-1,-1,0), new Vector3(0,-1,0), new Vector3(1,-1,0)
    };

	private static System.Random rnd = new System.Random((int)DateTime.Now.Ticks);
	public float shotSpeed, fireRate, range; //caratteristiche dei proiettili specifiche di questo script
	private float damage; //danno del proiettile
	private float counter; //timer per l'implementazione del fire rate nemico
	private EnemyBullet enemyBullet; //script per il settaggio del proiettile
	public GameObject bulletPrefab; //prefab del proiettile
	private Rigidbody2D rb;
    private int index; //serve a determinare in quale direzione sparare a un dato momento
    private Room actualRoom;

    void Start ()
    {
		sprite = GetComponent<EnemyHealth>().bulletSprite;
		damage = GetComponent<EnemyHealth>().bulletDamage;
		counter = fireRate;
        index = rnd.Next(directions.Length); //imposto casualmente la prima direzione del proiettile

	}
	
	void Update ()
    {
		//se il nemico può nuovamente attaccare
		if (counter >= fireRate)
        {
			//calcolo punto di partenza e direzione del proiettile
			GameObject bullet = Instantiate(bulletPrefab, transform.position + new Vector3(0, GetComponent<EnemyController>().RealOffset, 0) + directions[index].normalized / 2, Quaternion.identity) as GameObject;
			
			//imposto le caratteristiche del proiettile
			rb = bullet.GetComponent<Rigidbody2D>();
			rb.velocity = directions[index].normalized * shotSpeed;
			enemyBullet = bullet.GetComponent<EnemyBullet>();
			enemyBullet.SetStats(damage, range, sprite, transform.position, GetComponent<EnemyController>().flying);

            actualRoom = GameManager.manager.ActualRoom;
            actualRoom.toSort.Add(bullet);

            counter = 0; //azzero il counter per il prossimo proiettile

			//faccio ciclare le direzioni continuamente
			index++;
            index %= directions.Length;
        }

        counter += Time.deltaTime;
    }

	//Modifica la velocità dell'attacco in caso di status alterato (slowDown o speedUp)
	public void SetShotSpeed(float amount)
    {
        shotSpeed += amount;
    }
}
