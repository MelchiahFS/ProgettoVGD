using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileTrigger : MonoBehaviour
{
    private RoomChange rc;

    void Start()
    {
        rc = GameManager.manager.playerReference.GetComponent<RoomChange>();
    }

    //i trigger attivati permettono di aggiornare correttamente la posizione del player nelle stanze;
    //inoltre permettono di decidere il momento in cui intrappolare il player nelle stanze e spawnare i nemici
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player" && !other.isTrigger)
        {
            if (gameObject.tag == "outerDoorUp")
            {
                rc.passUp = true;
            }
            else if (gameObject.tag == "innerDoorUp")
            {
                rc.passUp = false;
                rc.passDown = false;
            }
            else if (gameObject.tag == "outerDoorDown")
            {
                rc.passDown = true;
            }
            else if (gameObject.tag == "innerDoorDown")
            {
                rc.passUp = false;
                rc.passDown = false;
            }
            else if (gameObject.tag == "outerDoorLeft")
            {
                rc.passLeft = true;
            }
            else if (gameObject.tag == "innerDoorleft")
            {
                rc.passLeft = false;
                rc.passRight = false;
            }
            else if (gameObject.tag == "outerDoorRight")
            {
                rc.passRight = true;
            }
            else if (gameObject.tag == "innerDoorRight")
            {
                rc.passLeft = false;
                rc.passRight = false;
            }
        }
        
    }
}
