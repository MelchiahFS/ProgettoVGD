using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsController : MonoBehaviour {

    private int health = 3;
    public GameObject heart1 = null, heart2 = null, heart3 = null;


    // Use this for initialization
    void Start ()
    {
        //heart1 = GameObject.Find("heart1");
        //heart2 = GameObject.Find("heart2");
        //heart3 = GameObject.Find("heart3");
        //heart1.SetActive(true);
        //heart2.SetActive(true);
        //heart3.SetActive(true);
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            switch (health)
            {
                case 1:
                    heart1.SetActive(false);
                    health--;
                    break;

                case 2:
                    heart2.SetActive(false);
                    health--;
                    break;

                case 3:
                    heart3.SetActive(false);
                    health--;
                    break;
            }
        }
        if (Input.GetKeyDown(KeyCode.KeypadEnter))
        {
            switch (health)
            {
                case 0:
                    heart1.SetActive(true);
                    health++;
                    break;

                case 1:
                    heart2.SetActive(true);
                    health++;
                    break;

                case 2:
                    heart3.SetActive(true);
                    health++;
                    break;
            }
        }
    }
}
