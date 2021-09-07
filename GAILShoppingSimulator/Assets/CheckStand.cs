using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CheckStand : MonoBehaviour
{
    [SerializeField] Text BasketText;
    [SerializeField] Text WaitText;
    [SerializeField] GameObject BasketLandingArea;
    [SerializeField] GameObject BasketSign;
    [SerializeField] GameObject Basket;
    public bool busy = false;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("basket") && !busy)
        {
            if (collision.collider.GetComponentInParent<ShoppingList>().shoppingComplete)
            {
                Debug.Log("checking out.");
                StartCoroutine("CheckOut");
                collision.collider.GetComponentInParent<ShoppingList>().dataExporter.SaveData();
                collision.collider.GetComponentInParent<ShoppingList>().gameObject.SetActive(false);
            }
            
        }
    }

    IEnumerator CheckOut()
    {
        SetBusy(true);
        yield return new WaitForSeconds(10f);
        SetBusy(false);
    }

    void SetBusy(bool isBusy)
    {
        busy = isBusy;
        WaitText.enabled = isBusy;
        BasketText.enabled = !isBusy;
        Basket.SetActive(isBusy);
        BasketLandingArea.SetActive(!isBusy);
        BasketSign.SetActive(!isBusy);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
