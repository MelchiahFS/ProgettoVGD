using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyController : SceneObject
{

    private SpriteRenderer playerRend, enemyRend;
    private GameObject player;
    private AStarAI astar;
    private float posX, posY;
    private Vector3 lastFramePosition, movement;
    private Animator animator;
    public float speed = 1.0F; //velocità in unità al secondo

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyRend = GetComponent<SpriteRenderer>();
        enemyRend.sortingLayerName = "Characters";
        gameObject.tag = "Enemy";
        playerRend = player.GetComponent<SpriteRenderer>();
        lastFramePosition = transform.position;
        animator = GetComponent<Animator>();
        astar = GetComponent<AStarAI>();
        SetRealOffset(gameObject);
        //se c'è il componente AStarAI imposto lo script per il pathfinding 
        if (astar != null)
        {
            astar.speed = speed;
            astar.startPathOffset = RealOffset;
            Debug.Log("real offset " + RealOffset);
            astar.targetPosition = player.transform;
        }

    }

    // Update is called once per frame
    void Update()
    {
        movement = transform.position - lastFramePosition;

        posX = movement.x;
        posY = movement.y;
        
        animator.SetFloat("PosX", posX);
        animator.SetFloat("PosY", posY);

        lastFramePosition = transform.position;

        //se AStarAI non è presente (ad esempio per i nemici volanti) gestisco diversamente lo spostamento
        if (astar == null)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }

    }

}
