using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    private static System.Random rnd = new System.Random((int)DateTime.Now.Ticks);
	private int tickNumber = 5; //numero tick status veleno
	private float poisonDamageRate = 1; //intervallo di tempo tra un tick di veleno e un altro
	private float poisonDamage = 3; //danno da veleno
	private float actualSpeed; //conserva la velocità per quando si è afflitti da slowDown o speedUp
    private float invTimer = 0, invincibilityTime = 1.5f; //contatore e durata di invulnerabilità dopo aver ricevuto danno
    private Animator animator;
    public bool isDead = false, isFlashing = false, isConsFlashing = false; //flag riguardandi la morte o il flash da danno fisico o consumable
    private Slider slider;
    private Color playerColor;
    private PlayerController pc;

	//flag riguardanti gli status alterati
	public bool invincible = false, flipMov = false, flipAtt = false, poisoned = false;
	public bool faster = false, slower = false, dd = false, gdd = false, hd = false;

    public Coroutine speedUpCO, slowDownCO, hdCO, ddCO, gddCO, invCO, flipMovCO, flipAttCO, poisonCO; //riferimenti alle coroutine degli status alterati

    private SpriteRenderer rend;
    public Material hitColor, defaultMaterial;

	//riferimenti alle icone di status
    private GameObject iconsContainer, fastIcon, slowIcon, strongIcon, weakIcon, invIcon, vulnIcon, flipAttIcon, flipMovIcon, poisonIcon;
    private AnchorIcons anchor;
	private Text money, points, hpAmount;

    private AudioSource source;
    public AudioClip hurt, hurt1, hurt2, dead;
	
    // Use this for initialization
    void Start ()
    {
        invTimer = invincibilityTime;
        rend = GetComponent<SpriteRenderer>();
        defaultMaterial = rend.material;
        pc = GetComponent<PlayerController>();
        animator = GetComponent<Animator>();

		//recupero i riferimenti a punti, soldi e hp del player
		foreach (Text t in GetComponentsInChildren<Text>())
		{
			if (t.gameObject.name == "Money")
				money = t;
			else if (t.gameObject.name == "Points")
				points = t;
			else if (t.gameObject.name == "HealthPoints")
				hpAmount = t;
		}
		money.text = "$ " + GameStats.stats.playerMoney.ToString();
		points.text = "PP " + GameStats.stats.playerPoints.ToString();

		//preparo la healthBar coi valori iniziali di punti vita
		slider = GetComponentInChildren<Slider>();
        slider.minValue = 0;
		slider.maxValue = GameStats.stats.maxHealth;
		slider.value = GameStats.stats.playerHealth;
		hpAmount.text = GameStats.stats.playerHealth.ToString();
		
		playerColor = rend.color;

		//recupero i riferimenti alle icone di status
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
        source = GetComponent<AudioSource>();
    }

    void Update()
    {
        invTimer += Time.deltaTime; //incremento il counter di invulnerabilità per danno ricevuto
    }

    //Infligge danno da combattimento al player
    public void TakeDamage(float amount)
    {
		//se il player non è invulnerabile (per status o danno appena ricevuto)
        if (!invincible && invTimer >= invincibilityTime)
        {
            invTimer = 0; //riazzero il timer di invulnerabilità

			//se lo status di vulnerabilità è attivo, il player riceve danno doppio
			if (gdd)
                amount *= 2;

			//se la vita del player è a 0 avvio la morte
            if (GameStats.stats.playerHealth - amount <= 0)
			{
				GameStats.stats.playerHealth = 0;
                slider.value = GameStats.stats.playerHealth;
				hpAmount.text = GameStats.stats.playerHealth.ToString();
				PlayerDeath();
            }
			//altrimenti aggiorno la healthBar coi punti vita attuali
            else
            {
                int value = rnd.Next(3);
                if (value == 0)
                    source.PlayOneShot(hurt);
                else if (value == 1)
                    source.PlayOneShot(hurt1);
                else
                    source.PlayOneShot(hurt2);

				GameStats.stats.playerHealth -= amount;
                slider.value = GameStats.stats.playerHealth;
				hpAmount.text = GameStats.stats.playerHealth.ToString();
				StartCoroutine(Flash(playerColor, GetComponent<SpriteRenderer>()));
            }

        }

    }

	//Attiva la morte del player
    private void PlayerDeath()
    {
		MusicManager.mm.musicController.Stop();
        source.PlayOneShot(dead);
		GameManager.manager.isDying = true;
		animator.SetBool("isDead", true);
        this.enabled = false;
        foreach (Collider2D c in GetComponents<Collider2D>())
            c.enabled = false;
    }

	//Attiva la schermata di game over (lanciato dall'animazione della morte)
	public void SignalPlayerDeath()
	{
		GameManager.manager.dead = true;
		isDead = true;
		MusicManager.mm.GameOver();
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

	//Ripristina alcuni punti vita al player
	public void HealthUp(int amount)
	{
		if (GameStats.stats.playerHealth + amount >= GameStats.stats.maxHealth)
		{
			GameStats.stats.playerHealth = GameStats.stats.maxHealth;
		}
		else
		{
			GameStats.stats.playerHealth += amount;
		}
		slider.value = GameStats.stats.playerHealth;
		hpAmount.text = GameStats.stats.playerHealth.ToString();
	}

	//Rende il player invulnerabile
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

	//Inverte i movimenti del player
    public IEnumerator FlipMovement()
    {
        anchor.SetIconPosition(flipMovIcon, true);
        flipMov = true;
        yield return new WaitForSeconds(10);
        anchor.SetIconPosition(flipMovIcon, false);
        flipMov = false;
        yield break;
    }

	//Inverte la direzione degli attacchi del player
    public IEnumerator FlipAttack()
    {
        anchor.SetIconPosition(flipAttIcon, true);
        flipAtt = true;
        yield return new WaitForSeconds(10);
        anchor.SetIconPosition(flipAttIcon, false);
        flipAtt = false;
        yield break;
    }

	//Infligge lo status veleno al player
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
		//se lo status di vulnerabilità è attivo, il player riceve danno doppio
        if (gdd)
            amount *= 2;

        if (GameStats.stats.playerHealth - amount <= 0)
        {
			GameStats.stats.playerHealth = 0;
            slider.value = GameStats.stats.playerHealth;
			hpAmount.text = GameStats.stats.playerHealth.ToString();
			PlayerDeath();
        }
        else
        {
			GameStats.stats.playerHealth -= amount;
            slider.value = GameStats.stats.playerHealth;
			hpAmount.text = GameStats.stats.playerHealth.ToString();
			if (!isFlashing)
                StartCoroutine(ConsumableFlash());
        }
        
    }

    //crea un flash bianco quando il player riceve danno da consumable
    public IEnumerator ConsumableFlash()
    {
        while (isConsFlashing)
            yield return null;

		//per rendere la sprite completamente bianca utilizzo un material
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

	//Accelera i movimenti del player
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

	//Rallenta i movimenti del player
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

    //Ripristina la velocità normale del player
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

	//Raddoppia il danno inflitto dal player
	public IEnumerator DoubleDamage()
    {
        anchor.SetIconPosition(strongIcon, true);
        dd = true;
        yield return new WaitForSeconds(10);
        SetNormalDamage();
        yield break;
    }

	//Dimezza il danno inflitto dal player
    public IEnumerator HalfDamage()
    {
        anchor.SetIconPosition(weakIcon, true);
        hd = true;
        yield return new WaitForSeconds(10);
        SetNormalDamage();
        yield break;
    }

	//Raddoppia il danno ricevuto dal player
    public IEnumerator GetDoubleDamage()
    {
        anchor.SetIconPosition(vulnIcon, true);
        gdd = true;
        yield return new WaitForSeconds(10);
        anchor.SetIconPosition(vulnIcon, false);
        gdd = false;
        yield break;
    }

	//Ripristina il danno normale
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
