using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class RoomChange : MonoBehaviour {

    private Room actualRoom = null, adiacentRoom = null;
    private TileSpriteSelector selector;
    private MiniMapController minimap;
    private int roomSizeX, roomSizeY;


    public bool passUp = false, passDown = false, passLeft = false, passRight = false;

    public bool chRoom = false;
    public bool hasKey = false;

    public float fadeTime = 0.3f;

    public AudioClip doorOpen, doorClosed, doorLocked, doorUnlocked, cantOpen;
    private AudioSource source;

    void Start()
    {
		selector = TileSpriteSelector.tss;
        minimap = GameManager.manager.GetComponent<MiniMapController>();
        actualRoom = GameManager.manager.ActualRoom;
        roomSizeX = GameManager.manager.lvlManager.roomSizeX;
        roomSizeY = GameManager.manager.lvlManager.roomSizeY;
        source = GetComponent<AudioSource>();
	}

    void Update ()
    {
		//se non è in atto un cambio di stanza
        if (!chRoom)
        {
			//se i trigger del corridoio sono entrambi attivi
            if (passUp && passDown)
            {
				//se mi trovo più in basso della stanza attuale
                if (transform.localPosition.y < actualRoom.gridPos.y)
                {
					//allora sono nel corridoio di giù e sto andando nella stanza sottostante
                    StartCoroutine(UpdateRoom('d'));
                }
				//se invece sono più in alto del limite superiore della stanza attuale
                else if (transform.position.y > (actualRoom.gridPos.y + GameManager.manager.lvlManager.roomSizeY))
                {
					//allora sono nel corridoio di sù e sto andando alla stanza superiore
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

            //se il player è effettivamente dentro i confini della stanza (non nei corridoi)
            if (transform.position.x > actualRoom.gridPos.x && transform.position.x < (actualRoom.gridPos.x + roomSizeX)
                && transform.position.y > actualRoom.gridPos.y && transform.position.y < (actualRoom.gridPos.y + roomSizeY))
            {
				//se non sono nella boss room
                if (!actualRoom.bossRoom)
                {
                    //se ci sono nemici da uccidere nella stanza attuale
                    if (actualRoom.enemyWaves > 0)
                    {
						//se le porte ancora non sono chiuse le sigillo
                        if (!actualRoom.locked)
                        {
                            LockRoom(actualRoom);
                            source.PlayOneShot(doorLocked);
                        }
						//istanzio infine i nemici da uccidere
                        GameManager.manager.lvlManager.InstantiateEnemies(GameManager.manager.actualPos.x, GameManager.manager.actualPos.y);
                    }
                }
				//se invece sono nella boss room
                else
                {
					//se ci sono ancora nemici da uccidere
                    if (actualRoom.enemyWaves > 0 && actualRoom.enemyNumber == 0)
                    {
						//se le porte non sono chiuse le sigillo
                        if (!actualRoom.locked)
                        {
                            LockRoom(actualRoom);
                            source.PlayOneShot(doorLocked);
                        }
						//istanzio i nemici
                        GameManager.manager.lvlManager.InstantiateEnemies(GameManager.manager.actualPos.x, GameManager.manager.actualPos.y);
                    }
					//se invece non ci sono più nemici da uccidere mostro il passaggio per il prossimo livello
                    else if (actualRoom.enemyWaves == 0 && actualRoom.enemyNumber == 0)
                    {
						if (GameStats.stats.levelNumber < 5)
							GameManager.manager.lvlManager.ShowExit();
                    }
                }
            }
            //se infine non ci sono più nemici sblocco le porte della stanza
            if (actualRoom.enemyNumber == 0 && actualRoom.enemyWaves == 0 && actualRoom.locked)
            {
                source.PlayOneShot(doorUnlocked);
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
        //rimuovo tutti i gameObject dalla lista dei scene object della stanza attuale
        actualRoom.toSort.Clear();

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

        //aggiorno la stanza attuale per spawnare le eventuali ricompense
        GetComponentInChildren<LootGenerator>().actualRoom = actualRoom;

        //aggiungo il player alla lista degli oggetti da ordinare della nuova stanza
        actualRoom.toSort.Add(gameObject);

        //imposto l'immagine corretta nella minimappa per la nuova stanza
        minimap.SetEnterRoom(actualRoom);

        yield return null;
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        //Se l'oggetto con cui il player ha sbattuto è l'uscita
        if (other.gameObject.tag == "Exit")
        {
			GameStats.stats.levelNumber++;
			StartCoroutine(GameManager.manager.lvlManager.FadeOffToNewScene(1f, "LoadingScreen"));
		}

        
        //se non è in atto un cambio di stanza e la stanza non è sigillata
        if (!chRoom && !actualRoom.locked)
        {
			//se l'oggetto toccato è la porta superiore
            if (other.gameObject.tag == "DoorUp")
            {
				//se sono già entrato nella stanza
                if (actualRoom.visited)
                {
					//se la porta è della boss room
                    if (GameManager.manager.GetAdiacentRoom('u').bossRoom)
                    {
						//se sono in possesso della chiave
                        if (hasKey)
                        {
							//se la porta non è ancora aperta
                            if (!actualRoom.openUp)
                            {
                                source.PlayOneShot(doorUnlocked);
								//apro la porta
                                SetRoomDoor('u', other.gameObject);
								//prendo il riferimento alla stanza sopra
                                adiacentRoom = GameManager.manager.GetAdiacentRoom('u');
								//illumino il passaggio sopra
                                GameManager.manager.lvlManager.LightUpPassage(adiacentRoom.doorSpriteDown, actualRoom.passageUpTiles, true);
                            }
                            
                        }
						//se invece non ho la chiave non posso aprire la porta
                        else if (!actualRoom.openUp)
                        {
                            source.PlayOneShot(cantOpen);
                        }
                    }
					//se la stanza adiacente non è quella del boss
                    else
                    {
						//se la porta non è ancora aperta
						if (!actualRoom.openUp)
                        {
                            source.PlayOneShot(doorOpen);
							//apro la porta
                            SetRoomDoor('u', other.gameObject);
							//prendo il riferimento alla stanza sopra
							adiacentRoom = GameManager.manager.GetAdiacentRoom('u');
							//illumino il passaggio sopra
							GameManager.manager.lvlManager.LightUpPassage(adiacentRoom.doorSpriteDown, actualRoom.passageUpTiles, true);
                        }
                    }
                }
				//altrimenti se ancora non sono entrato (ad esempio sono nel corridoio di su e sto scendendo)
                else
                {
					//se la porta non è aperta
                    if (!actualRoom.openUp)
                    {
                        source.PlayOneShot(doorOpen);
						//apro la porta
                        SetRoomDoor('u', other.gameObject);
						//illumino la stanza attuale e la imposto come visitata
                        GameManager.manager.lvlManager.LightUpRoom(actualRoom, true);
                    }
                }
            }
            else if (other.gameObject.tag == "DoorDown")
            {
                if (actualRoom.visited)
                {
                    if (GameManager.manager.GetAdiacentRoom('d').bossRoom)
                    {
                        if (hasKey)
                        {
                            if (!actualRoom.openDown)
                            {
                                source.PlayOneShot(doorUnlocked);
                                SetRoomDoor('d', other.gameObject);
                                adiacentRoom = GameManager.manager.GetAdiacentRoom('d');
                                GameManager.manager.lvlManager.LightUpPassage(adiacentRoom.doorSpriteUp, actualRoom.passageDownTiles, true);
                            }
                        }
                        else if (!actualRoom.openDown)
                        {
                            source.PlayOneShot(cantOpen);
                        }
                    }
                    else
                    {
                        if (!actualRoom.openDown)
                        {
                            source.PlayOneShot(doorOpen);
                            SetRoomDoor('d', other.gameObject);
                            adiacentRoom = GameManager.manager.GetAdiacentRoom('d');
                            GameManager.manager.lvlManager.LightUpPassage(adiacentRoom.doorSpriteUp, actualRoom.passageDownTiles, true);
                        }
                    }
                }
                else
                {
                    if (!actualRoom.openDown)
                    {
                        source.PlayOneShot(doorOpen);
                        SetRoomDoor('d', other.gameObject);
                        GameManager.manager.lvlManager.LightUpRoom(actualRoom, true);
                    }
                }
            }
            else if (other.gameObject.tag == "DoorLeft")
            {
                if (actualRoom.visited)
                {
                    if (GameManager.manager.GetAdiacentRoom('l').bossRoom)
                    {
                        if (hasKey)
                        {
                            if (!actualRoom.openLeft)
                            {
                                source.PlayOneShot(doorUnlocked);
                                SetRoomDoor('l', other.gameObject);
                                adiacentRoom = GameManager.manager.GetAdiacentRoom('l');
                                GameManager.manager.lvlManager.LightUpPassage(adiacentRoom.doorSpriteRight, actualRoom.passageLeftTiles, true);
                            }
                        }
                        else if (!actualRoom.openLeft)
                        {
                            source.PlayOneShot(cantOpen);
                        }
                    }
                    else
                    {
                        if (!actualRoom.openLeft)
                        {
                            source.PlayOneShot(doorOpen);
                            SetRoomDoor('l', other.gameObject);
                            adiacentRoom = GameManager.manager.GetAdiacentRoom('l');
                            GameManager.manager.lvlManager.LightUpPassage(adiacentRoom.doorSpriteRight, actualRoom.passageLeftTiles, true);
                        }
                    }
                }
                else
                {
                    if (!actualRoom.openLeft)
                    {
                        source.PlayOneShot(doorOpen);
                        SetRoomDoor('l', other.gameObject);
                        GameManager.manager.lvlManager.LightUpRoom(actualRoom, true);
                    }
                }
            }
            else if (other.gameObject.tag == "DoorRight")
            {
                if (actualRoom.visited)
                {
                    if (GameManager.manager.GetAdiacentRoom('r').bossRoom)
                    {
                        if (hasKey)
                        {
                            if (!actualRoom.openRight)
                            {
                                source.PlayOneShot(doorUnlocked);
                                SetRoomDoor('r', other.gameObject);
                                adiacentRoom = GameManager.manager.GetAdiacentRoom('r');
                                GameManager.manager.lvlManager.LightUpPassage(adiacentRoom.doorSpriteLeft, actualRoom.passageRightTiles, true);
                            }
                        }
                        else if (!actualRoom.openRight)
                        {
                            source.PlayOneShot(cantOpen);
                        }
                    }
                    else
                    {
                        if (!actualRoom.openRight)
                        {
                            source.PlayOneShot(doorOpen);
                            SetRoomDoor('r', other.gameObject);
                            adiacentRoom = GameManager.manager.GetAdiacentRoom('r');
                            GameManager.manager.lvlManager.LightUpPassage(adiacentRoom.doorSpriteLeft, actualRoom.passageRightTiles, true);
                        }
                    }
                }
                else
                {
                    if (!actualRoom.openRight)
                    {
                        source.PlayOneShot(doorOpen);
                        SetRoomDoor('r', other.gameObject);
                        GameManager.manager.lvlManager.LightUpRoom(actualRoom, true);
                    }
                }
            }

        }
        else if (actualRoom.locked)
        {
            if (other.gameObject.tag == "DoorUp" || other.gameObject.tag == "DoorDown" || other.gameObject.tag == "DoorLeft" || other.gameObject.tag == "DoorRight")
            {
                source.PlayOneShot(cantOpen);
            }
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
        if (doorPos == 'u')
        {
            if (actualRoom.openUp)
            {
                door.GetComponent<SpriteRenderer>().sprite = selector.closedDoorUp;
                door.GetComponent<BoxCollider2D>().enabled = true;
                actualRoom.openUp = false;
            }
            else
            {
                door.GetComponent<SpriteRenderer>().sprite = selector.openDoorUp;
                door.GetComponent<BoxCollider2D>().enabled = false;
                actualRoom.openUp = true;
            }

        }
        else if (doorPos == 'd')
        {
            if (actualRoom.openDown)
            {
                door.GetComponent<SpriteRenderer>().sprite = selector.closedDoorDown;
                door.GetComponent<BoxCollider2D>().enabled = true;
                actualRoom.openDown = false;
            }
            else
            {
                door.GetComponent<SpriteRenderer>().sprite = selector.openDoorDown;
                door.GetComponent<BoxCollider2D>().enabled = false;
                actualRoom.openDown = true;
            }
        }
        else if (doorPos == 'l')
        {
            if (actualRoom.openLeft)
            {
                door.GetComponent<SpriteRenderer>().sprite = selector.closedDoorLeft;
                door.GetComponent<BoxCollider2D>().enabled = true;
                actualRoom.openLeft = false;
            }
            else
            {
                door.GetComponent<SpriteRenderer>().sprite = selector.openDoorLeft;
                door.GetComponent<BoxCollider2D>().enabled = false;
                actualRoom.openLeft = true;
            }
        }
        else if (doorPos == 'r')
        {
            if (actualRoom.openRight)
            {
                door.GetComponent<SpriteRenderer>().sprite = selector.closedDoorRight;
                door.GetComponent<BoxCollider2D>().enabled = true;
                actualRoom.openRight = false;
            }
            else
            {
                door.GetComponent<SpriteRenderer>().sprite = selector.openDoorRight;
                door.GetComponent<BoxCollider2D>().enabled = false;
                actualRoom.openRight = true;
            }
        }
    }

    //aggiorna il counter dei nemici; quando è a zero le porte si sbloccano
    public void DecreaseEnemyCounter()
    {
        if (actualRoom.enemyNumber > 0)
            actualRoom.enemyNumber--;
    }
}
