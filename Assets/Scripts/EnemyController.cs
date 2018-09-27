using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour {

    private SpriteRenderer playerRend, enemyRend;
    private GameObject player;

	
    // Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        enemyRend = GetComponent<SpriteRenderer>();
        playerRend = player.GetComponent<SpriteRenderer>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		if(player.transform.position.y < gameObject.transform.position.y)
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
