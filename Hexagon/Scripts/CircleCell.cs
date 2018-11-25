using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircleCell : ObjectCell
{

    protected override void SetSprite()
    {
        Sprite sprite = null;
        sprite = ResourceManager.Instance.Animals[(int)objectCellType];
        GetComponent<SpriteRenderer>().sprite = sprite;
        GetComponent<SpriteRenderer>().color = Color.white;
    }
}

public class BoomCell : CircleCell
{
    protected override void SetSprite()
    {
        Sprite sprite = null;
        sprite = ResourceManager.Instance.Animals[(int)objectCellType];
        GetComponent<SpriteRenderer>().sprite = sprite;
        GetComponent<SpriteRenderer>().color = Color.cyan;
    }
}