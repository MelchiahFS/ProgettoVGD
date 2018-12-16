using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    private float startingHealth = 300, invTimer = 0, poisonDamageRate = 1, poisonDamage = 3, actualSpeed;
    private float invincibilityTime = 2f; //periodo di invulnerabilità dopo aver ricevuto danno
    public float currentHealth;
    private Animator animator;
    private Collider2D[] hitObjects;
    public bool isDead = false, isFlashing = false,isConsFlashing = false;
    private Slider slider;
    private Color playerColor;
    private PlayerController pc;
    public bool invincible = false, flipMov = false, flipAtt = false, poisoned = false, faster = false, slower = false, dd = false, gdd = false, hd = false;
    public Coroutine speedUpCO, slowDownCO, hdCO, ddCO, gddCO, invCO, flipMovCO, flipAttCO, poisonCO;
    private int tickNumber = 5;
    private SpriteRenderer rend;
    public Material hitColor, defaultMaterial;
    private GameObject iconsContainer, fastIcon, slowIcon, strongIcon, weakIcon, invIcon, vulnIcon, flipAttIcon, flipMovIcon, poisonIcon;
    private AnchorIcons anchor;

    
    
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

        iconsContainer = transform.Find("HealthBar").gameObject;
        fastIcon = iconsContainer.transform.Find("Fast").gameObject;
        slowIcon = iconsContainer.transform.Find("Slow").gameObject;
        strongIcon = iconsContainer.transform.Find("Strength").gameObject;
        weakIcon = iconsContainer.transform.Find("Weakness").gameObject;
        invIcon = iconsContainer.transform.Find("Invincibility").gameObject;
        vulnIcon = iconsContainer.transform.Find("Vulnerability").gameObject;
        flipAttIcon = iconsContainer.transform.Find("FlipAttack").gameObject;
        flipMovIcon = iconsContainer.transform.Find("FlipMovement").gameObject;
        poisonIcon = iconsContainer.transform.Find("Poison").gameObject;

        anchor = GetComponentInChildren<AnchorIcons>();
        if (anchor == null)
            Debug.Log("gayyyyyyyyy");
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

    public void HealthUp(int amount)
    {
        currentHealth += amount;
        slider.value = currentHealth;
    }

    public IEnumerator Invincible()
    {
        if (gdd)
        {
            StopCoroutine(GetDoubleDamage());
            anchor.SetIconPosition(vulnIcon, false);
            gdd = false;
        }
        anchor.SetIconPosition(invIcon, true);
        invincible = true;
        yield return new WaitForSeconds(10);
        anchor.SetIconPosition(invIcon, false);
        invincible = false;
        yield break;
    }

    public IEnumerator FlipMovement()
    {
        anchor.SetIconPosition(flipMovIcon, true);
        flipMov = true;
        yield return new WaitForSeconds(10);
        anchor.SetIconPosition(flipMovIcon, false);
        flipMov = false;
        yield break;
    }

    public IEnumerator FlipAttack()
    {
        anchor.SetIconPosition(flipAttIcon, true);
        flipAtt = true;
        yield return new WaitForSeconds(10);
        anchor.SetIconPosition(flipAttIcon, false);
        flipAtt = false;
        yield break;
    }

    public IEnumerator Poisoned()
    {
        anchor.SetIconPosition(poisonIcon, true);
        poisoned = true;
        for (int i = 0; i < tickNumber; i++)
        {
            yield return new WaitForSeconds(poisonDamageRate);
            ConsumableDamage(poisonDamage);
        }
        anchor.SetIconPosition(poisonIcon, false);
        poisoned = false;
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

    public IEnumerator SpeedUp()
    {
        anchor.SetIconPosition(fastIcon, true);
        faster = true;
        actualSpeed = pc.speed;
        pc.speed += 3;
        yield return new WaitForSeconds(10);
        SetNormalSpeed();
        yield break;

    }

    public IEnumerator SlowDown()
    {
        anchor.SetIconPosition(slowIcon, true);
        slower = true;
        actualSpeed = pc.speed;
        pc.speed -= 2;
        yield return new WaitForSeconds(10);
        SetNormalSpeed();
        yield break;
    }

    //reimposto la velocità normale del player; 
    //il controllo della shotSpeed avviene nella classe Weapon tramite i bool faster e slower di questa classe
    public void SetNormalSpeed()
    {
        if (faster)
        {
            StopCoroutine(speedUpCO);
            faster = false;
            anchor.SetIconPosition(fastIcon, false);
        }
        if (slower)
        {
            StopCoroutine(slowDownCO);
            slower = false;
            anchor.SetIconPosition(slowIcon, false);
        }

        pc.speed = actualSpeed;

    }


    public IEnumerator DoubleDamage()
    {
        anchor.SetIconPosition(strongIcon, true);
        dd = true;
        yield return new WaitForSeconds(10);
        SetNormalDamage();
        yield break;
    }

    public IEnumerator HalfDamage()
    {
        anchor.SetIconPosition(weakIcon, true);
        hd = true;
        yield return new WaitForSeconds(10);
        SetNormalDamage();
        yield break;
    }

    public IEnumerator GetDoubleDamage()
    {
        anchor.SetIconPosition(vulnIcon, true);
        gdd = true;
        yield return new WaitForSeconds(10);
        anchor.SetIconPosition(vulnIcon, false);
        gdd = false;
        yield break;
    }

    public void SetNormalDamage()
    {
        if (dd)
        {
            anchor.SetIconPosition(strongIcon, false);
            StopCoroutine(ddCO);
            dd = false;
        }
        if (hd)
        {
            anchor.SetIconPosition(weakIcon, false);
            StopCoroutine(hdCO);
            hd = false;
        }
    }
}
