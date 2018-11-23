using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AnchorHealthBar))]
public class EnemyHealth : MonoBehaviour {

    public int damage = 10;
    public float startingHealth = 50;
    private float currentHealth;
    private GameObject player;
    private PlayerHealth playerHealth;
    private PlayerController playerController;
    private Slider slider;
        

	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        playerHealth = player.GetComponent<PlayerHealth>();
        slider = GetComponentInChildren<Slider>();
        slider.minValue = 0;
        slider.maxValue = startingHealth;
        slider.value = startingHealth;
        currentHealth = startingHealth;
    }
	

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerHealth.TakeDamage(damage);
        }
    }

    public void TakeDamage(float amount)
    {
        currentHealth -= amount;
        slider.value -= amount;
        StartCoroutine(Flash(gameObject.GetComponent<SpriteRenderer>()));
        Debug.Log("enemy takes damage");
        if (currentHealth <= 0)
        {
            //decrementa il counter dei nemici ancora vivi per la stanza attuale (intercettata da RoomChange)
            playerHealth.SendMessage("DecreaseEnemyCounter"); 

            //elimino il nemico dalla lista dei nemici per la stanza attuale, poi lo elimino dalla scena
            Room actualRoom = GameManager.manager.ActualRoom;
            actualRoom.enemies.Remove(gameObject);
            actualRoom.toSort.Remove(gameObject);
            Destroy(gameObject);
        }
            
    }

    //crea un effetto flash quando il player viene colpito
    private IEnumerator Flash(SpriteRenderer r)
    {
        Color c = r.color;
        r.color = Color.white;
        yield return new WaitForSeconds(0.15f);
        r.color = c;
        yield break;
    }
}
