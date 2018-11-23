using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class PlayerController : Character
{
    private Rigidbody2D rb2d;
    public float speed;
    private Room actualRoom = null, adiacentRoom = null;
	private Animator animator;
    private int roomSizeX, roomSizeY;
    private GameObject hitbox;
    private PlayerHealth ph;
    private Weapon weapon;
    private float faceX, faceY;
    
    private bool passUp = false, passDown = false, passLeft = false, passRight = false;

    bool setDoor = false, chRoom = false;
    float x, y;

    public float fadeTime = 0.3f;

    private void Start()
    {
        ph = GetComponent<PlayerHealth>();
        weapon = GetComponentInChildren<Weapon>();
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        actualRoom = GameManager.manager.ActualRoom;
        roomSizeX = GameManager.manager.lvlManager.roomSizeX;
        roomSizeY = GameManager.manager.lvlManager.roomSizeY;
        SetRealOffset(gameObject);
    }
	
	void FixedUpdate()
	{
		x = Input.GetAxisRaw("CustomHorizontal");
		
	    y = Input.GetAxisRaw("CustomVertical");

        
	    Vector2 movement = new Vector2(x, y);

        //se il player attacca non può muoversi
        if (!weapon.isAttacking && !ph.isDead)
            rb2d.velocity = movement * speed;
        else
            rb2d.velocity = movement * 0;

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
