using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;

public class ShelfManager : MonoBehaviour
{
    [SerializeField] float[] spawnXValues;
    [SerializeField] float[] spawnYValues;
    [SerializeField] float spawnZValue;
    private string color;
    private Material mat;

    // Array of items on shelf. Index 0 corresponds to top left shelf.
    // Index 1 corresponds to top right shelf, and so on.
    public GameObject[] itemPrefabs;


    // Start is called before the first frame update
    void Start()
    {
        Shuffle(itemPrefabs);
    }

    public void SetColor(string colorParam)
    {
        color = colorParam;
        //StringBuilder sb = new StringBuilder();
        //sb.Append("Assets/Materials/");
        //sb.Append(color);
        //sb.Append(".mat");
        mat = Resources.Load<Material>(color);
        spawnItems();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void spawnItems()
    {
        for (int i = 0; i < 8; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                double temp = i / 4;
                // index magic to change the prefab based on the spawn location
                GameObject item = Instantiate(itemPrefabs[(int) Math.Floor(temp) + j*2], transform);
                item.transform.localPosition = new Vector3(spawnXValues[i], spawnYValues[j], spawnZValue);

                // Color the item
                item.GetComponentInChildren<MeshRenderer>().material = mat;

                // Generate tag based on object color and shape
                item.tag = color + itemPrefabs[(int)Math.Floor(temp) + j*2].name;
                item.GetComponentInChildren<ShoppingItem>().gameObject.tag = item.tag;
            }
        }
    }

    void Shuffle<T>(T[] array)
    {
        int n = array.Length;
        while (n > 1)
        {

            int k = Random.Range(0, n--);
            T temp = array[n];
            array[n] = array[k];
            array[k] = temp;
        }
    }
}
