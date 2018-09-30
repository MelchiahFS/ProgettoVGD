using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealth : MonoBehaviour {

    public int damage = 10;
    private int startingHealth = 50;
    private int currentHealth;
    private GameObject player;
    private PlayerHealth playerHealth;
    private PlayerController playerController;
    public Slider slider;
	
        
    // Use this for initialization
	void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        playerHealth = player.GetComponent<PlayerHealth>();
        //slider = GetComponentInChildren<Slider>();
        slider.minValue = 0;
        slider.maxValue = startingHealth;
        slider.value = startingHealth;
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
        slider.value -= amount;
        StartCoroutine(Flash(gameObject.GetComponent<SpriteRenderer>()));
        Debug.Log("enemy takes damage");
        if (currentHealth <= 0)
        {
            playerHealth.SendMessage("DecreaseEnemyCounter"); //intercettata da PlayerController
            Destroy(gameObject);
        }
            
    }

    //crea un effetto flash quando il player viene colpito
    private IEnumerator Flash(SpriteRenderer r)
    {
        Color c = r.color;
        r.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        r.color = c;
        yield break;
    }
}
