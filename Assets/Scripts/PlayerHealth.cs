using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    private int startingHealth = 100;
    private int currentHealth;
    private float timer = 0;
    private float invincibilityTime = 3f; //periodo di invulnerabilità dopo aver ricevuto danno
    private PlayerController pc;




	// Use this for initialization
	void Start ()
    {
        pc = GetComponent<PlayerController>();
        currentHealth = startingHealth;	
	}
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
	}

    //i nemici possono danneggiare nuovamente il player solo dopo un certo periodo
    public void TakeDamage(int amount)
    {
        if (timer > invincibilityTime)
        {
            timer = 0;
            currentHealth -= amount; 
            if (currentHealth <= 0)
                PlayerDeath();
        }
        
    }

    private void PlayerDeath()
    {
        pc.enabled = false;
    }
}
