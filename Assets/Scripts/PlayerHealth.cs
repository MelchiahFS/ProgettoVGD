using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    private int startingHealth = 300;
    public int currentHealth;
    private float invTimer = 0;
    private float invincibilityTime = 2f; //periodo di invulnerabilità dopo aver ricevuto danno
    private Animator animator;
    Collider2D[] hitObjects;
    public bool isDead = false;
    private Slider slider;
    private Color playerColor;
    private PlayerController pc;
    public bool invincible = false, flipMov = false, flipAtt = false;
    private Coroutine speedUpCO, slowDownCO, gddCO, ddCO, invCO, flipMovCO, flipAttCO;



    // Use this for initialization
    void Start ()
    {
        pc = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        currentHealth = startingHealth;
        slider = GetComponentInChildren<Slider>();
        slider.minValue = 0;
        slider.maxValue = startingHealth;
        slider.value = startingHealth;
        currentHealth = startingHealth;
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
        if (!invincible && invTimer > invincibilityTime)
        {
            invTimer = 0;
            slider.value -= amount;
            currentHealth -= amount;
            Debug.Log("player health: " + currentHealth);
            if (currentHealth <= 0)
                PlayerDeath();
            //else
            //    StartCoroutine(Flash(playerColor, GetComponent<SpriteRenderer>()));
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
            yield return new WaitForSeconds(0.04f);
            r.color = c1;
            yield return new WaitForSeconds(0.04f);
        }
        yield break;
    }

    //------------------------------
    //Effetti relativi ai consumables

    public void ApplyEffect(ItemStats.ConsumableType consumable)
    {
        switch (consumable)
        {
            case ItemStats.ConsumableType.healthUp25:
                HealthUp(25);
                break;
            case ItemStats.ConsumableType.healthUp50:
                HealthUp(50);
                break;
            case ItemStats.ConsumableType.slowDownAll:
                break;
            case ItemStats.ConsumableType.slowDownSelf:
                slowDownCO = StartCoroutine(SlowDown());
                break;
            case ItemStats.ConsumableType.poisonAll:
                break;
            case ItemStats.ConsumableType.poisonSelf:
                break;
            case ItemStats.ConsumableType.damageAll:
                break;
            case ItemStats.ConsumableType.damageSelf:
                break;
            case ItemStats.ConsumableType.flipAttack:
                break;
            case ItemStats.ConsumableType.flipMovement:
                flipMovCO = StartCoroutine(FlipMovement());
                break;
            case ItemStats.ConsumableType.invincible:
                invCO = StartCoroutine(Invincible());
                break;
            case ItemStats.ConsumableType.speedUpSelf:
                speedUpCO = StartCoroutine(SpeedUp());
                break;
            case ItemStats.ConsumableType.speedUpAll:
                break;
            case ItemStats.ConsumableType.doubleDamage:
                break;
            case ItemStats.ConsumableType.halfDamage:
                break;
            case ItemStats.ConsumableType.getDoubleDamage:
                break;
        }
    }

    private void HealthUp(int amount)
    {
        currentHealth += amount;
        slider.value = currentHealth;
    }

    private IEnumerator SpeedUp()
    {
        float actualSpeed = pc.speed;
        pc.speed += 2;
        yield return new WaitForSeconds(10);
        pc.speed = actualSpeed;
        yield return null;

    }

    private IEnumerator SlowDown()
    {
        float actualSpeed = pc.speed;
        pc.speed -= 2;
        yield return new WaitForSeconds(10);
        pc.speed = actualSpeed;
        yield return null;
    }

    private IEnumerator Invincible()
    {
        invincible = true;
        yield return new WaitForSeconds(10);
        invincible = false;
        yield return null;
    }

    private IEnumerator FlipMovement()
    {
        flipMov = true;
        yield return new WaitForSeconds(10);
        flipMov = false;
        yield return null;
    }
}
