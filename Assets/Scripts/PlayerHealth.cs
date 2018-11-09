using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    private int startingHealth = 100;
    public int currentHealth;
    private float invTimer = 0;
    private float invincibilityTime = 2f; //periodo di invulnerabilità dopo aver ricevuto danno
    private Animator animator;
    Collider2D[] hitObjects;
    public bool isDead = false;
    private Slider slider;
    private Color playerColor;



    // Use this for initialization
    void Start ()
    {       
        animator = GetComponent<Animator>();
        currentHealth = startingHealth;
        slider = GetComponentInChildren<Slider>();
        slider.minValue = 0;
        slider.maxValue = startingHealth;
        slider.value = startingHealth;
        currentHealth = startingHealth;

        //actualWeapon = GetComponentInChildren<Weapon>();
        playerColor = GetComponent<SpriteRenderer>().color;
    }

    // Update is called once per frame
    void Update()
    {
        invTimer += Time.deltaTime;       
    }

    //i nemici possono danneggiare nuovamente il player solo dopo un certo periodo
    public void TakeDamage(int amount)
    {
        if (invTimer > invincibilityTime)
        {
            invTimer = 0;
            slider.value -= amount;
            currentHealth -= amount;
            Debug.Log("player health: " + currentHealth);
            if (currentHealth <= 0)
                PlayerDeath();
            else
                StartCoroutine(Flash(playerColor, GetComponent<SpriteRenderer>()));
        }

    }

    private void PlayerDeath()
    {
        animator.SetBool("isDead", true);
        isDead = true;
        this.enabled = false;
        GameManager.manager.Invoke("ReturnToMenu", 2f);
    }

    //crea un effetto flash quando il player viene colpito
    private IEnumerator Flash(Color c1, SpriteRenderer r)
    {
        Color c2 = c1;
        c2.a = 0;
        while (invTimer < invincibilityTime)
        {
            r.color = c2;
            yield return new WaitForSeconds(0.05f);
            r.color = c1;
            yield return new WaitForSeconds(0.05f);
        }
        yield break;
    }
}
