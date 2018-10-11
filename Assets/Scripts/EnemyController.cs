using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Pathfinding;

public class EnemyController : MonoBehaviour {

    private SpriteRenderer playerRend, enemyRend;
    private GameObject player;
    private AStarAI astar;

    // Movement speed in units/sec.
    public float speed = 1.0F;

    // Time when the movement started.
    private float startTime;

    // Total distance between the markers.
    private float journeyLength;

    // Use this for initialization
    void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyRend = GetComponent<SpriteRenderer>();
        playerRend = player.GetComponent<SpriteRenderer>();

        astar = GetComponent<AStarAI>();

        //se c'è il componente AStarAI imposto lo script per il pathfinding 
        if (astar != null)
        {
            foreach (Collider2D collider in GetComponents<Collider2D>())
            {
                if (!collider.isTrigger)
                {
                    astar.startPathOffset = collider.offset.y * GetComponent<RectTransform>().localScale.y;
                    Debug.Log("offset = " + collider.offset.y);
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
