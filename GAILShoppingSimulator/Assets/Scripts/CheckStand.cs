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
    public CloneManager manager;
    public bool busy = false;
    public bool VRMode = false;
    public ShoppingList client;

    
    // Start is called before the first frame update
    void Start()
    {
        manager = GetComponentInParent<CloneManager>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("basket") && !busy)
        {
            client = collision.collider.GetComponentInParent<ShoppingList>();
            if (client.shoppingComplete && !client.checkedOut)
            {
                StartCheckout();
            }
            
        }
    }

    public void StartCheckout()
    {
        Debug.Log("checking out.");
        client.basketModel.SetActive(false);
        client.agent.CheckOut();
        StartCoroutine("CheckOut");
    }

    IEnumerator CheckOut()
    {
        if (manager)
        {
            manager.SetCheckStandBusy(true);
        } 
        else
        {
            SetBusy(true);
        }
        yield return new WaitForSeconds(10f);
        if (manager)
        {
            manager.SetCheckStandBusy(false);
        }
        else
        {
            SetBusy(false);
        }

        client.checkedOut = true;
        client.basketModel.SetActive(true);
        client.agent.UpdateWorldTags();
        client = null;
    }

    public void SetBusy(bool isBusy)
    {
        busy = isBusy;
        WaitText.enabled = isBusy;
        BasketText.enabled = !isBusy;
        Basket.SetActive(isBusy);
        BasketLandingArea.SetActive(!isBusy);
        BasketSign.SetActive(!isBusy);
    }

    public void SetAsGoal(bool goal)
    {
        if (goal)
        {
            this.tag = "goal";
            foreach (Collider col in GetComponentsInChildren<BoxCollider>())
            {
                col.gameObject.tag = "goal";
            }
        } else
        {
            this.tag = "wall";
            foreach (Collider col in GetComponentsInChildren<BoxCollider>())
            {
                col.gameObject.tag = "wall";
            }
        }
        
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }
}
