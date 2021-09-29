using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CopyHeadRotation : MonoBehaviour
{
    public Transform head;
    public float height;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.Euler(0, head.rotation.eulerAngles.y, 0);
        transform.position.Set(transform.position.x, height, transform.position.z);
    }
}
