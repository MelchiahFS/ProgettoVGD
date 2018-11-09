using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SceneItem : MonoBehaviour {

    Sprite sprite;
    private ItemStats info;

    public ItemStats Info
    {
        get { return info; }
        set { info = value; }
    }

}
