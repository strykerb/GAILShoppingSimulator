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
    List<GameObject> items;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    public void SetColor(string colorParam)
    {
        items = new List<GameObject>();
        Shuffle(itemPrefabs);
        color = colorParam;
        //StringBuilder sb = new StringBuilder();
        //sb.Append("Assets/Materials/");
        //sb.Append(color);
        //sb.Append(".mat");
        mat = Resources.Load<Material>(color);
        
        foreach (BoxCollider shelf in GetComponentsInChildren<BoxCollider>())
        {
            shelf.gameObject.tag = "goal";
        }
        this.tag = "goal";
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
                items.Add(Instantiate(itemPrefabs[(int) Math.Floor(temp) + j*2], transform));
                items[items.Count-1].transform.localPosition = new Vector3(spawnXValues[i], spawnYValues[j], spawnZValue);
                items[items.Count - 1].tag = "goal";
                items[items.Count - 1].name = color;
                // Color the item
                items[items.Count - 1].GetComponentInChildren<MeshRenderer>().material = mat;

                // Generate tag based on object color and shape
                // item.tag = color + itemPrefabs[(int)Math.Floor(temp) + j*2].name;
                items[items.Count - 1].GetComponentInChildren<ShoppingItem>().gameObject.tag = "goal";
                items[items.Count - 1].GetComponentInChildren<ShoppingItem>().gameObject.name = color;
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

    public void SetShelfTag(string newTag)
    {
        Collider[] children = GetComponentsInChildren<Collider>();
        foreach (Collider col in children)
        {
            col.gameObject.tag = newTag;
        }
        foreach (GameObject item in items)
        {
            item.tag = newTag;
        }
        this.tag = newTag;
    }

    public void RemoveGoalTag()
    {
        Collider[] children = GetComponentsInChildren<Collider>();
        foreach (Collider col in children)
        {
            col.gameObject.tag = "shelf";
        }
        foreach (GameObject item in items)
        {
            item.tag = "shelf";
        }
        this.tag = "shelf";
    }
}
