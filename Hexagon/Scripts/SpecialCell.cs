using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialCell : ObjectCell
{
    int count;
    public override void Init(int y, int x, ObjectCellType objectCellType)
    {
        count = Random.Range(1, 4);

        base.Init(y, x, objectCellType);
    }

    protected override void SetSprite()
    {
        Sprite sprite = null;
        if(count > 0)
            sprite = ResourceManager.Instance.Animals[ResourceManager.Instance.Animals.Count - count];
        GetComponent<SpriteRenderer>().sprite = sprite;
        GetComponent<SpriteRenderer>().color = Color.green;
    }

    public void Effect()
    {
        count--;
        if (count <= 0)
            matched = true;
        SetSprite();
    }
}