using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class ShoppingItem : MonoBehaviour
{
    XRGrabInteractable XRGrab;
    BoxCollider hitbox;
    bool released;
    public GameObject selfRef;
    public ShelfManager shelfParent;
    Vector3 spawnLocation;

    private void OnCollisionEnter(Collision collision)
    {
        
        //if (XRGrab.selectingInteractor == null)
        if (collision.collider.CompareTag("basket") && released)
        {
            collision.collider.GetComponentInParent<ShoppingList>().placeItem(this);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        XRGrab = GetComponentInParent<XRGrabInteractable>();
        hitbox = GetComponentInParent<BoxCollider>();
        shelfParent = GetComponentInParent<ShelfManager>();
        released = false;
        spawnLocation = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ReleaseItem()
    {
        StartCoroutine("SetRelease");
    }

    IEnumerator SetRelease()
    {
        released = true;
        yield return new WaitForSeconds(0.5f);
        released = false;
        
        // reset position if the item is dropped outside of the basket
        if (selfRef.activeSelf)
        {
            transform.position = spawnLocation;
            transform.rotation = Quaternion.identity;
            GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
