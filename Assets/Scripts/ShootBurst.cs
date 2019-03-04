using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ShootBurst : ShootAbstract
{
	public float shotSpeed, fireRate, range, distance; //caratteristiche dei proiettili specifiche di questo script
	public float timeBetweenBursts; //tempo tra due raffiche
	public float bulletNumber; //numero di proiettili sparati per raffica
	private float damage; //danno del proiettile
	private float counter, burstCounter; //timer per l'implementazione del fire rate nemico e del tempo tra due raffiche
	private EnemyBullet enemyBullet; //script per il settaggio del proiettile
	private Transform playerTransform;
    public GameObject bulletPrefab; //prefab del proiettile
	private Rigidbody2D rb;
    private RaycastHit2D hit; //risultato del raycast verso il player
	private Room actualRoom;
    private bool shooting; //indica se il nemico sta sparando una raffica

	//vettori utili al calcolo del punto di partenza del proiettile e la sua direzione
	private Vector3 centerPoint, direction, shotStartPoint;


	void Start()
    {
		playerTransform = GameManager.manager.playerReference.transform;
		sprite = GetComponent<EnemyHealth>().bulletSprite;
		damage = GetComponent<EnemyHealth>().bulletDamage;
		burstCounter = timeBetweenBursts;
	}


    void Update()
    {
		//se il nemico ha agganciato il player
		if (Vector3.Distance(playerTransform.position, transform.position) <= distance)
        {
			//se può iniziare una nuova raffica
			if (burstCounter >= timeBetweenBursts && !shooting)
            {
				//calcolo punto di partenza e direzione del proiettile
				centerPoint = transform.position + new Vector3(0, GetComponent<EnemyController>().RealOffset, 0);
                direction = playerTransform.position - centerPoint;
                shotStartPoint = centerPoint + direction.normalized / 2;

				//controllo se tra player e nemico non ci siano ostacoli
				hit = Physics2D.Raycast(centerPoint, direction, range, ~LayerMask.GetMask("Enemy"));

				//se non ho trovato ostacoli (o se il nemico vola, in tal caso gli ostacoli saranno sotto di lui) allora sparo una raffica
				if ((hit.collider != null && hit.collider.gameObject.tag == "Player") || GetComponent<EnemyController>().flying)
                {
                    StartCoroutine(Burst());
                }
            }
        }

		//se non è in corso una raffica aggiorno il tempo per la prossima raffica
        if (!shooting)
        {
            burstCounter += Time.deltaTime;
        }
    }

	//Permette al nemico di sparare una raffica di proiettili
    private IEnumerator Burst()
    {
        shooting = true; //segnalo l'inizio della raffica

		//effettuo un ciclo per ogni proiettile
        for (int i = 0; i < bulletNumber; i++)
        {
            centerPoint = transform.position + new Vector3(0, GetComponent<EnemyController>().RealOffset, 0);
            direction = playerTransform.position - centerPoint;
            shotStartPoint = centerPoint + direction.normalized / 2;

            GameObject bullet = Instantiate(bulletPrefab, shotStartPoint, Quaternion.identity) as GameObject;
            bullet.GetComponent<EnemyBullet>().SetStats(damage, range, sprite, transform.position, GetComponent<EnemyController>().flying);
            bullet.GetComponent<Rigidbody2D>().velocity = direction.normalized * shotSpeed;

            actualRoom = GameManager.manager.ActualRoom;
            actualRoom.toSort.Add(bullet);

			//dopo aver sparato attendo per un istante dato
            yield return new WaitForSeconds(fireRate);    
        }

        burstCounter = 0; //azzero il tempo per la prossima raffica
        shooting = false; //segnalo la fine della raffica
        yield break;
    }

	//Modifica la velocità dell'attacco in caso di status alterato (slowDown o speedUp)
	public void SetShotSpeed(float amount)
    {
        shotSpeed += amount;
    }
}
