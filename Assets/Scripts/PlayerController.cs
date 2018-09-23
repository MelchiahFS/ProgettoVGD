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
    private int roomSizeX, roomSizeY;
    private GameObject hitbox;
    private GameObject weapon;
    
    private bool passUp = false, passDown = false, passLeft = false, passRight = false;

    bool setDoor = false, chRoom = false;
    float x, y;

    private void Start()
    {
        hitbox = gameObject.transform.Find("Hitbox").gameObject;
        rb2d = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        minimap = GameManager.manager.GetComponent<MiniMapController>();
        actualRoom = GameManager.manager.ActualRoom;
        roomSizeX = GameManager.manager.lvlManager.roomSizeX;
        roomSizeY = GameManager.manager.lvlManager.roomSizeY;
        
    }
	
	void FixedUpdate()
	{
		x = Input.GetAxisRaw("CustomHorizontal");
		
	    y = Input.GetAxisRaw("CustomVertical");

	    Vector2 movement = new Vector2(x, y);

	    rb2d.velocity = movement * speed;

    }

    void Update()
    {
        if (x == 0 && y == 0)
        {
            animator.SetBool("MoveRight", false);
            animator.SetBool("Idle", true);
            animator.SetBool("MoveLeft", false);
            animator.SetBool("MoveDown", false);
            animator.SetBool("MoveUp", false);

            animator.SetBool("SlashLeft", false);
            animator.SetBool("SlashRight", false);
            animator.SetBool("SlashUp", false);
            animator.SetBool("SlashDown", false);
        }
        else if (y == 0 && x > 0)
        {
            animator.SetBool("MoveRight", true);
            animator.SetBool("Idle", false);
            animator.SetBool("MoveLeft", false);
            animator.SetBool("MoveDown", false);
            animator.SetBool("MoveUp", false);

            animator.SetBool("SlashLeft", false);
            animator.SetBool("SlashRight", false);
            animator.SetBool("SlashUp", false);
            animator.SetBool("SlashDown", false);
        }
        else if (y == 0 && x < 0)
        {
            animator.SetBool("MoveRight", false);
            animator.SetBool("Idle", false);
            animator.SetBool("MoveLeft", true);
            animator.SetBool("MoveDown", false);
            animator.SetBool("MoveUp", false);

            animator.SetBool("SlashLeft", false);
            animator.SetBool("SlashRight", false);
            animator.SetBool("SlashUp", false);
            animator.SetBool("SlashDown", false);
        }
        else if (y > 0)
        {
            animator.SetBool("MoveRight", false);
            animator.SetBool("Idle", false);
            animator.SetBool("MoveLeft", false);
            animator.SetBool("MoveDown", false);
            animator.SetBool("MoveUp", true);

            animator.SetBool("SlashLeft", false);
            animator.SetBool("SlashRight", false);
            animator.SetBool("SlashUp", false);
            animator.SetBool("SlashDown", false);
        }
        else if (y < 0)
        {
            animator.SetBool("MoveRight", false);
            animator.SetBool("Idle", false);
            animator.SetBool("MoveLeft", false);
            animator.SetBool("MoveDown", true);
            animator.SetBool("MoveUp", false);

            animator.SetBool("SlashLeft", false);
            animator.SetBool("SlashRight", false);
            animator.SetBool("SlashUp", false);
            animator.SetBool("SlashDown", false);
        }

        if (Input.GetKeyDown("up"))
        {
            animator.SetBool("SlashUp", true);
            animator.SetBool("Idle", false);
            animator.SetBool("SlashLeft", false);
            animator.SetBool("SlashRight", false);
            animator.SetBool("SlashDown", false);

            animator.SetBool("MoveRight", false);
            animator.SetBool("Idle", false);
            animator.SetBool("MoveLeft", false);
            animator.SetBool("MoveDown", false);
            animator.SetBool("MoveUp", false);
        }
        else if (Input.GetKeyDown("down"))
        {
            animator.SetBool("SlashDown", true);
            animator.SetBool("Idle", false);
            animator.SetBool("SlashLeft", false);
            animator.SetBool("SlashRight", false);
            animator.SetBool("SlashUp", false);

            animator.SetBool("MoveRight", false);
            animator.SetBool("Idle", false);
            animator.SetBool("MoveLeft", false);
            animator.SetBool("MoveDown", false);
            animator.SetBool("MoveUp", false);
        }
        else if (Input.GetKeyDown("left"))
        {
            animator.SetBool("SlashLeft", true);
            animator.SetBool("Idle", false);
            animator.SetBool("SlashRight", false);
            animator.SetBool("SlashUp", false);
            animator.SetBool("SlashDown", false);

            animator.SetBool("MoveRight", false);
            animator.SetBool("Idle", false);
            animator.SetBool("MoveLeft", false);
            animator.SetBool("MoveDown", false);
            animator.SetBool("MoveUp", false);
        }
        else if (Input.GetKeyDown("right"))
        {
            animator.SetBool("SlashRight", true);
            animator.SetBool("Idle", false);
            animator.SetBool("SlashLeft", false);
            animator.SetBool("SlashUp", false);
            animator.SetBool("SlashDown", false);

            animator.SetBool("MoveRight", false);
            animator.SetBool("Idle", false);
            animator.SetBool("MoveLeft", false);
            animator.SetBool("MoveDown", false);
            animator.SetBool("MoveUp", false);
        }

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
            //se ci sono nemici nella stanza attuale e le porte non sono chiuse
            if (actualRoom.enemyCounter > 0 && !actualRoom.locked) 
            {
                //se il player è effettivamente dentro la stanza allora sigillo la stanza
                if (transform.position.x > actualRoom.gridPos.x && transform.position.x < (actualRoom.gridPos.x + roomSizeX)
                    && transform.position.y > actualRoom.gridPos.y && transform.position.y < (actualRoom.gridPos.y + roomSizeY))
                {
                    LockRoom(actualRoom);
                }
            }
            //se non ci sono più nemici sblocco le porte della stanza
            else if (actualRoom.enemyCounter == 0)
            {
                actualRoom.locked = false;
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

        if (!setDoor && !chRoom && !actualRoom.locked)
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
    
    //i trigger attivati permettono di aggiornare correttamente la posizione del player nelle stanze;
    //inoltre permettono di decidere il momento in cui intrappolare il player nelle stanze e spawnare i nemici
    private void OnTriggerEnter2D(Collider2D trigger)
    {
        if (trigger.gameObject.tag == "innerDoorUp") 
        {
            passUp = false;
        }
        else if (trigger.gameObject.tag == "outerDoorUp")
        {
            passUp = true;
        }
        else if (trigger.gameObject.tag == "innerDoorDown") 
        {
            passDown = false;
        }
        else if (trigger.gameObject.tag == "outerDoorDown")
        {
             passDown = true;
        }
        else if (trigger.gameObject.tag == "innerDoorLeft") 
        {
            passLeft = false;
        }
        else if (trigger.gameObject.tag == "outerDoorLeft")
        {
            passLeft = true;
        }
        else if (trigger.gameObject.tag == "innerDoorRight") 
        {
            passRight = false;
        }
        else if (trigger.gameObject.tag == "outerDoorRight")
        {
            passRight = true;
        }
        /*else if (trigger.gameObject.tag == "weapon")
        {
            PickWeapon(trigger.gameObject);
        }*/

    }

    //controlla quale porta è stata aperta per entrare nella stanza e la richiude,
    //sigillando poi tutte le stanze finché restano nemici vivi nella stanza
    private void LockRoom(Room room)
    {
        if (actualRoom.openUp)
        {
            SetRoomDoor('u', actualRoom.doorSpriteUp);
        }
        if (actualRoom.openDown)
        {
            SetRoomDoor('d', actualRoom.doorSpriteDown);
        }
        if (actualRoom.openLeft)
        {
            SetRoomDoor('l', actualRoom.doorSpriteLeft);
        }
        if (actualRoom.openRight)
        {
            SetRoomDoor('r', actualRoom.doorSpriteRight);
        }
        actualRoom.locked = true;

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

    /*private void PickWeapon(GameObject weapon)
    {

    }*/

}
