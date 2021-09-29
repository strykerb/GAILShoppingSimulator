using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationCloner : MonoBehaviour
{
    public ShoppingAgent original;
    
    public void SetCloneOrigin(ShoppingAgent origin)
    {
        origin.clones.Add(this);
        original = origin;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (original == null)
        {
            return;
        }
        transform.localPosition = original.transform.localPosition;
        transform.rotation = original.transform.rotation;
    }
}
