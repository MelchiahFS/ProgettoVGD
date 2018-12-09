using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    private float startingHealth = 300;
    public float currentHealth;
    private float invTimer = 0;
    private float invincibilityTime = 2f; //periodo di invulnerabilità dopo aver ricevuto danno
    private Animator animator;
    Collider2D[] hitObjects;
    public bool isDead = false, isFlashing = false,isConsFlashing = false;
    private Slider slider;
    private Color playerColor;
    private PlayerController pc;
    public bool invincible = false, flipMov = false, flipAtt = false, poisoned = false, faster = false, slower = false, dd = false, gdd = false, hd = false;
    private Coroutine speedUpCO, slowDownCO, hdCO, ddCO, gddCO, speedUpAllCO, slowDownAllCO, invCO, flipMovCO, flipAttCO, poisonCO, poisonAllCO;
    private int tickNumber = 5;
    private float poisonDamageRate = 1;
    private float poisonDamage = 3;
    private SpriteRenderer rend;
    public Material hitColor, defaultMaterial;


    // Use this for initialization
    void Start ()
    {
        invTimer = invincibilityTime;
        rend = GetComponent<SpriteRenderer>();
        defaultMaterial = rend.material;
        pc = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();
        
        slider = GetComponentInChildren<Slider>();
        slider.minValue = 0;
        slider.maxValue = startingHealth;

        //slider.value = startingHealth;
        //currentHealth = startingHealth;
        slider.value = 150;
        currentHealth = 150;

        playerColor = rend.color;
    }

    // Update is called once per frame
    void Update()
    {
        invTimer += Time.deltaTime;       
    }

    //i nemici possono danneggiare nuovamente il player solo dopo un certo periodo
    public void TakeDamage(float amount)
    {

        if (!invincible && invTimer >= invincibilityTime)
        {
            invTimer = 0;

            if (gdd)
                amount *= 2;

            if (currentHealth - amount <= 0)
            {
                currentHealth = 0;
                slider.value = currentHealth;
                PlayerDeath();
            }
            else
            {
                currentHealth -= amount;
                slider.value = currentHealth;
                StartCoroutine(Flash(playerColor, GetComponent<SpriteRenderer>()));
            }

        }

    }


    private void PlayerDeath()
    {
        animator.SetBool("isDead", true);
        isDead = true;
        this.enabled = false;
        GameManager.manager.Invoke("ReturnToMenu", 2f);
    }

    //crea un effetto flash intermittente quando il player viene colpito
    private IEnumerator Flash(Color c1, SpriteRenderer r)
    {
        Color c2 = c1;
        c2.a = 0;
        isFlashing = true;
        while (invTimer < invincibilityTime)
        {
            r.color = c2;
            yield return new WaitForSeconds(0.04f);
            r.color = c1;
            yield return new WaitForSeconds(0.04f);
        }
        isFlashing = false;
        yield break;
    }

    //------------------------------
    //Effetti relativi ai consumables

    public void ApplyEffect(ItemStats.ConsumableType consumable)
    {
        Room actualRoom = GameManager.manager.ActualRoom;

        switch (consumable)
        {
            case ItemStats.ConsumableType.healthUp25: //ok
                HealthUp(25);
                break;

            case ItemStats.ConsumableType.healthUp50: //ok
                HealthUp(50);
                break;

            case ItemStats.ConsumableType.slowDownAll: //ok
                foreach (GameObject en in actualRoom.enemies)
                {
                    slowDownAllCO = StartCoroutine(en.GetComponent<EnemyHealth>().SlowDown());
                }
                break;

            case ItemStats.ConsumableType.slowDownSelf: //ok
                slowDownCO = StartCoroutine(SlowDown());
                break;

            case ItemStats.ConsumableType.poisonAll: //ok
                foreach (GameObject en in actualRoom.enemies)
                {
                    poisonAllCO = StartCoroutine(en.GetComponent<EnemyHealth>().Poisoned());
                }
                break;

            case ItemStats.ConsumableType.poisonSelf: //ok
                poisonCO = StartCoroutine(Poisoned());
                break;

            case ItemStats.ConsumableType.damageAll: //ok
                foreach (GameObject en in actualRoom.enemies)
                {
                    en.GetComponent<EnemyHealth>().TakeDamage(20);
                }
                break;

            case ItemStats.ConsumableType.damageSelf: //ok
                ConsumableDamage(20);
                break;

            case ItemStats.ConsumableType.flipMovement: //ok
                flipMovCO = StartCoroutine(FlipMovement());
                break;

            case ItemStats.ConsumableType.invincible: //ok
                invCO = StartCoroutine(Invincible());
                break;

            case ItemStats.ConsumableType.speedUpSelf: //ok
                speedUpCO = StartCoroutine(SpeedUp());
                break;

            case ItemStats.ConsumableType.speedUpAll: //ok
                foreach (GameObject en in actualRoom.enemies)
                {
                    speedUpAllCO = StartCoroutine(en.GetComponent<EnemyHealth>().SpeedUp());
                }
                break;

            case ItemStats.ConsumableType.doubleDamage: //ok
                ddCO = StartCoroutine(DoubleDamage());
                break;

            case ItemStats.ConsumableType.halfDamage: //ok
                hdCO = StartCoroutine(HalfDamage());
                break;

            case ItemStats.ConsumableType.getDoubleDamage: //ok
                gddCO = StartCoroutine(GetDoubleDamage());
                break;
                
            case ItemStats.ConsumableType.flipAttack: //ok
                flipAttCO = StartCoroutine(FlipAttack());
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
        faster = true;
        float actualSpeed = pc.speed;
        pc.speed += 3;
        yield return new WaitForSeconds(10);
        pc.speed = actualSpeed;
        faster = false;
        yield break;

    }

    private IEnumerator SlowDown()
    {
        slower = true;
        float actualSpeed = pc.speed;
        pc.speed -= 2;
        yield return new WaitForSeconds(10);
        pc.speed = actualSpeed;
        slower = false;
        yield break;
    }

    private IEnumerator Invincible()
    {
        invincible = true;
        yield return new WaitForSeconds(10);
        invincible = false;
        yield break;
    }

    private IEnumerator FlipMovement()
    {
        flipMov = true;
        yield return new WaitForSeconds(10);
        flipMov = false;
        yield break;
    }

    private IEnumerator FlipAttack()
    {
        flipAtt = true;
        yield return new WaitForSeconds(10);
        flipAtt = false;
        yield break;
    }

    public IEnumerator Poisoned()
    {
        poisoned = true;
        //p.enabled = true;

        for (int i = 0; i < tickNumber; i++)
        {
            yield return new WaitForSeconds(poisonDamageRate);
            ConsumableDamage(poisonDamage);
        }

        //p.enabled = false;
        poisoned = false;
        yield break;
    }

    private IEnumerator DoubleDamage()
    {
        dd = true;
        yield return new WaitForSeconds(10);
        dd = false;
        yield break;
    }

    private IEnumerator HalfDamage()
    {
        hd = true;
        yield return new WaitForSeconds(10);
        hd = false;
        yield break;
    }

    private IEnumerator GetDoubleDamage()
    {
        gdd = true;
        yield return new WaitForSeconds(10);
        gdd = false;
        yield break;
    }

    //infligge danno da consumable al player
    public void ConsumableDamage(float amount)
    {
        if (gdd)
            amount *= 2;

        if (currentHealth - amount <= 0)
        {
            currentHealth = 0;
            slider.value = currentHealth;
            PlayerDeath();
        }
        else
        {
            currentHealth -= amount;
            slider.value = currentHealth;
            if (!isFlashing)
                StartCoroutine(ConsumableFlash());
        }
        
    }

    //crea un flash bianco quando il player riceve danno da consumable
    public IEnumerator ConsumableFlash()
    {
        while (isConsFlashing)
            yield return null;

        Color c = rend.color;
        isConsFlashing = true;

        rend.color = Color.white;
        rend.material = hitColor;
        yield return new WaitForSeconds(0.1f);

        rend.material = defaultMaterial;
        c.a = 1;
        rend.color = c;

        isConsFlashing = false;
        yield break;
    }
}
