using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BorderCell : HexCell
{
    public void Init(bool isActive)
    {
        this.isActive = isActive;
        if(!isActive)
            gameObject.GetComponent<MeshRenderer>().material.color = Color.clear;


    }
}