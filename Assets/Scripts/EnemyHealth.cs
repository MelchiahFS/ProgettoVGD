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
    private SpriteRenderer rend;
    private Slider slider;
    public Material hitColor;
    private Material defaultMaterial;
    private Color color;
    private float fadeTime = 2f;
    private bool isFlashing = false;
        

	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        playerHealth = player.GetComponent<PlayerHealth>();
        rend = GetComponent<SpriteRenderer>();
        color = rend.color;
        defaultMaterial = rend.material;
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
        if (currentHealth - amount > 0)
        {
            currentHealth -= amount;
            slider.value -= amount;
            StartCoroutine(Flash());
        }
        else
        {
            currentHealth -= amount;
            slider.value -= amount;
            DeathAnimation();
        }
            
    }

    //crea un effetto flash quando il player viene colpito
    private IEnumerator Flash()
    {
        isFlashing = true;
        rend.color = Color.white;
        rend.material = hitColor;
        yield return new WaitForSeconds(0.1f);

        rend.material = defaultMaterial;
        color.a = 1;
        rend.color = color;
        isFlashing = false;
        yield break;
    }

    private void DeathAnimation()
    {
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {

        float rate = 1 / fadeTime;
        Color c = Color.white;

        while (isFlashing)
        {
            yield return 0;
        }

        Destroy(transform.Find("HealthBar").gameObject);
        GetComponent<Animator>().speed = 0;
        Destroy(GetComponent<AStarAI>());
        if (GetComponent<ShootPlayer>() != null)
            Destroy(GetComponent<ShootPlayer>());
        if (GetComponent<ShootCircle>() != null)
            Destroy(GetComponent<ShootCircle>());
        if (GetComponent<ShootMultiple>() != null)
            Destroy(GetComponent<ShootMultiple>());
        foreach (Collider2D coll in GetComponents<Collider2D>())
        {
            coll.enabled = false;
        }
        

        rend.color = c;
        rend.material = hitColor;

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
}
