using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    public int damage = 10;
    private int startingHealth = 50;
    private int currentHealth;
    private GameObject player;
    private PlayerHealth playerHealth;
	
        
    // Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.tag == "playerHitbox")
        {
            playerHealth.TakeDamage(damage);
        }
    }
}
