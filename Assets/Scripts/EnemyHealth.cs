using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AnchorHealthBar))]
public class EnemyHealth : MonoBehaviour {

    public int damage = 10;
    public float startingHealth = 50;
    public float currentHealth;
    private GameObject player;
    private PlayerHealth playerHealth;
    private PlayerController playerController;
    private SpriteRenderer rend, burnR, slowR, poisonR, fastR;
    private Slider slider;
    private AStarAI astar;
    public Material hitColor;
    private Material defaultMaterial;
    private Color normalColor;
    private float fadeTime = 1.5f;
    public bool isFlashing = false, poisoned = false, faster = false, burning = false, slowed = false, contact = false;
    public bool dying = false;
    private int tickNumber = 5;
    private float slowDownTime = 5, poisonDamageRate = 1, poisonDamage = 3, fireDamageRate = 0.5f, fireDamage = 3, fireContact = 1.5f;
    private float counter = 0, speed;
    private Coroutine flashCO, slowCO, poisonCO, burnCO;
    private GameObject hb;

	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (!GetComponent<EnemyController>().flying)
            astar = GetComponent<AStarAI>();
        playerHealth = player.GetComponent<PlayerHealth>();
        rend = GetComponent<SpriteRenderer>();
        defaultMaterial = rend.material;
        hb = transform.Find("EnemyHealthBar").gameObject;
        slider = GetComponentInChildren<Slider>();
        slider.minValue = 0;
        slider.maxValue = startingHealth;
        slider.value = startingHealth;
        currentHealth = startingHealth;
    }

    void Update()
    {
        if (contact)
        {
            counter += Time.deltaTime;
        }
    }
	
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            playerHealth.TakeDamage(damage);
        }
        else if (collision.gameObject.tag == "Enemy" && burning)
        {
            contact = true;
            if (counter >= fireContact)
            {
                counter = 0;
                collision.gameObject.GetComponent<EnemyHealth>().ApplyModifier(ItemStats.BulletType.burning);
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy" && burning)
        {
            contact = false;
            counter = 0;
        }
    }

    public void TakeDamage(float amount)
    {
        if (playerHealth.dd)
            amount *= 2;
        if (playerHealth.hd)
            amount /= 2;

        if (currentHealth - amount > 0)
        {
            currentHealth -= amount;
            slider.value -= amount;
            StartCoroutine(Flash());
        }
        else
        {
            currentHealth = 0;
            slider.value = 0;
            StartCoroutine(Die());
        }
            
    }

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

        //termino eventuali coroutine attive
        if (slowed)
            StopCoroutine(slowCO);
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

    public IEnumerator SlowDown()
    {

        GetComponent<AnchorHealthBar>().SetIconPosition(hb.transform.Find("Slow").gameObject, true);
        slowed = true;

        speed = GetComponent<MovementPattern>().speed;
        if (!GetComponent<EnemyController>().flying)
            astar.SetSpeed(1);
        else
            GetComponent<MovementPattern>().speed = 1;
        yield return new WaitForSeconds(slowDownTime);
        
        if (!GetComponent<EnemyController>().flying)
            astar.SetSpeed(speed);
        else
            GetComponent<MovementPattern>().speed = speed;

        GetComponent<AnchorHealthBar>().SetIconPosition(hb.transform.Find("Slow").gameObject, false);
        slowed = false;
        yield break;
    }

    public IEnumerator SpeedUp()
    {
        GetComponent<AnchorHealthBar>().SetIconPosition(hb.transform.Find("Fast").gameObject, true);
        faster = true;

        gameObject.SendMessage("SetShotSpeed", 2);
        //incremento la velocitò del nemico
        speed = GetComponent<MovementPattern>().speed;
        if (!GetComponent<EnemyController>().flying)
            astar.SetSpeed(speed + 2);
        else
            GetComponent<MovementPattern>().speed +=2;
        yield return new WaitForSeconds(slowDownTime);
        
        if (!GetComponent<EnemyController>().flying)
            astar.SetSpeed(speed);
        else
            GetComponent<MovementPattern>().speed = speed;

        gameObject.SendMessage("SetShotSpeed", -2);
        GetComponent<AnchorHealthBar>().SetIconPosition(hb.transform.Find("Fast").gameObject, false);
        faster = false;
        yield break;
    }

    public IEnumerator Poisoned()
    {
        //aspetto che sia finita la precedente chiamata alla coroutine
        while (poisoned)
            yield return null;

        GetComponent<AnchorHealthBar>().SetIconPosition(hb.transform.Find("Poison").gameObject, true);
        poisoned = true;

        for (int i = 0; i < tickNumber; i++)
        {
            yield return new WaitForSeconds(poisonDamageRate);
            TakeDamage(poisonDamage);
        }

        GetComponent<AnchorHealthBar>().SetIconPosition(hb.transform.Find("Poison").gameObject, false);
        poisoned = false;
        yield break;
    }

    private IEnumerator Burn()
    {
        GetComponent<AnchorHealthBar>().SetIconPosition(hb.transform.Find("Fire").gameObject, true);
        burning = true;

        for (int i = 0; i < tickNumber; i++)
        {
            yield return new WaitForSeconds(fireDamageRate);
            TakeDamage(fireDamage);
        }

        GetComponent<AnchorHealthBar>().SetIconPosition(hb.transform.Find("Fire").gameObject, false);
        burning = false;
        yield break;

    }

    public void ApplyModifier(ItemStats.BulletType bullet)
    {
        if (bullet == ItemStats.BulletType.poisonous)
        {
            if (!poisoned)
                poisonCO = StartCoroutine(Poisoned());
        }
        else if (bullet == ItemStats.BulletType.slowing)
        {
            if (!slowed)
                slowCO = StartCoroutine(SlowDown());
        }
        else if (bullet == ItemStats.BulletType.burning)
        {
            if (!burning)
                burnCO = StartCoroutine(Burn());
        }
    }
}
