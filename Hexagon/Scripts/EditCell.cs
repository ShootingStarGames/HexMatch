using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditCell : HexCell
{

    private void Awake()
    {
        isActive = true;    
    }
    public void Toggle()
    {
        isActive = !isActive;
        if(!isActive)
            gameObject.GetComponent<MeshRenderer>().material.color = Color.grey;
        else
            gameObject.GetComponent<MeshRenderer>().material.color = Color.white;
    }
}
