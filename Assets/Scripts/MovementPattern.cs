using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MovementPattern : MonoBehaviour
{
    private GameObject player;
    private EnemyController controller;
    public Room actualRoom;
    public AStarAI astar;
    private System.Random rnd = new System.Random((int)DateTime.Now.Ticks);

	private float timeCounter = 0;
    public float changeTargetTime; //il periodo di tempo dopo cui il nemico cambia direzione
    public float chargeTime; //tempo di caricamento della carica verso il nemico
    public float playerDistance; //la distanza entro il quale il nemico inizia a seguire il player
    public float speed; //velocità in unità al secondo
    private float customSpeed;

	private bool lockPlayer = false; //indica se il player è stato agganciato dal nemico (hybridWandering)
	private bool chargingPlayer = false; //indica se il nemico sta caricando verso il player (charging)
	private bool running = false; //indica se il nemico è partito in carica verso il player (charging)
	private bool followingPlayer = false; //indica se il nemico segue il player (following - hybridWandering)
	private bool followingTarget = false; //indica se il nemico si muove verso una posizione statica (following - hybridWandering)
	private bool bounceDir = false; //indica se il nemico ha scelto una direzione di movimento (bouncing)
    private bool u = false, d = false, l = false, r = false; //indicano quale lato ha colpito il nemico: u = up, d = down... (bouncing)
    private Vector3 playerPosition; 
	private Vector3 randomPosition; //posizione casuale seguita dal nemico (following - hybridWandering)
	private Vector3 straightLine; //la direzione in linea retta verso il player che seguirà il nemico (charging)
    Vector3 randDir; //conterrà la direzione iniziale verso cui si muoverà il nemico rimbalzante (bouncing)
	//possibili direzioni iniziali del nemico rimbalzante (bouncing)
    Vector3[] dirs = new Vector3[] { new Vector3(1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), }; 
    RaycastHit2D hitLeft, hitRight, hitUp, hitDown; //risultati dei raycast per le collisioni dei nemici rimbalzanti (bouncing)
    
    
    
    // Use this for initialization
    void Start ()
    {
		player = GameManager.manager.playerReference;
        actualRoom = GameManager.manager.ActualRoom;
        randomPosition = actualRoom.freePositions[rnd.Next(actualRoom.freePositions.Count)];
        controller = GetComponent<EnemyController>();
    }

    //Usata dai nemici che volano per raggiungere un target (statico o no)
    public void Follow(Vector3 pos)
    {
        if (running)
            customSpeed = speed * 3;            
        else
            customSpeed = speed;

        transform.position = Vector2.MoveTowards(transform.position, pos, customSpeed * Time.deltaTime);
    }

    //Usata dai nemici che non volano per seguire un target statico
    //isTransform viene usato da AStarAI per determinare se seguire il player o una posizione statica
    public void FollowAI(Vector2 pos, bool isTransform)
    {
        if (!followingTarget)
        {
            astar.SetSpeed(speed);
            astar.SetStartPathOffset(controller.RealOffset);
            astar.SetTarget(isTransform, pos);
            followingTarget = true;
        }
        
    }

    //Usata dai nemici che non volano per seguire il player
    //isTransform viene usato da AStarAI per determinare se seguire il player o una posizione statica
    public void FollowAI(Transform pos, bool isTransform)
    {
        if (!followingPlayer)
        {
            astar.SetSpeed(speed);
            astar.SetStartPathOffset(controller.RealOffset);
            astar.SetTarget(isTransform, pos);
            followingPlayer = true;
        }
        
    }

    //Genera un movimento casuale dei nemici
    public void Wander(bool flying)
    {
        timeCounter += Time.deltaTime;

        //se è scaduto il tempo di cambio posizione oppure ho raggiunto la destinazione, scelgo una nuova destinazione
        if (timeCounter >= changeTargetTime || astar.reachedEndOfPath || Vector3.Distance(transform.position, randomPosition) == 0)
        {
            followingTarget = false;
            randomPosition = actualRoom.freePositions[rnd.Next(actualRoom.freePositions.Count)];

            timeCounter = 0;
        }

		//seguo la nuova posizione
        if (flying)
            Follow(randomPosition);
        else
            FollowAI(randomPosition, false);
    }

    //Genera un movimento casuale dei nemici, ma se il player entra nel loro range, questi lo inseguono
    public void WanderAndLock(bool flying)
    {
        if (!lockPlayer)
        {
			//se il nemico non è agganciato vado verso una posizione casuale
			Wander(flying);

            //se entro nel raggio d'azione del player, inseguo il player
            if (Vector3.Distance(transform.position, player.transform.position) <= playerDistance)
            {
                lockPlayer = true;
                timeCounter = changeTargetTime; //se il nemico si allontana di nuovo dal player sceglierà subito una direzione casuale
            }
        }

        if (lockPlayer)
        {
			//se una volta agganciato, il player si allontana del doppio del range, lo perdo di vista e seguo una direzione random
            if (Vector3.Distance(transform.position, player.transform.position) > playerDistance * 2)
            {
                lockPlayer = false;
                followingPlayer = false;
            }
			//altrimenti seguo il player
            else
            {
                if (flying)
                {
                    Follow(player.transform.position);
                }
                    
                else
                {
                    FollowAI(player.transform, true);
                }                   
            }
        }
    }

    //I nemici si avvicinano al player e raggiunta una certa distanza, lo caricano in linea retta
    public void Charge()
    {
        if (!chargingPlayer)
        {
            //se entro nel raggio d'azione del player, carico il player
            if (Vector3.Distance(transform.position, player.transform.position) <= playerDistance)
            {
                chargingPlayer = true; 
            }
            //altrimenti mi avvicino normalmente
            else
            {
                Follow(player.transform.position);
            }
        }

        if (chargingPlayer)
        {
            timeCounter += Time.deltaTime;
            if (!running)
            {   
                //se ho finito il tempo di caricamento, parto in carica verso il player
                if (timeCounter >= chargeTime)
                {
                    straightLine = transform.position + (player.transform.position - transform.position) * 20;
                    running = true;
                }             
            }
            
            if (running)
            {
                Follow(straightLine);
            }
        }
    }

	//Fa muovere in nemici in obliquo e li fa rimbalzare sui muri
    public void Bounce()
    {
		//scelgo la direzione iniziale
        if (!bounceDir)
        {
            randDir = dirs[rnd.Next(dirs.Length)];
            bounceDir = true;
        }
		//controllo quale muro ha colpito e modifico la direzione di conseguenza
        if (u || d)
        {
            randDir.y = -randDir.y;
            u = false;
            d = false;
        }
        else if (l || r)
        {
            randDir.x = -randDir.x;
            l = false;
            r = false;
        }
        GetComponent<Rigidbody2D>().velocity = randDir * speed;
    }

    
    void OnCollisionEnter2D(Collision2D collision)
    {
		//permette al RagingSkull di terminare la sua carica solo quando sbatte col muro o le porte
		if (controller.movementType == EnemyController.MovementType.charging)
        {
            if ((collision.gameObject.layer == LayerMask.NameToLayer("Walls")) ||
            (collision.gameObject.layer == LayerMask.NameToLayer("InnerWalls")) ||
            (collision.gameObject.layer == LayerMask.NameToLayer("Doors")))
            {
                running = false;
                chargingPlayer = false;
                timeCounter = 0;
            }
        }

		//ad ogni collisione spara un raycast dal nemico verso le quattro direzioni per capire quale lato ha toccato
        else if (controller.movementType == EnemyController.MovementType.bouncing)
        {
            hitUp = Physics2D.Raycast(transform.position + new Vector3(0, GetComponent<Character>().RealOffset, 0), Vector3.up);
            hitDown = Physics2D.Raycast(transform.position + new Vector3(0, GetComponent<Character>().RealOffset, 0), Vector3.down);
            hitLeft = Physics2D.Raycast(transform.position + new Vector3(0, GetComponent<Character>().RealOffset, 0), Vector3.left);
            hitRight = Physics2D.Raycast(transform.position + new Vector3(0, GetComponent<Character>().RealOffset, 0), Vector3.right);

            //controllo la distanza minore per determinare quale lato è stato colpito
            if (hitUp.distance < hitDown.distance)
            {
                if (hitUp.distance < hitLeft.distance)
                {
                    if (hitUp.distance <= hitRight.distance)
                        u = true;
                    else
                        r = true;
                }
                else
                {
                    if (hitLeft.distance <= hitRight.distance)
                        l = true;
                    else
                        r = true;
                }
            }
            else
            {
                if (hitDown.distance < hitLeft.distance)
                {
                    if (hitDown.distance <= hitRight.distance)
                        d = true;
                    else
                        r = true;
                }
                else
                {
                    if (hitLeft.distance <= hitRight.distance)
                        l = true;
                    else
                        r = true;
                }
            }
        }
    }

	
}
