using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviourSingleton<ResourceManager> {

    [SerializeField]
    List<Sprite> animals;
    public List<Sprite> Animals
    {
        get
        {
            return animals;
        }
    }
}
