using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloneManager : MonoBehaviour
{
    public List<ShoppingAgent> agents;
    public List<LayoutManager> stores;
    public List<CheckStand> checkstands;
    public GameObject clonePrefab;
    public int activeAgents;
    int idx;
    public float spawnDelay;

    public void AddAgent(ShoppingAgent newAgent)
    {
        agents.Add(newAgent);
        
    }
    
    // Start is called before the first frame update
    void Start()
    {
        activeAgents = agents.Count;
        if (agents[0].Stage1){spawnDelay = 0f;}
        
        foreach (ShoppingAgent newAgent in agents)
        {
            newAgent.gameObject.SetActive(false);
            Invoke("SpawnAgent", spawnDelay * idx);
            idx++;
        }
    }

    void SpawnAgent()
    {
        ShoppingAgent currAgent = null;
        for (int i = 0; i < activeAgents; i++)
        {
            if (!agents[i].isActiveAndEnabled)
            {
                currAgent = agents[i];
                break;
            }
        }
        if (currAgent == null)
        {
            return;
        }

        currAgent.gameObject.SetActive(true);

        //Don't spawn clones in first stage of training
        if (agents[0].Stage1) { return; }
        foreach (LayoutManager store in stores)
        {
            if (store != currAgent.list.layoutManager)
            {
                //Debug.Log("Spawning new clone in " + store.gameObject.name);
                GameObject clone = Instantiate(clonePrefab, store.transform);
                clone.GetComponent<LocationCloner>().SetCloneOrigin(currAgent);
            }
        }
    }

    public void SetCheckStandBusy(bool isBusy)
    {
        if (agents[0].Stage1) { return; }
        foreach (CheckStand check in checkstands)
        {
            check.SetBusy(isBusy);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
