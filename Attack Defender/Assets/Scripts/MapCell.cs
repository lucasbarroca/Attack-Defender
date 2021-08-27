using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCell : MonoBehaviour
{
    public GameObject model;

    DefenseStructure _structure;
    public DefenseStructure structure
    {
        get
        {
            return _structure;
        }
        set
        {
            _structure = value;
        }
    }

    public bool hasStructure
    {
        get
        {
            if (structure != null)
            {
                return !structure.isDead;
            }
            else
            {
                return false;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ResizeObject(Vector3 size)
    {
        if (model != null)
        {
            // Resize
            model.transform.localScale = size;

            // Reposition
            model.transform.localPosition = size / 2;
        }
    }
}
