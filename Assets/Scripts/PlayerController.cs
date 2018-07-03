using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb2d;
    public float speed;
    private Room actualRoom = null, adiacentRoom = null;

	private Animator animator;
    private TileSpriteSelector selector;
    private MiniMapController minimap;

    private bool passUp = false, passDown = false, passLeft = false, passRight = false;

    bool setDoor = false, chRoom = false;

    private void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        minimap = GameManager.manager.GetComponent<MiniMapController>();
        actualRoom = GameManager.manager.ActualRoom;
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
			animator.SetBool("MoveLeft", false);
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

    void Update()
    {
        if (!chRoom)
        {
            if (passUp && passDown)
            {
                if (transform.localPosition.y < actualRoom.gridPos.y)
                {
                    StartCoroutine(UpdateRoom('d'));
                }
                else if (transform.position.y > (actualRoom.gridPos.y + GameManager.manager.lvlManager.roomSizeY))
                {
                    StartCoroutine(UpdateRoom('u'));
                }
            }
            else if (passLeft && passRight)
            {
                if (transform.position.x < actualRoom.gridPos.x)
                {
                    StartCoroutine(UpdateRoom('l'));
                }
                else if (transform.position.x > (actualRoom.gridPos.x + GameManager.manager.lvlManager.roomSizeX))
                {
                    StartCoroutine(UpdateRoom('r'));
                }
            }
        }        
    }

    //coroutine che esegue ChangeRoom
    IEnumerator UpdateRoom(char c)
    {
        chRoom = true;
        yield return StartCoroutine(ChangeRoom(c));
        chRoom = false;
    }

    //aggiorna la stanza attuale 
    IEnumerator ChangeRoom(char c)
    {
        minimap.SetExitRoom(actualRoom);
        if (c == 'd')
        {
            passDown = false;
            GameManager.manager.UpdateActualRoom('d');
            actualRoom = GameManager.manager.ActualRoom;
        }
        else if (c == 'u')
        {
            passUp = false;
            GameManager.manager.UpdateActualRoom('u');
            actualRoom = GameManager.manager.ActualRoom;
        }
        
        else if (c == 'l')
        {
            passLeft = false;
            GameManager.manager.UpdateActualRoom('l');
            actualRoom = GameManager.manager.ActualRoom;
        }
        else if (c == 'r')
        {
            passRight = false;
            GameManager.manager.UpdateActualRoom('r');
            actualRoom = GameManager.manager.ActualRoom;
        }
        minimap.SetEnterRoom(actualRoom);

        //GameManager.manager.lvlManager.LightUpRoom(actualRoom, true);
        yield return null;
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        //Se l'oggetto con cui il player ha sbattuto è l'uscita
        if (other.gameObject.tag == "Exit")
        {
            //Invoco la funzione restart con delay di due secondi
            Invoke("Restart", 2);
        }

        if (!setDoor && !chRoom)
        {
            if (other.gameObject.tag == "DoorUp")
            {
                if (!actualRoom.openUp)
                {
                    SetRoomDoor('u', other.gameObject);
                    if (!actualRoom.visited)
                        GameManager.manager.lvlManager.LightUpRoom(actualRoom, true);
                    else
                    {
                        adiacentRoom = GameManager.manager.GetAdiacentRoom('u');
                        GameManager.manager.lvlManager.LightUpPassage(adiacentRoom.doorSpriteDown, actualRoom.passageUpTiles, true);
                    }
                    
                }
            }
            else if (other.gameObject.tag == "DoorDown")
            {
                if (!actualRoom.openDown)
                {
                    SetRoomDoor('d', other.gameObject);
                    if (!actualRoom.visited)
                        GameManager.manager.lvlManager.LightUpRoom(actualRoom, true);
                    else
                    {
                        adiacentRoom = GameManager.manager.GetAdiacentRoom('d');
                        GameManager.manager.lvlManager.LightUpPassage(adiacentRoom.doorSpriteUp, actualRoom.passageDownTiles, true);
                    }
                    
                }
            }
            else if (other.gameObject.tag == "DoorLeft")
            {
                if (!actualRoom.openLeft)
                {
                    SetRoomDoor('l', other.gameObject);
                    if (!actualRoom.visited)
                        GameManager.manager.lvlManager.LightUpRoom(actualRoom, true);
                    else
                    {
                        adiacentRoom = GameManager.manager.GetAdiacentRoom('l');
                        GameManager.manager.lvlManager.LightUpPassage(adiacentRoom.doorSpriteRight, actualRoom.passageLeftTiles, true);
                    }
                    
                }
            }
            else if (other.gameObject.tag == "DoorRight")
            {
                if (!actualRoom.openRight)
                {
                    SetRoomDoor('r', other.gameObject);
                    if (!actualRoom.visited)
                        GameManager.manager.lvlManager.LightUpRoom(actualRoom, true);
                    else
                    {
                        adiacentRoom = GameManager.manager.GetAdiacentRoom('r');
                        GameManager.manager.lvlManager.LightUpPassage(adiacentRoom.doorSpriteLeft, actualRoom.passageRightTiles, true);
                    }
                    adiacentRoom = GameManager.manager.GetAdiacentRoom('r');
                    GameManager.manager.lvlManager.LightUpPassage(adiacentRoom.doorSpriteLeft, actualRoom.passageRightTiles, true);
                }
            }
        }
    }
    
    //i trigger attivati permettono di aggiornare correttamente la posizione del player nelle stanze
    public void OnTriggerEnter2D(Collider2D doorTrigger)
    {
        if (doorTrigger.gameObject.tag == "innerDoorUp") 
        {
            passUp = false;
        }
        else if (doorTrigger.gameObject.tag == "outerDoorUp")
        {
            passUp = true;
        }
        else if (doorTrigger.gameObject.tag == "innerDoorDown") 
        {
            passDown = false;
        }
        else if (doorTrigger.gameObject.tag == "outerDoorDown")
        {
             passDown = true;
        }
        else if (doorTrigger.gameObject.tag == "innerDoorLeft") 
        {
            passLeft = false;
        }
        else if (doorTrigger.gameObject.tag == "outerDoorLeft")
        {
            passLeft = true;
        }
        else if (doorTrigger.gameObject.tag == "innerDoorRight") 
        {
            passRight = false;
        }
        else if (doorTrigger.gameObject.tag == "outerDoorRight")
        {
            passRight = true;
        }

    }

    //Apre o chiude le porte
    public void SetRoomDoor(char doorPos, GameObject door)
    {
        setDoor = true;
        if (doorPos == 'u')
        {
            if (actualRoom.openUp)
            {
                selector = door.GetComponent<TileSpriteSelector>();
                door.GetComponent<SpriteRenderer>().sprite = selector.closedDoorUp;
                door.GetComponent<BoxCollider2D>().enabled = true;
                actualRoom.openUp = false;
            }
            else
            {
                selector = door.GetComponent<TileSpriteSelector>();
                door.GetComponent<SpriteRenderer>().sprite = selector.openDoorUp;
                door.GetComponent<BoxCollider2D>().enabled = false;
                actualRoom.openUp = true;
            }

        }
        else if (doorPos == 'd')
        {
            if (actualRoom.openDown)
            {
                selector = door.GetComponent<TileSpriteSelector>();
                door.GetComponent<SpriteRenderer>().sprite = selector.closedDoorDown;
                door.GetComponent<BoxCollider2D>().enabled = true;
                actualRoom.openDown = false;
            }
            else
            {
                selector = door.GetComponent<TileSpriteSelector>();
                door.GetComponent<SpriteRenderer>().sprite = selector.openDoorDown;
                door.GetComponent<BoxCollider2D>().enabled = false;
                actualRoom.openDown = true;
            }
        }
        else if (doorPos == 'l')
        {
            if (actualRoom.openLeft)
            {
                selector = door.GetComponent<TileSpriteSelector>();
                door.GetComponent<SpriteRenderer>().sprite = selector.closedDoorLeft;
                door.GetComponent<BoxCollider2D>().enabled = true;
                actualRoom.openLeft = false;
            }
            else
            {
                selector = door.GetComponent<TileSpriteSelector>();
                door.GetComponent<SpriteRenderer>().sprite = selector.openDoorLeft;
                door.GetComponent<BoxCollider2D>().enabled = false;
                actualRoom.openLeft = true;
            }
        }
        else if (doorPos == 'r')
        {
            if (actualRoom.openRight)
            {
                selector = door.GetComponent<TileSpriteSelector>();
                door.GetComponent<SpriteRenderer>().sprite = selector.closedDoorRight;
                door.GetComponent<BoxCollider2D>().enabled = true;
                actualRoom.openRight = false;
            }
            else
            {
                selector = door.GetComponent<TileSpriteSelector>();
                door.GetComponent<SpriteRenderer>().sprite = selector.openDoorRight;
                door.GetComponent<BoxCollider2D>().enabled = false;
                actualRoom.openRight = true;
            }
        }
        setDoor = false;
    }

    //Ricarica la scena
    private void Restart()
    {
        //Ricarica l'unica scena esistente con modalità Single, per eliminare la scena precedente
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex, LoadSceneMode.Single);
    }

}