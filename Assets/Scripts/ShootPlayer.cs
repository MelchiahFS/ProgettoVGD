using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShootPlayer : ShootAbstract
{
	public float shotSpeed, fireRate, range; //caratteristiche dei proiettili specifiche di questo script
	public float distance; //indica la distanza dal player entro cui il nemico inizia a sparare
	private float damage; //danno del proiettile
	private float counter; //timer per l'implementazione del fire rate nemico
    private EnemyBullet enemyBullet; //script per il settaggio del proiettile
    private Transform playerTransform; //posizione del player
    public GameObject bulletPrefab; //prefab del proiettile
    private Rigidbody2D rb;
    private RaycastHit2D hit; //risultato del raycast verso il player
    private Room actualRoom;

	//vettori utili al calcolo del punto di partenza del proiettile e la sua direzione
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
		//se il nemico ha agganciato il player
		if (Vector3.Distance(playerTransform.position, transform.position) <= distance)
        {
			//se può nuovamente attaccare
            if (counter >= fireRate)
            {
				//calcolo punto di partenza e direzione del proiettile
                centerPoint = transform.position + new Vector3(0, GetComponent<EnemyController>().RealOffset, 0);
                direction = playerTransform.position - centerPoint;
                shotStartPoint = centerPoint + direction.normalized / 2;

				//controllo se tra player e nemico non ci siano ostacoli
				hit = Physics2D.Raycast(centerPoint, direction, range, ~LayerMask.GetMask("Enemy"));

				//se non ho trovato ostacoli (o se il nemico vola, in tal caso gli ostacoli saranno sotto di lui) allora sparo un proiettile
				if ((hit.collider != null && hit.collider.gameObject.tag == "Player") || GetComponent<EnemyController>().flying)
                {
                    GameObject bullet = Instantiate(bulletPrefab, shotStartPoint, Quaternion.identity) as GameObject;
                    rb = bullet.GetComponent<Rigidbody2D>();
                    rb.velocity = direction.normalized * shotSpeed;
                    enemyBullet = bullet.GetComponent<EnemyBullet>();
					
					//imposto le caratteristiche del proiettile
                    enemyBullet.SetStats(damage, range, sprite, transform.position, GetComponent<EnemyController>().flying);

                    actualRoom = GameManager.manager.ActualRoom;
                    actualRoom.toSort.Add(bullet);

                    counter = 0; //azzero il counter per il prossimo proiettile
                }
            }
        }

		//aggiorno il counter per il prossimo attacco
        counter += Time.deltaTime;
	}

	//Disegna una linea verso il player nell'editor (usata per testare l'agganciamento al player)
	public void OnDrawGizmos()
	{
		if (hit.collider != null && hit.collider.gameObject.tag == "Player")
		{
			Gizmos.color = Color.blue;
			Gizmos.DrawLine(shotStartPoint, playerTransform.position);
		}
			
	}

	//Modifica la velocità dell'attacco in caso di status alterato (slowDown o speedUp)
    public void SetShotSpeed(float amount)
    {
        shotSpeed += amount;
    }

}