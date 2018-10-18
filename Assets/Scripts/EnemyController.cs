using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyController : MonoBehaviour {

    private SpriteRenderer playerRend, enemyRend;
    private GameObject player;

    private AStarAI astar;

    public float speed = 1.0F; //velocità in unità al secondo

    void Start ()
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

            foreach (Collider2D collider in GetComponents<Collider2D>())
            {
                //cerco il collider ai piedi del nemico e lo uso come starting point del percorso
                if (!collider.isTrigger)
                {
                    astar.startPathOffset = collider.offset.y * transform.localScale.y;
                    //Debug.Log("offset = " + collider.offset.y);
                    break;
                }
            }
            astar.targetPosition = player.transform;
        }

    }
	
	// Update is called once per frame
	void Update ()
    {
        //se AStarAI non è presente (ad esempio per i nemici volanti) gestisco diversamente lo spostamento
        if (astar == null)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);
        }

        //imposto dinamicamente il sorting layer dei nemici rispetto al player per permettere una corretta renderizzazione quando il nemico è sopra o sotto di esso
        if (player.transform.position.y < gameObject.transform.position.y)
        {
            if (enemyRend.sortingOrder >= playerRend.sortingOrder)
                enemyRend.sortingOrder--;

        }
        else
        {
            if (enemyRend.sortingOrder <= playerRend.sortingOrder)
                enemyRend.sortingOrder++;
        }
	}


}
