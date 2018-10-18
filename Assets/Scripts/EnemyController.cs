using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyController : SceneObject
{

    private SpriteRenderer playerRend, enemyRend;
    private GameObject player;
    private AStarAI astar;

    public float speed = 1.0F; //velocità in unità al secondo

    void Start()
    {
        gameObject.tag = "Enemy";
        player = GameObject.FindGameObjectWithTag("Player");
        enemyRend = GetComponent<SpriteRenderer>();
        playerRend = player.GetComponent<SpriteRenderer>();

        astar = GetComponent<AStarAI>();

        //se c'è il componente AStarAI imposto lo script per il pathfinding 
        if (astar != null)
        {
            astar.speed = speed;
            astar.startPathOffset = RealOffset;
            astar.targetPosition = player.transform;
        }

    }

    // Update is called once per frame
    void Update()
    {
        //se AStarAI non è presente (ad esempio per i nemici volanti) gestisco diversamente lo spostamento
        if (astar == null)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }
    }

}
