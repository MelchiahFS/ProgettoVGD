using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealth : MonoBehaviour {

    private int startingHealth = 100;
    public int currentHealth;
    private float invTimer = 0;
    private float attackDuration = 0.5f;
    private float attackTimer = 0;
    private int meleeDamage = 30;
    private float invincibilityTime = 2f; //periodo di invulnerabilità dopo aver ricevuto danno
    private EnemyHealth eh;
    private Animator animator;
    Collider2D[] hitObjects;
    public bool isAttacking = false, isDead = false;
    private Slider slider;
    public GameObject weaponPrefab;
    private Weapon weapon;
    private int weaponNumber = 1;




    // Use this for initialization
    void Start ()
    {
        animator = GetComponent<Animator>();
        weapon = weaponPrefab.GetComponent<Weapon>();
        currentHealth = startingHealth;
        slider = GetComponentInChildren<Slider>();
        slider.minValue = 0;
        slider.maxValue = startingHealth;
        slider.value = startingHealth;
        currentHealth = startingHealth;
    }

    // Update is called once per frame
    void Update()
    {
        invTimer += Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            Debug.Log("Weapon 1");
            weaponNumber = 1;
        }
           
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Debug.Log("Weapon 2");
            weaponNumber = 2;
        }
            


        //da modificare e rendere un'arma effettiva
        if (weaponNumber == 1)
        {
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

        else if (weaponNumber == 2)
        {
            if (Input.GetKeyDown("up"))
            {
                weapon.Shoot("up", transform.position);
            }
            else if (Input.GetKeyDown("down"))
            {
                weapon.Shoot("down", transform.position);

            }
            else if (Input.GetKeyDown("left"))
            {
                weapon.Shoot("left", transform.position);

            }
            else if (Input.GetKeyDown("right"))
            {
                weapon.Shoot("right", transform.position);
            }
        }
        
       
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
            //else
            //    StartCoroutine(Flash(GetComponent<SpriteRenderer>()));
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
