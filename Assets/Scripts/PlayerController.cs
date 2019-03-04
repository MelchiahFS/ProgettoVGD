using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class PlayerController : Character
{
    private Rigidbody2D rb2d;
    public float speed;
	private Animator animator;
    private GameObject hitbox;
    private PlayerHealth ph;
    private Weapon weapon;
    private float faceX, faceY;
    float x, y;

    public float fadeTime = 0.3f;

    private void Start()
    {
        ph = GetComponent<PlayerHealth>();
        weapon = GetComponentInChildren<Weapon>();
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        SetRealOffset(gameObject);
    }
	
	void FixedUpdate()
	{
		//se il gioco non è in pausa
		if (!GameManager.manager.gamePause)
		{
			//prendo i valori di input degli assi x e y
			x = Input.GetAxisRaw("CustomHorizontal");

			y = Input.GetAxisRaw("CustomVertical");

			//se è attivo lo status FlipMovement inverto gli assi direzionali
			if (ph.flipMov)
			{
				x = -x;
				y = -y;
			}

			//creo il vettore di movimento
			Vector2 movement = new Vector2(x, y);

			//se il player attacca o sta morendo non può muoversi
			if (!weapon.isAttacking && !GameManager.manager.isDying && !GameManager.manager.ending)
				rb2d.velocity = movement * speed;
			else
				rb2d.velocity = movement * 0;
		}
	}

    void Update()
    {
        //registro i valori di spostamento sugli assi del player
        if (x != 0)
            faceX = 1.0f * Mathf.Sign(x);
        else
            faceX = x;
        if (y != 0)
            faceY = 1.0f * Mathf.Sign(y);
        else
            faceY = y;

        //imposto i valori nel blend tree relativo all'animazione di movimento 
        animator.SetFloat("FaceX", faceX);
        animator.SetFloat("FaceY", faceY);
     }


    

}
