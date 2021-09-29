using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayoutManager : MonoBehaviour
{
    // Start is called before the first frame update

    [SerializeField] float aisleHeight;
    [SerializeField] float wallSpawnYValue;
    [SerializeField] float[] wallSpawnXValues;
    [SerializeField] float[] aisleSpawnXValues;
    [SerializeField] float[] aisleSpawnYValues;
    [SerializeField] GameObject[] shelfPrefabs;
    public List<ShoppingList> shoppers;
    List<Vector3> AisleEndSpawns;
    List<Vector3> spawnPoints;
    List<Quaternion> spawnRotations;
    bool hasAisleEnds = false;
    string[] colors = { "black", "blue", "green",  "purple", "red"};
    public int callCount = 0;

    void Start()
    {
        spawnPoints = new List<Vector3>();
        AisleEndSpawns = new List<Vector3>();
        spawnRotations = new List<Quaternion>();
        
        //GenerateEnvironment();
    }

    public void GenerateEnvironment()
    {
        
        spawnPoints.Clear();
        spawnRotations.Clear();

        // Remove existing shelves
        foreach (EmptyShelf shelf in GetComponentsInChildren<EmptyShelf>())
        {
            foreach (Transform child in shelf.transform)
            {
                Destroy(child.gameObject);
            }
            Destroy(shelf.gameObject);
        }
        foreach (ShelfManager shelf in GetComponentsInChildren<ShelfManager>())
        {
            foreach (Transform child in shelf.transform)
            {
                Destroy(child.gameObject);
            }
            Destroy(shelf.gameObject);
        }


        // Create Side Wall 1
        // Rotation is 0 deg about y-axis
        for (int i = 0; i < wallSpawnXValues.Length; i++)
        {
            spawnPoints.Add(new Vector3(wallSpawnXValues[i], aisleHeight, wallSpawnYValue));
            spawnRotations.Add(Quaternion.identity);
        }

        // Create Back Wall
        // Rotation is 90 deg about y-axis
        for (int i = 0; i < wallSpawnXValues.Length; i++)
        {
            spawnPoints.Add(new Vector3(wallSpawnYValue, aisleHeight, wallSpawnXValues[i]));
            spawnRotations.Add(Quaternion.Euler(new Vector3(0, 90, 0)));
        }

        // Create Side Wall 2
        // Rotation is 180 deg about y-axis
        for (int i = 0; i < wallSpawnXValues.Length; i++)
        {
            spawnPoints.Add(new Vector3(wallSpawnXValues[i], aisleHeight, -wallSpawnYValue));
            spawnRotations.Add(Quaternion.Euler(new Vector3(0, 180, 0)));
        }

        // Create the Aisles
        // Rotation is either 0 or 180 deg about y-axis
        for (int i = 0; i < aisleSpawnXValues.Length; i++)
        {
            for (int j = 0; j < aisleSpawnYValues.Length; j++)
            {
                spawnPoints.Add(new Vector3(aisleSpawnXValues[i], aisleHeight, aisleSpawnYValues[j] - .15f));
                spawnRotations.Add(Quaternion.identity);
                spawnPoints.Add(new Vector3(aisleSpawnXValues[i], aisleHeight, aisleSpawnYValues[j] + .15f));
                spawnRotations.Add(Quaternion.Euler(new Vector3(0, 180, 0)));
            }
        }

        // Aisle End Poitions
        for (int i = 0; i < aisleSpawnYValues.Length; i++)
        {
            AisleEndSpawns.Add(new Vector3(aisleSpawnXValues[0] + 2.4f, 0, aisleSpawnYValues[i]));
            AisleEndSpawns.Add(new Vector3(aisleSpawnXValues[aisleSpawnXValues.Length-1] - 2.4f, 0, aisleSpawnYValues[i]));
        }

        SpawnShelves();
    }

    void SpawnShelves()
    {
        // Randomly Spawn Each Shopper's Required Items
        foreach (ShoppingList shopper in shoppers)
        {
            foreach (string color in shopper.shoppingList)
            {
                int idx = Random.Range(0, spawnPoints.Count);
                shopper.agent.shelfDict[color] = Instantiate(shelfPrefabs[0], transform);
                shopper.agent.shelfDict[color].transform.localPosition = spawnPoints[idx];
                shopper.agent.shelfDict[color].transform.rotation = spawnRotations[idx];
                shopper.agent.shelfDict[color].GetComponent<ShelfManager>().SetColor(color);
                shopper.agent.shelfDict[color].GetComponent<ShelfManager>().SetShelfTag(shopper.agent.goalTag);
                spawnPoints.RemoveAt(idx);
                spawnRotations.RemoveAt(idx);
            }
            //shopper.agent.UpdateWorldTags();
        }
        
        /*
        // Randomly spawn the colored Items
        for (int i = 0; i < colors.Length; i++)
        {
            int idx = Random.Range(0, spawnPoints.Count);
            shelfDict[colors[i]] = Instantiate(shelfPrefabs[0], transform);
            shelfDict[colors[i]].transform.localPosition = spawnPoints[idx];
            shelfDict[colors[i]].transform.rotation = spawnRotations[idx];
            shelfDict[colors[i]].GetComponent<ShelfManager>().SetColor(colors[i]);
            spawnPoints.RemoveAt(idx);
            spawnRotations.RemoveAt(idx);
        }
        */

        // Fill the rest of the store with Empty Shelves
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            GameObject thisShelf = Instantiate(shelfPrefabs[1], transform);
            thisShelf.transform.localPosition = spawnPoints[i];
            thisShelf.transform.rotation = spawnRotations[i];
        }

        if (!hasAisleEnds)
        {
            foreach (Vector3 spawn in AisleEndSpawns)
            {
                GameObject aisleEnd = Instantiate(shelfPrefabs[2], transform);
                aisleEnd.transform.localPosition = spawn;
            }
        }
    }


    public void AddShopper(ShoppingList shopper)
    {
        shoppers.Add(shopper);
    }
    public void RemoveShopper(ShoppingList shopper)
    {
        shoppers.Remove(shopper);
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
