﻿using System.Collections;
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

    private bool lockPlayer = false, chargingPlayer = false, running = false, followingPlayer = false, followingTarget = false;
    private Vector3 playerPosition;
    private Vector3 randomPosition, straightLine, direction;
    private float distance;
    
    
    // Use this for initialization
    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        actualRoom = GameManager.manager.ActualRoom;
        rnd = new System.Random((int)DateTime.Now.Ticks);
        randomPosition = actualRoom.freePositions[rnd.Next(actualRoom.freePositions.Count)];
        controller = GetComponent<EnemyController>();
    }

    public void Follow(Vector3 pos)
    {
        if (running)
            customSpeed = speed * 3;            
        else
            customSpeed = speed;

        transform.position = Vector2.MoveTowards(transform.position, pos, customSpeed * Time.deltaTime);
    }

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

    public void Wander(bool flying)
    {
        timeCounter += Time.deltaTime;

        //se è scaduto il tempo di cambio posizione oppure ho raggiunto la posizione, scelgo una nuova posizione
        if (timeCounter >= changeTargetTime || Vector3.Distance(transform.position, randomPosition) == 0)
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

    public void Charge()
    {
        if (!chargingPlayer)
        {

            //se entro nel raggio d'azione del player, carico il player
            if (Vector3.Distance(transform.position, player.transform.position) <= playerDistance)
            {
                chargingPlayer = true; 
            }
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

    void OnCollisionEnter2D(Collision2D collision)
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

	
}