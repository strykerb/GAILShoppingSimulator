using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingList : MonoBehaviour
{
    [SerializeField] public List<string> shoppingList;
    [SerializeField] List<Text> texts;
    [SerializeField] int minListItems;
    [SerializeField] int maxListItems;
    public DataExporter dataExporter;
    public string[] shapes = {"ball", "capsule", "cross", "cube", "cylinder", "triangle"};
    public string[] colors = {"black", "blue", "green", "orange", "pink", "purple", "red", "teal", "white", "yellow"};
    public bool shoppingComplete;
    public string shoppingListString;

    // Start is called before the first frame update
    void Start()
    {
        shoppingList = new List<string>();
        //texts = new List<Text>();
        int length = Random.Range(minListItems, maxListItems);
        for (int i = 0; i < length; i++)
        {
            // combine a random color and shape
            int colorIdx = Random.Range(0, colors.Length);
            int shapeIdx = Random.Range(0, shapes.Length);
            string thisItem = colors[colorIdx] + shapes[shapeIdx];
            // Try again if the item is a duplicate
            if (shoppingList.Contains(thisItem))
            {
                i--;
                continue;
            }
            shoppingList.Add(thisItem);
            texts[i].text = "- " + colors[colorIdx].Substring(0,1).ToUpper() + colors[colorIdx].Substring(1) + " " + shapes[shapeIdx].Substring(0, 1).ToUpper() + shapes[shapeIdx].Substring(1);
        }
        buildShoppingListString();
    }

    public void placeItem(ShoppingItem item)
    {
        // If the collected item is on the list, destoy it and update the list
        if (shoppingList.Contains(item.selfRef.tag)){
            texts[shoppingList.IndexOf(item.selfRef.tag)].text = "";
            texts.RemoveAt(shoppingList.IndexOf(item.selfRef.tag));
            shoppingList.Remove(item.selfRef.tag);
            item.selfRef.SetActive(false);
            buildShoppingListString();
        }
        
        // If this was the last item, player can now check out
        if (shoppingList.Count == 0)
        {
            shoppingComplete = true;
            Debug.Log("done.");
        }
    }

    void buildShoppingListString()
    {
        var sb = new StringBuilder();
        foreach (string s in shoppingList) { sb.Append(s).Append(","); }
        shoppingListString = sb.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
