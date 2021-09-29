using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ExitManager : MonoBehaviour
{
    public bool agentVersion;
    public CloneManager cloneManager;    

    private ShoppingList clientList;
    private ShoppingAgent clientAgent;

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log(collision.collider.tag + " collided with exit");
        /*if (collision.collider.CompareTag("agent"))
        {
            if (collision.collider.GetComponentInParent<LocationCloner>() != null)
            {
                collision.collider.gameObject.SetActive(false);
            }

            
            clientAgent = collision.collider.GetComponentInParent<ShoppingAgent>();
            if (clientAgent != null && clientAgent.list.checkedOut)
            {
                clientAgent.ExitStore();
                Debug.Log("Exit success. Saving data...");
#if UNITY_EDITOR
                EditorApplication.ExecuteMenuItem("Edit/Play");
#else
                Application.Quit();
#endif
            }
        }
        */
        
        if (collision.collider.CompareTag("Player"))
        {
            clientList = collision.collider.GetComponentInChildren<ShoppingList>();
            if (clientList == null)
            {
                clientList = collision.collider.GetComponentInParent<ShoppingAgent>().GetComponentInChildren<ShoppingList>();
            }
            Debug.Log("client list: " + clientList);
            if (clientList.checkedOut)
            {
                clientList.agent.ExitStore();
                
                /*
                
#if UNITY_EDITOR
                EditorApplication.ExecuteMenuItem("Edit/Play");
#else
                Application.Quit();
#endif
                //client.dataExporter.SaveData();
            */
            }
        } else if (collision.collider.CompareTag("basket"))
        {
            clientList = collision.collider.GetComponentInParent<ShoppingList>();
            if (clientList.checkedOut)
            {
                clientList.agent.ExitStore();
            }
        }
    }

    public void SetGoalTag(bool goal)
    {
        if (goal)
        {
            this.tag = "goal";
            foreach (Collider col in GetComponentsInChildren<BoxCollider>())
            {
                col.gameObject.tag = "goal";
            }
        }
        else
        {
            this.tag = "wall";
            foreach (Collider col in GetComponentsInChildren<BoxCollider>())
            {
                col.gameObject.tag = "wall";
            }
        }
    }
}
