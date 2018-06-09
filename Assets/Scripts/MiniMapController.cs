using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniMapController : MonoBehaviour {

    public static MiniMapController control;
	

	void Awake () {
        if (control == null)
        {
            DontDestroyOnLoad(gameObject);
            control = this;
        }
        else if (control != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);


    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
