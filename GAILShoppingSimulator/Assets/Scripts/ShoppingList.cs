using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class ShoppingList : MonoBehaviour
{
    [SerializeField] public ShoppingAgent agent;
    [SerializeField] public List<string> shoppingList;
    [SerializeField] List<Text> texts;
    [SerializeField] int minListItems;
    [SerializeField] int maxListItems;
    public GameObject basketModel;
    public LayoutManager layoutManager;
    public string[] shapes = { "ball", "capsule", "cross", "cube", "cylinder", "triangle" };
    public string[] colors = { "black", "blue", "green", "purple", "red", };
    public bool shoppingComplete = false;
    public bool checkedOut = false;
    public string shoppingListString;
    public Dictionary<string, int> itemDict;

    // Start is called before the first frame update
    void Start()
    {
        //layoutManager = FindObjectOfType<LayoutManager>();
        shoppingList = new List<string>();
        InitializeList();
    }

    public void InitializeList()
    {
        shoppingComplete = false;
        checkedOut = false;
        shoppingList.Clear();
        //texts = new List<Text>();
        int length = Random.Range(minListItems, maxListItems);
        for (int i = 0; i < length; i++)
        {
            // combine a random color and shape
            int colorIdx = Random.Range(0, colors.Length);
            string thisItem = colors[colorIdx];
            // Try again if the item is a duplicate
            if (shoppingList.Contains(thisItem))
            {
                i--;
                continue;
            } 
            else
            {
                shoppingList.Add(thisItem);
            }
        }

        int idx = 0;
        foreach (string item in shoppingList)
        {
            texts[idx].text = "- " + item + " " + shapes[Random.Range(0, shapes.Length)];
            idx++;
        }

        buildShoppingListString();

        itemDict = BuildItemDict();
    }

    public void placeItem(ShoppingItem item)
    {
        // If the collected item is on the list, destoy it and update the list
        foreach (Text txt in texts)
        {
            if (txt.text.Contains(item.selfRef.name))
            {
                txt.text = "";
            }
        }

        if (shoppingList.Contains(item.selfRef.name)) {
            
            //texts[shoppingList.IndexOf(item.selfRef.tag)].text = "";
            //texts.RemoveAt(shoppingList.IndexOf(item.selfRef.tag));
            shoppingList.Remove(item.selfRef.name);
            item.selfRef.SetActive(false);
            buildShoppingListString();

            // If this was the last item, player can now check out
            if (shoppingList.Count == 0)
            {
                shoppingComplete = true;
                //Debug.Log("done.");
            }
            agent.CollectItem();
        }
    }

    void buildShoppingListString()
    {
        var sb = new StringBuilder();
        foreach (string s in shoppingList) { sb.Append(s).Append(","); }
        shoppingListString = sb.ToString();
    }

    Dictionary<string, int> BuildItemDict()
    {
        Dictionary<string, int> dict = new Dictionary<string, int>();
        dict["agent"] = 0;
        dict["shelf"] = 1;
        //dict["basketarea"] = 2;
        //dict["exit"] = 3;
        dict["goal"] = 4;
        /*
        for (int i = 0; i < colors.Length; i++)
        {
            dict[colors[i]] = 4 + i;
        }
        */
        return dict;
    }


    // Update is called once per frame
    void Update()
    {
        
    }

}
