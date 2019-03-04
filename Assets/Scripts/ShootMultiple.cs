using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShootMultiple : ShootAbstract
{
	public float shotSpeed, fireRate, range, distance; //caratteristiche dei proiettili specifiche di questo script
	private float damage; //danno del proiettile
	private float counter; //timer per l'implementazione del fire rate nemico
	private EnemyBullet enemyBullet; //script per il settaggio del proiettile
	private Transform playerTransform; //posizione del player
	public GameObject bulletPrefab; //prefab del proiettile
	private Rigidbody2D rb;
    private RaycastHit2D hit; //risultato del raycast verso il player
	private Room actualRoom;

	//vettori utili al calcolo del punto di partenza del proiettile e la sua direzione
	private Vector3 centerPoint, direction1, direction2, direction3, shotStartPoint;

	void Start()
    {
		playerTransform = GameManager.manager.playerReference.transform;
		sprite = GetComponent<EnemyHealth>().bulletSprite;
		damage = GetComponent<EnemyHealth>().bulletDamage;
		counter = fireRate;

	}


    void Update()
    {
		//se il nemico ha agganciato il player
		if (Vector3.Distance(playerTransform.position, transform.position) <= distance)
        {
			//se può nuovamente attaccare
			if (counter >= fireRate)
            {
				//calcolo punto di partenza e direzione dei proiettili
				centerPoint = transform.position + new Vector3(0, GetComponent<EnemyController>().RealOffset, 0);
                direction1 = playerTransform.position - centerPoint;
                shotStartPoint = centerPoint + direction1.normalized / 2;
				
                direction2 = Quaternion.AngleAxis(-30, Vector3.forward) * direction1;
                direction3 = Quaternion.AngleAxis(30, Vector3.forward) * direction1;

				//controllo se tra player e nemico non ci siano ostacoli
				hit = Physics2D.Raycast(centerPoint, direction1, range, ~LayerMask.GetMask("Enemy"));

				//se non ho trovato ostacoli (o se il nemico vola, in tal caso gli ostacoli saranno sotto di lui) allora sparo i proiettili
				if ((hit.collider != null && hit.collider.gameObject.tag == "Player") || GetComponent<EnemyController>().flying)
                {
					//istanzio i proiettili e ne imposto le caratteristiche
					GameObject bullet1 = Instantiate(bulletPrefab, shotStartPoint, Quaternion.identity) as GameObject;
                    bullet1.GetComponent<EnemyBullet>().SetStats(damage, range, sprite, transform.position, GetComponent<EnemyController>().flying);
                    bullet1.GetComponent<Rigidbody2D>().velocity = direction1.normalized * shotSpeed;

                    GameObject bullet2 = Instantiate(bulletPrefab, shotStartPoint, Quaternion.identity) as GameObject;
                    bullet2.GetComponent<EnemyBullet>().SetStats(damage, range, sprite, transform.position, GetComponent<EnemyController>().flying);
                    bullet2.GetComponent<Rigidbody2D>().velocity = direction2.normalized * shotSpeed;

                    GameObject bullet3 = Instantiate(bulletPrefab, shotStartPoint, Quaternion.identity) as GameObject;
                    bullet3.GetComponent<EnemyBullet>().SetStats(damage, range, sprite, transform.position, GetComponent<EnemyController>().flying);
                    bullet3.GetComponent<Rigidbody2D>().velocity = direction3.normalized * shotSpeed;

                    actualRoom = GameManager.manager.ActualRoom;
                    actualRoom.toSort.Add(bullet1);
                    actualRoom.toSort.Add(bullet2);
                    actualRoom.toSort.Add(bullet3);

                    counter = 0; //azzero il counter per i prossimi proiettili
				}
            }
        }

        counter += Time.deltaTime;
    }

	//Modifica la velocità dell'attacco in caso di status alterato (slowDown o speedUp)
	public void SetShotSpeed(float amount)
    {
        shotSpeed += amount;
    }
}
