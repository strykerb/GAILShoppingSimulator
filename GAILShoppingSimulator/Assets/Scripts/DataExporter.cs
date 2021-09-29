using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.MLAgents.Sensors;
using UnityEngine;
using UnityEngine.XR;
using System.IO;
using UnityEngine.XR.Interaction.Toolkit;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DataExporter : MonoBehaviour
{
    public Transform head;
    [SerializeField] ShoppingList listRef;
    [SerializeField] DeviceBasedContinuousMoveProvider movement;
    public RayPerceptionSensorComponent3D[] sensors;
    public RayPerceptionSensor raySensor1;
    RayPerceptionInput input1;
    RayPerceptionInput input2;
    RayPerceptionInput input3;
    private List<KeyFrame> keyFrames = new List<KeyFrame>(20000);
    private string output;
    private bool writing = false;
    private int counter;
    private List<InputDevice> devices;
    int maxCountBeforeWriting = 19900;

    List<ShoppingAgent> agents;
    public float maxTime;
    private float currTimeLimit;
    public bool recording;


    // Start is called before the first frame update
    void Start()
    {
        currTimeLimit = maxTime;
    }

    public void SaveShoppingData(float time, int collisions)
    {
        keyFrames.Add(new KeyFrame(time, collisions));
        //Debug.Log("saving keyframe.");
    }

    // Update is called once per frame
    void Update()
    {
        if (!recording)
        {
            return;
        }
        if (Time.realtimeSinceStartup > currTimeLimit)
        {
            SaveData();
            currTimeLimit += maxTime;
        }
        
        /*
        counter++;
        // Collect observations every 5 frames
        if (counter >= 4)
        {
            counter = 0;
            // Don't record new data while writing to CSV
            if (writing) return;
            if (!recording) return;
            keyFrames.Add(new KeyFrame(head.position, head.rotation, RayPerceptionSensor.Perceive(input1).RayOutputs, RayPerceptionSensor.Perceive(input2).RayOutputs, RayPerceptionSensor.Perceive(input3).RayOutputs, listRef.shoppingListString));
            if (keyFrames.Count > maxCountBeforeWriting)
            {
                Debug.Log("Running out of room. Writing data.");
                SaveData();
                writing = true;
                //keyFrames.Clear();
            }
        }
        */
    }

    void CheckForInput()
    {
        
    }

    IEnumerator ToCSV()
    {
        var sb = new StringBuilder("Shopping Time:, Collision Count:");
        foreach (var frame in keyFrames)
        {
            sb.Append('\n').Append(frame.shoppingTime.ToString("G8")).Append(',').Append(frame.collisionCount.ToString("G8"));
            
        }
        output = sb.ToString();

        StartCoroutine("SaveToFile");

        yield return true;
    }

    public void StartRecording()
    {
        recording = true;
        movement.enabled = true;
    }

    public void SaveData()
    {
        StartCoroutine("ToCSV");
    }

    IEnumerator SaveToFile()
    {
        // Use the CSV generation from before
        ToCSV();
        string fname = gameObject.name + "_"+ System.DateTime.Now.ToString("HH-mm-ss") + ".csv";
        // The target file path e.g.
#if UNITY_EDITOR
        var folder = Application.streamingAssetsPath;

        if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
#else
        
        var folder = Application.persistentDataPath;
#endif

        var filePath = Path.Combine(folder, fname);

        using (var writer = new StreamWriter(filePath, false))
        {
            writer.Write(output);
        }

        // Or just
        //File.WriteAllText(content);

        Debug.Log($"CSV file written to \"{filePath}\"");

#if UNITY_EDITOR
        AssetDatabase.Refresh();
#endif
        //writing = false;
        keyFrames.Clear();
        //Debug.Log("Quitting application");
        //Application.Quit();
        yield return true;
    }

}

public class KeyFrame
{

    public float shoppingTime;
    public int collisionCount;

    public KeyFrame() { }

    public KeyFrame(float time, int collisions)
    {
        shoppingTime = time;
        collisionCount = collisions;
    }
}

