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
    List<Vector3> spawnPoints;
    List<Quaternion> spawnRotations;
    string[] colors = { "black", "blue", "green", "orange", "pink", "purple", "red", "teal", "white", "yellow" };

    void Start()
    {
        spawnPoints = new List<Vector3>();
        spawnRotations = new List<Quaternion>();
        GenerateEnvironment();
        SpawnShelves();
    }

    void GenerateEnvironment()
    {
        // Create Side Wall 1
        // Rotation is 0 deg about y-axis
        for (int i = 0; i < 6; i++)
        {
            spawnPoints.Add(new Vector3(wallSpawnXValues[i], aisleHeight, wallSpawnYValue));
            spawnRotations.Add(Quaternion.identity);
        }

        // Create Back Wall
        // Rotation is 90 deg about y-axis
        for (int i = 0; i < 6; i++)
        {
            spawnPoints.Add(new Vector3(wallSpawnYValue, aisleHeight, wallSpawnXValues[i]));
            spawnRotations.Add(Quaternion.Euler(new Vector3(0, 90, 0)));
        }

        // Create Side Wall 2
        // Rotation is 180 deg about y-axis
        for (int i = 0; i < 6; i++)
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
                spawnPoints.Add(new Vector3(aisleSpawnXValues[i], aisleHeight, aisleSpawnYValues[j] - .01f));
                spawnRotations.Add(Quaternion.identity);
                spawnPoints.Add(new Vector3(aisleSpawnXValues[i], aisleHeight, aisleSpawnYValues[j] + .01f));
                spawnRotations.Add(Quaternion.Euler(new Vector3(0, 180, 0)));
            }
        }
    }

    void SpawnShelves()
    {
        // Randomly spawn the colored Items
        for (int i = 0; i < colors.Length; i++)
        {
            int idx = Random.Range(0, spawnPoints.Count);
            GameObject thisShelf = Instantiate(shelfPrefabs[0], spawnPoints[idx], spawnRotations[idx]);
            thisShelf.GetComponent<ShelfManager>().SetColor(colors[i]);
            spawnPoints.RemoveAt(idx);
            spawnRotations.RemoveAt(idx);
        }
        
        // Fill the rest of the store with Empty Shelves
        for (int i = 0; i < spawnPoints.Count; i++)
        {
            Instantiate(shelfPrefabs[shelfPrefabs.Length - 1], spawnPoints[i], spawnRotations[i]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
