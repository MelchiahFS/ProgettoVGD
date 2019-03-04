using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(AnchorHealthBar))]
public class EnemyHealth : MonoBehaviour
{
	private static System.Random rnd = new System.Random((int)DateTime.Now.Ticks);
	public float damage, bulletDamage; //danno da contatto e dei proiettili sparati
    private int tickNumber = 5; //numero di tick di danno ricevuti sotto status alterato
    public float currentHealth, startingHealth; //vita attuale e iniziale
	private float speedStatusTime = 5; //durata status SlowDown e SpeedUp
	private float poisonDamageRate = 1, fireDamageRate = 0.5f; //intervalli di tempo tra un tick e l'altro per gli status Poison e Fire
	private float poisonDamage = 3, fireDamage = 3; //danno per tick per gli status Poison e Fire
	private float fireContact = 1.5f; //durata minima di contatto per poter passare lo status Fire da un nemico a un altro nemico
	private float counter = 0; //misura il tempo del contatto tra nemici con status Fire
	private float speed; //conterrà temporaneamente la velocità del nemico prima di una alterazione di status SlowDown o SpeedUp
	private float fadeTime = 1.5f; //la durata della transizione di fade-off alla morte del nemico
    private PlayerHealth playerHealth;
    private PlayerController playerController;
    private SpriteRenderer rend, burnR, slowR, poisonR, fastR;
    private Slider slider;
    private AStarAI astar;
    public Material hitColor;
    private Material defaultMaterial;
    private Color normalColor;
    public bool isFlashing = false, poisoned = false, faster = false, burning = false, slowed = false, contact = false, dying = false;
    public Coroutine flashCO, slowCO, poisonCO, burnCO, fastCO;
    private GameObject player, hb, burnIcon, fastIcon, slowIcon, poisonIcon;
    public int points; //valore in punti del nemico ucciso
    private Text playerPoints;
	public Sprite bulletSprite;

    private AudioSource source;
    public AudioClip enemySound, enemyDeath;

	//valori relativi all'attesa fino alla prossima emissione del verso nemico
    private float waitingTime, waitCounter = 0;
    private bool waiting = true;

	private bool hasShootScript = false; //controlla che il nemico abbia uno script di attacco



	void Start ()
    {
        source = GetComponent<AudioSource>();
		player = GameManager.manager.playerReference;

		//recupero il riferimento al testo dei punti del player
		foreach (Text t in player.GetComponentsInChildren<Text>())
		{
			if (t.gameObject.name == "Points")
				playerPoints = t;
		}

		//se il nemico non vola recupero il riferimento allo script di AI
        if (!GetComponent<EnemyController>().flying)
            astar = GetComponent<AStarAI>();

        playerHealth = player.GetComponent<PlayerHealth>();
        rend = GetComponent<SpriteRenderer>();
        defaultMaterial = rend.material;

        hb = transform.Find("EnemyHealthBar").gameObject;
        slider = GetComponentInChildren<Slider>();

		//se c'è uno script di attacco imposto la sprite del proiettile
		if (GetComponent<ShootAbstract>() != null)
		{
			hasShootScript = true;
			if (GameStats.stats.enemyBulletSprites.ContainsKey(gameObject.name))
				bulletSprite = GameStats.stats.enemyBulletSprites[gameObject.name];
			else
			{
				bulletSprite = ItemSpriteSelector.iss.bullets[rnd.Next(ItemSpriteSelector.iss.bullets.Count)];
				GameStats.stats.enemyBulletSprites.Add(gameObject.name, bulletSprite);
			}
		}

		//incremento il danno da contatto a seconda del livello
		switch (GameStats.stats.levelNumber)
		{
			case 2:
				startingHealth += startingHealth / 2;
				damage += damage * 15 / 100;
				bulletDamage += bulletDamage * 15 / 100;
				break;
			case 3:
				startingHealth += startingHealth;
				damage += damage * 30 / 100;
				bulletDamage += bulletDamage * 30 / 100;
				break;
			case 4:
				startingHealth += startingHealth + startingHealth / 2;
				damage += damage * 45 / 100;
				bulletDamage += bulletDamage * 45 / 100;
				break;
			case 5:
				startingHealth += startingHealth * 2;
				damage += damage * 60 / 100;
				bulletDamage += bulletDamage * 60 / 100;
				break;

		}

		//imposto i punti vita iniziali
        slider.minValue = 0;
        slider.maxValue = startingHealth;
        slider.value = startingHealth;
        currentHealth = startingHealth;

		//recupero i riferimenti alle icone di status
        burnIcon = hb.transform.Find("Fire").gameObject;
        poisonIcon = hb.transform.Find("Poison").gameObject;
        slowIcon = hb.transform.Find("Slow").gameObject;
        fastIcon = hb.transform.Find("Fast").gameObject;

        waitingTime = UnityEngine.Random.Range(2, 5);
        waiting = true;
    }

    void Update()
    {
        if (!dying)
        {
			//il nemico emetterà un verso a intervalli casuali
            if (!waiting)
            {
				if (gameObject.name != "Slime")
					source.PlayOneShot(enemySound);
                waitingTime = UnityEngine.Random.Range(3, 7);
                waiting = true;
            }
            else
            {
                if (waitCounter >= waitingTime)
                {
                    waitCounter = 0;
                    waiting = false;
                }
                else
                {
                    waitCounter += Time.deltaTime;
                }
            }

			//calcolo il tempo di contatto per poter "passare" il fuoco ad altri nemici
            if (contact)
            {
                counter += Time.deltaTime;
            }
        }
        
    }

	
	void OnCollisionStay2D(Collision2D collision)
    {
		//se il nemico sta a contatto col player questo riceve danno da contatto
        if (collision.gameObject.tag == "Player")
        {
            playerHealth.TakeDamage(damage);
        }
		//se un nemico sotto status Fire sta a contatto con un altro nemicoattivo il counter e il flag di contatto
		else if (collision.gameObject.tag == "Enemy" && burning)
        {
            contact = true;
			//se sta a contatto più del tempo limite azzero il counter e passo lo status Fire all'altro nemico
            if (counter >= fireContact)
            {
                counter = 0;
                collision.gameObject.GetComponent<EnemyHealth>().ApplyModifier(ItemStats.BulletType.burning);
            }
        }
    }

	//se un nemico sotto status Fire abbandona il contatto con un altro nemico azzero il counter e il flag di contatto
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && burning)
        {
            contact = false;
            counter = 0;
        }
    }

	//infligge danno standard al nemico
    public void TakeDamage(float amount)
    {
        //controllo se il player è sotto l'effetto di doubleDamage o halfDamage
        if (playerHealth.dd)
            amount *= 2;
        else if (playerHealth.hd)
            amount /= 2;

		//se il danno non uccide il nemico aggiorno i punti vita e lo slider
        if (currentHealth - amount > 0)
        {
            currentHealth -= amount;
            slider.value -= amount;
            StartCoroutine(Flash());
        }
		//altrimenti elimino il nemico
        else
        {
            currentHealth = 0;
            slider.value = 0;

            //incremento il punteggio del player
			GameStats.stats.playerPoints += points;
			playerPoints.text = "PP " + GameStats.stats.playerPoints.ToString();

			//uccido il nemico
			if (!dying)
                StartCoroutine(Die());
        }
            
    }

	//disattiva coroutine e script vari del nemico e poi lo distrugge
    private IEnumerator Die()
    {
        dying = true;
        float rate = 1 / fadeTime;
        Color c = Color.white;

        //aspetto che l'animazione del danno finisca
        while (isFlashing)
        {
            yield return 0;
        }

        source.PlayOneShot(enemyDeath);

        //termino eventuali coroutine attive
        if (slowed)
            StopCoroutine(slowCO);
		if (faster)
			StopCoroutine(fastCO);
        if (poisoned)
            StopCoroutine(poisonCO);
        if (burning)
            StopCoroutine(burnCO);

        //rimuovo la healthBar
        Destroy(hb);

        //blocco l'animazione e i movimenti
        Animator animator = GetComponent<Animator>();
        animator.speed = 0;
        animator = GetComponent<Animator>();
        if (!GetComponent<EnemyController>().flying)
            Destroy(GetComponent<AStarAI>());
        else
            GetComponent<MovementPattern>().speed = 0;

        //impedisco al nemico di sparare ancora
        if (GetComponent<ShootPlayer>() != null)
            Destroy(GetComponent<ShootPlayer>());
        if (GetComponent<ShootCircle>() != null)
            Destroy(GetComponent<ShootCircle>());
        if (GetComponent<ShootMultiple>() != null)
            Destroy(GetComponent<ShootMultiple>());
        if (GetComponent<ShootBurst>() != null)
            Destroy(GetComponent<ShootBurst>());
        if (GetComponent<ShootBidirectional>() != null)
            Destroy(GetComponent<ShootBidirectional>());

        //rendo il nemico intangibile
        foreach (Collider2D coll in GetComponents<Collider2D>())
        {
            coll.enabled = false;
        }
        
        rend.color = c;
        rend.material = hitColor;

        //creo l'effetto di fade-off
        while (c.a > 0)
        {
            c.a -= Time.deltaTime * rate;
            rend.color = c;
            yield return 0;
        }
        //decrementa il counter dei nemici ancora vivi per la stanza attuale (intercettata da RoomChange)
        playerHealth.SendMessage("DecreaseEnemyCounter");

        //elimino il nemico dalla lista dei nemici per la stanza attuale, poi lo elimino dalla scena
        Room actualRoom = GameManager.manager.ActualRoom;
        actualRoom.enemies.Remove(gameObject);
        actualRoom.toSort.Remove(gameObject);
        Destroy(gameObject);
        yield break;
    }

    //crea un flash bianco quando il nemico viene colpito
    public IEnumerator Flash()
    {
        while (isFlashing)
            yield return 0;

        Color c = rend.color;
        isFlashing = true;

        rend.color = Color.white;
        rend.material = hitColor;
        yield return new WaitForSeconds(0.1f);

        rend.material = defaultMaterial;
        c.a = 1;
        rend.color = c;

        isFlashing = false;
        yield break;
    }

	//decrementa la velocità del nemico e dei suoi proiettili
	public IEnumerator SlowDown()
    {
        while (slowed)
            yield return null;

        GetComponent<AnchorHealthBar>().SetIconPosition(slowIcon, true);
        slowed = true;

		if (hasShootScript)
			gameObject.SendMessage("SetShotSpeed", -2);

        speed = GetComponent<MovementPattern>().speed;
        if (!GetComponent<EnemyController>().flying)
            astar.SetSpeed(1);
        else
            GetComponent<MovementPattern>().speed = 1;
        yield return new WaitForSeconds(speedStatusTime);
        
        SetNormalSpeed();
        yield break;
    }

	//incrementa la velocità del nemico e dei suoi proiettili
    public IEnumerator SpeedUp()
    {
		//finché è già attivo lo status SpeedUp non faccio nulla
        while (faster)
            yield return null;

        GetComponent<AnchorHealthBar>().SetIconPosition(fastIcon, true);
        faster = true;

		//se è presente uno script d'attacco aumento la velocità dei proiettili nemici
		if (hasShootScript)
			gameObject.SendMessage("SetShotSpeed", 2);

        //incremento la velocità del nemico
        speed = GetComponent<MovementPattern>().speed;
        if (!GetComponent<EnemyController>().flying)
            astar.SetSpeed(speed + 2);
        else
            GetComponent<MovementPattern>().speed +=2;
        yield return new WaitForSeconds(speedStatusTime);

        SetNormalSpeed();
        yield break;
    }

	//infligge danno da veleno al nemico per un determinato numero di tick
	public IEnumerator Poisoned()
    {
        //aspetto che sia finita la precedente chiamata alla coroutine
        while (poisoned)
            yield return null;

        GetComponent<AnchorHealthBar>().SetIconPosition(poisonIcon, true);
        poisoned = true;

        for (int i = 0; i < tickNumber; i++)
        {
            yield return new WaitForSeconds(poisonDamageRate);
            TakeDamage(poisonDamage);
        }

        GetComponent<AnchorHealthBar>().SetIconPosition(poisonIcon, false);
        poisoned = false;
        yield break;
    }

	//infligge danno da fuoco al nemico per un determinato numero di tick
    private IEnumerator Burn()
    {
        GetComponent<AnchorHealthBar>().SetIconPosition(burnIcon, true);
        burning = true;

        for (int i = 0; i < tickNumber; i++)
        {
            yield return new WaitForSeconds(fireDamageRate);
            TakeDamage(fireDamage);
        }

        GetComponent<AnchorHealthBar>().SetIconPosition(burnIcon, false);
        burning = false;
        yield break;

    }

	//applica lo status relativo al proiettile che colpisce il nemico
    public void ApplyModifier(ItemStats.BulletType bullet)
    {
        if (bullet == ItemStats.BulletType.poisonous)
        {
			//se non è già avvelenato avvelena il nemico
            if (!poisoned)
                poisonCO = StartCoroutine(Poisoned());
        }
        else if (bullet == ItemStats.BulletType.burning)
        {
            if (!burning)
                burnCO = StartCoroutine(Burn());
        }
        else if (bullet == ItemStats.BulletType.slowing)
        {
			//se non è già rallentato
            if (!slowed)
            {
				//se il nemico era sotto status di velocità aumentata i due effetti si annullano
                if (faster)
                    SetNormalSpeed();
				//altrimenti lo rallento
                else
                    slowCO = StartCoroutine(SlowDown());
            }
                
        }
    }

	//ripristina la velocità normale dei nemici e dei relativi proiettili
    public void SetNormalSpeed()
    {
		//interrompo gli status di velocità alterata
        if (faster)
        {
            StopCoroutine(fastCO);
            faster = false;
            GetComponent<AnchorHealthBar>().SetIconPosition(fastIcon, false);
			//se il nemico ha uno script d'attacco ripristino la velocità dei proiettili iniziale
			if (hasShootScript)
				gameObject.SendMessage("SetShotSpeed", -2);
        }
        if (slowed)
        {
            StopCoroutine(slowCO);
            slowed = false;
            GetComponent<AnchorHealthBar>().SetIconPosition(slowIcon, false);
			if (hasShootScript)
				gameObject.SendMessage("SetShotSpeed", 2);
        }

		//ripristino la velocità di partenza del nemico
        if (!GetComponent<EnemyController>().flying)
            astar.SetSpeed(speed);
        else
            GetComponent<MovementPattern>().speed = speed;

        
    }

	//usato dall'animazione del nemico Slime
    public void PlayJumpSound()
    {
        source.PlayOneShot(enemySound);
    }
}
