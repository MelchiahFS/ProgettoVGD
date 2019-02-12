﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignboardTrigger : MonoBehaviour {

	void OnTriggerStay2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player")
			GameManager.manager.signboardContact = true;
	}

	void OnTriggerExit2D(Collider2D coll)
	{
		if (coll.gameObject.tag == "Player")
			GameManager.manager.signboardContact = false;
	}
}