using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpriteSelector : MonoBehaviour
{
	public static ItemSpriteSelector iss;

	//liste relative alle sprite degli oggetti e armi generati e dei proiettili nemici
    public List<Sprite> meeleWeapons;
    public List<Sprite> rangedWeapons;
    public List<Sprite> potions;
    public List<Sprite> money;
    public List<Sprite> scrolls;
    public List<Sprite> books;
    public List<Sprite> keys;
	public List<Sprite> bullets;

	void Awake()
	{
		if (iss == null)
		{
			iss = this;
		}
		else if (iss != this)
		{
			Destroy(gameObject);
		}
	}
}
