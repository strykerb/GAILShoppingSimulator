using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHead : MonoBehaviour
{
    [SerializeField] Transform Head;
    [SerializeField] float yOffset;
    [SerializeField] float zOffset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.rotation = Quaternion.identity;
        transform.position = new Vector3(Head.position.x, Head.position.y - yOffset, Head.position.z - zOffset);
    }
}
