using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour {

    public int damage = 10;
    private int startingHealth = 50;
    private int currentHealth;
    private GameObject player;
    private PlayerHealth playerHealth;
    private PlayerController playerController;
	
        
    // Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();

        currentHealth = startingHealth;
    }
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerHealth.TakeDamage(damage);
        }
    }

    public void TakeDamage(int amount)
    {
        currentHealth -= amount;
        Debug.Log("enemy takes damage");
        if (currentHealth <= 0)
        {
            playerHealth.SendMessage("DecreaseEnemyCounter"); //intercettata da PlayerController
            Destroy(gameObject);
        }
            
    }
}
