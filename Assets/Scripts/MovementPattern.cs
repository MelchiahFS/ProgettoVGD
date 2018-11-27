using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class MovementPattern : MonoBehaviour {

    //wandering indica un movimento casuale, in cui a ogni intervallo di tempo 
    //prefissato (changeTargetTime) viene cambiata la direzione di movimento verso un punto della stanza scelto a caso
    //-------
    //following e AIfollowing indicano che i nemici seguiranno il player, rispettivamente senza e con il pathfinding A*
    //-------

    private GameObject player;
    private EnemyController controller;
    public Room actualRoom;
    public AStarAI astar;
    private System.Random rnd;
    
    private float timeCounter = 0;
    public float changeTargetTime = 3; //il periodo di tempo dopo cui il nemico cambia direzione
    public float chargeTime = 3; //tempo di caricamento della carica verso il nemico
    public float playerDistance = 5; //la distanza entro il quale il nemico inizia a seguire il player
    public float speed; //velocità in unità al secondo
    private float customSpeed;

    private bool lockPlayer = false, chargingPlayer = false, running = false, followingPlayer = false, followingTarget = false, bounceDir = false;
    private bool u = false, d = false, l = false, r = false;
    private Vector3 playerPosition;
    private Vector3 randomPosition, straightLine;
    Vector3 randDir;
    Vector3[] dirs = new Vector3[] { new Vector3(1, 1, 0), new Vector3(1, -1, 0), new Vector3(-1, -1, 0), new Vector3(-1, 1, 0), };
    RaycastHit2D hitLeft, hitRight, hitUp, hitDown;
    
    
    
    // Use this for initialization
    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        actualRoom = GameManager.manager.ActualRoom;
        rnd = new System.Random((int)DateTime.Now.Ticks);
        randomPosition = actualRoom.freePositions[rnd.Next(actualRoom.freePositions.Count)];
        controller = GetComponent<EnemyController>();
    }

    //Usata dai nemici che volano per raggiungere il target
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

    //Usata dai nemici che non volano per seguire un target in movimento
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

        //se è scaduto il tempo di cambio posizione oppure ho raggiunto la posizione, scelgo una nuova posizione
        if (timeCounter >= changeTargetTime || astar.reachedEndOfPath || Vector3.Distance(transform.position, randomPosition) == 0)
        {
            followingTarget = false;
            randomPosition = actualRoom.freePositions[rnd.Next(actualRoom.freePositions.Count)];

            timeCounter = 0;
        }

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
            if (Vector3.Distance(transform.position, player.transform.position) > playerDistance * 3)
            {
                lockPlayer = false;
                followingPlayer = false;
            }
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

    public void Bounce()
    {
        if (!bounceDir)
        {
            randDir = dirs[rnd.Next(dirs.Length)];
            bounceDir = true;
        }
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

    //permette al RagingSkull di terminare la sua carica solo quando sbatte col muro o le porte
    void OnCollisionEnter2D(Collision2D collision)
    {
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
        else if (controller.movementType == EnemyController.MovementType.bouncing)
        {
            hitUp = Physics2D.Raycast(transform.position + new Vector3(0, GetComponent<Character>().RealOffset, 0), Vector3.up);
            hitDown = Physics2D.Raycast(transform.position + new Vector3(0, GetComponent<Character>().RealOffset, 0), Vector3.down);
            hitLeft = Physics2D.Raycast(transform.position + new Vector3(0, GetComponent<Character>().RealOffset, 0), Vector3.left);
            hitRight = Physics2D.Raycast(transform.position + new Vector3(0, GetComponent<Character>().RealOffset, 0), Vector3.right);

            
            if (hitUp.distance < hitDown.distance)
            {
                if (hitUp.distance < hitLeft.distance)
                {
                    if (hitUp.distance <= hitRight.distance)
                    {
                        //Debug.Log("su " + hitUp.collider.tag);
                        u = true;
                    }
                    else
                    {
                        //Debug.Log("destra " + hitRight.collider.tag);
                        r = true;
                    }
                }
                else
                {
                    if (hitLeft.distance <= hitRight.distance)
                    {
                        //Debug.Log("sinistra " + hitLeft.collider.tag);
                        l = true;
                    }
                    else
                    {
                        //Debug.Log("destra " + hitRight.collider.tag);
                        r = true;
                    }
                }
            }
            else
            {
                if (hitDown.distance < hitLeft.distance)
                {
                    if (hitDown.distance <= hitRight.distance)
                    {
                        //Debug.Log("giu " + hitDown.collider.tag);
                        d = true;
                    }
                    else
                    {
                        //Debug.Log("destra " + hitRight.collider.tag);
                        r = true;
                    }
                }
                else
                {
                    if (hitLeft.distance <= hitRight.distance)
                    {
                        //Debug.Log("sinistra " + hitLeft.collider.tag);
                        l = true;
                    }
                    else
                    {
                        //Debug.Log("destra " + hitRight.collider.tag);
                        r = true;
                    }
                }
            }
             
        }
    }

	
}
