﻿using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class PlayerController : Character
{
    private Rigidbody2D rb2d;
    public float speed;
    private Room actualRoom = null, adiacentRoom = null;
	private Animator animator;
    private TileSpriteSelector selector;
    private MiniMapController minimap;
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
        minimap = GameManager.manager.GetComponent<MiniMapController>();
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
        if (x != 0)
            faceX = 1.0f * Mathf.Sign(x);
        else
            faceX = x;
        if (y != 0)
            faceY = 1.0f * Mathf.Sign(y);
        else
            faceY = y;

        animator.SetFloat("FaceX", faceX);
        animator.SetFloat("FaceY", faceY);

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
                    foreach (GameObject g in actualRoom.enemies)
                    {
                        g.SetActive(true);
                        StartCoroutine(GameManager.manager.lvlManager.FadeIn(g.GetComponent<SpriteRenderer>(), fadeTime));          
                        //StartCoroutine(GameManager.manager.lvlManager.FadeIn(g.GetComponentInChildren<SpriteRenderer>(), fadeTime));AAAAAAAAAAAAAAAA
                    }
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
        //rimuovo il player dalla lista dei nemici
        actualRoom.enemies.Remove(gameObject);

        //imposto l'immagine corretta nella minimappa per la stanza lasciata
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

        //aggiorno la stanza attuale per il corretto rendering dei character
        GetComponent<SortRenderingOrder>().actualRoom = actualRoom;
        //aggiungo il player alla lista dei nemici della nuova stanza
        actualRoom.enemies.Add(gameObject);

        //imposto l'immagine corretta nella minimappa per la nuova stanza
        minimap.SetEnterRoom(actualRoom);

        yield return null;
    }


    private void OnCollisionEnter2D(Collision2D other)
    {
        //Se l'oggetto con cui il player ha sbattuto è l'uscita
        if (other.gameObject.tag == "Exit")
        {
            //Invoco la funzione restart con delay di due secondi
            GameManager.manager.Invoke("Restart", 2);
        }

        //se non è in atto un cambio di stanza e la stanza non è sigillata
        if (!setDoor && !chRoom && !actualRoom.locked)
        {
            if (other.gameObject.tag == "DoorUp")
            {
                //se la porta non è ancora aperta
                if (!actualRoom.openUp)
                {
                    //allora la apro
                    SetRoomDoor('u', other.gameObject);
                    //se la stanza attuale non è ancora stata visitata la illumino (entro in una stanza nuova)
                    if (!actualRoom.visited)
                        GameManager.manager.lvlManager.LightUpRoom(actualRoom, true);
                    //altrimenti sto uscendo dalla stanza e recupero le info sulla stanza successiva
                    else
                    {
                        adiacentRoom = GameManager.manager.GetAdiacentRoom('u');
                        //se non ho ancora visitato la stanza successiva allora illumino il passaggio che la collega alla stanza attuale
                        if (!adiacentRoom.visited)
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
        actualRoom.locked = true; //finché resta vero le porte non possono essere aperte

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

    //aggiorna il counter dei nemici; quando è a zero le porte si sbloccano
    public void DecreaseEnemyCounter()
    {
        actualRoom.enemyCounter--;
    }

}
