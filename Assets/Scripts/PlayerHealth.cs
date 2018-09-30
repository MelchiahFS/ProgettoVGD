using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour {

    private int startingHealth = 100;
    private int currentHealth;
    private float invTimer = 0;
    private float attackDuration = 0.5f;
    private float attackTimer = 0;
    private int meleeDamage = 30;
    private float invincibilityTime = 2f; //periodo di invulnerabilità dopo aver ricevuto danno
    private PlayerController pc;
    private EnemyHealth eh;
    private Animator animator;
    Collider2D[] hitObjects;
    public bool isAttacking = false, isDead = false;




    // Use this for initialization
    void Start ()
    {
        animator = GetComponent<Animator>();
        pc = GetComponent<PlayerController>();
        currentHealth = startingHealth;	
	}

    // Update is called once per frame
    void Update()
    {
        invTimer += Time.deltaTime;
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackDuration)
        {
            animator.Play("Movement");
            isAttacking = false;

            if (Input.GetKeyDown("up"))
            {
                animator.Play("SlashUp");
                isAttacking = true;
                attackTimer = 0;
                hitObjects = Physics2D.OverlapCircleAll(transform.position + new Vector3(0, 0.5f, 0), 1f);
            }
            else if (Input.GetKeyDown("down"))
            {
                animator.Play("SlashDown");
                isAttacking = true;
                attackTimer = 0;
                hitObjects = Physics2D.OverlapCircleAll(transform.position + new Vector3(0, -0.5f, 0), 1f);
            }
            else if (Input.GetKeyDown("left"))
            {
                animator.Play("SlashLeft");
                isAttacking = true;
                attackTimer = 0;
                hitObjects = Physics2D.OverlapCircleAll(transform.position + new Vector3(-0.5f, 0, 0), 1f);
            }
            else if (Input.GetKeyDown("right"))
            {
                animator.Play("SlashRight");
                isAttacking = true;
                attackTimer = 0;
                hitObjects = Physics2D.OverlapCircleAll(transform.position + new Vector3(0.5f, 0, 0), 1f);
            }



            if (hitObjects != null)
            {
                Debug.Log(hitObjects.Length);
                foreach (Collider2D el in hitObjects)
                {
                    if (el.gameObject.tag == "Enemy" && el.isTrigger)
                    {
                        Debug.Log("enemy found!");
                        eh = el.gameObject.GetComponent<EnemyHealth>();
                        eh.TakeDamage(meleeDamage);
                    }
                }
                hitObjects = null;
            }


        }
    }

    //i nemici possono danneggiare nuovamente il player solo dopo un certo periodo
    public void TakeDamage(int amount)
    {
        if (invTimer > invincibilityTime)
        {
            invTimer = 0;
            currentHealth -= amount;
            Debug.Log("player health: " + currentHealth);
            if (currentHealth <= 0)
                PlayerDeath();
            else
                StartCoroutine(Flash(GetComponent<SpriteRenderer>()));
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
    private IEnumerator Flash(SpriteRenderer r)
    {
        Color c1 = r.color, c2 = r.color;
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
