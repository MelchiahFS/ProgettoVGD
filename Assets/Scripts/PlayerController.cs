using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public float speed;
    private Room[,] rooms;

	private SpriteRenderer spriteRenderer;
	private Animator animator;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }
	  
	void FixedUpdate()
	{
		float x = Input.GetAxisRaw("Horizontal");
		
	    float y = Input.GetAxisRaw("Vertical");

	    Vector2 movement = new Vector2(x, y);

	    rb2d.velocity = movement * speed;

        if (x > 0)
        {
            animator.SetBool("MoveRight", true);
            animator.SetBool("Idle", false);
        }
        else
        {
            if (x == 0)
            {
                animator.SetBool("Idle", true);
                animator.SetBool("MoveRight", false);
                animator.SetBool("MoveLeft", false);
            }
            else
            {
                animator.SetBool("Idle", false);
                animator.SetBool("MoveLeft", true);
                animator.SetBool("MoveRight", false);
            }

        }

    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //Se l'oggetto con cui il player ha sbattuto è l'uscita
        if (other.gameObject.tag == "Exit")
        {
            //Invoco la funzione restart con delay di due secondi
            Invoke("Restart", 2);
        }
    }

    //Ricarica la scena
    private void Restart()
    {
        //Ricarica l'unica scena esistente con modalità Single, per eliminare la scena precedente
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

}


