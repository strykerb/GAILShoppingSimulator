using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.MLAgents.Sensors;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class DataExporter : MonoBehaviour
{
    public Transform head;
    [SerializeField] ShoppingList listRef;
    public RayPerceptionSensorComponent3D[] sensors;
    public RayPerceptionSensor raySensor1;
    RayPerceptionInput input1;
    RayPerceptionInput input2;
    private List<KeyFrame> keyFrames = new List<KeyFrame>(20000);
    private string output;
    private bool writing = false;
    private int counter;
    int maxCountBeforeWriting = 19900;

    // Start is called before the first frame update
    void Start()
    {
        sensors = GetComponentsInChildren<RayPerceptionSensorComponent3D>();
        //sensors[0].CreateSensors();
        //sensors[1].CreateSensors();

        input1 = sensors[0].GetRayPerceptionInput();
        input2 = sensors[1].GetRayPerceptionInput();
        input1.DetectableTags = GetDetectableTags();
        input2.DetectableTags = input1.DetectableTags;
    }

    // Update is called once per frame
    void Update()
    {
        counter++;
        // Collect observations every 5 frames
        if (counter >= 4)
        {
            counter = 0;
            // Don't record new data while writing to CSV
            if (writing) return;
            keyFrames.Add(new KeyFrame(head.position, head.rotation, RayPerceptionSensor.Perceive(input1).RayOutputs, RayPerceptionSensor.Perceive(input2).RayOutputs, listRef.shoppingListString));
            if (keyFrames.Count > maxCountBeforeWriting)
            {
                Debug.Log("Running out of room. Writing data.");
                SaveData();
                writing = true;
                //keyFrames.Clear();
            }
        }
    }

    IEnumerator ToCSV()
    {
        var sb = new StringBuilder("pX, pY, pZ, rX, rY, rZ, rW, RayPerception1 output Array followed by RayPerception2 output Array followed by shopping list.");
        foreach (var frame in keyFrames)
        {
            sb.Append('\n').Append(frame.pos.x.ToString("G8")).Append(',').Append(frame.pos.y.ToString("G8")).Append(',').Append(frame.pos.z.ToString("G8")).Append(',');
            sb.Append(frame.rot.x.ToString("G8")).Append(',').Append(frame.rot.y.ToString("G8")).Append(',').Append(frame.rot.z.ToString("G8")).Append(',').Append(frame.rot.w.ToString("G8")).Append(',');
            foreach (float val in frame.ray1Output)
            {
                sb.Append(val).Append(",");
            }
            foreach (float val in frame.ray2Output)
            {
                sb.Append(val).Append(",");
            }
            sb.Append(frame.list);
        }

        output = sb.ToString();

        StartCoroutine("SaveToFile");

        yield return true;
    }

    public void SaveData()
    {
        StartCoroutine("ToCSV");
    }

    IEnumerator SaveToFile()
    {
        // Use the CSV generation from before
        ToCSV();
        string fname = System.DateTime.Now.ToString("HH-mm-ss") + ".csv";
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
        writing = false;
        keyFrames.Clear();
        yield return true;
    }

    List<string> GetDetectableTags()
    {
        List<string> assembledList = new List<string>() { "agent", "wall", "ground", "shelf", "checkout", "basketarea" };
        for (int i = 0; i < listRef.colors.Length; i++)
        {
            for (int j = 0; j < listRef.shapes.Length; j++)
            {
                assembledList.Add(listRef.colors[i] + listRef.shapes[j]);
            }
        }
        return assembledList;
    }
}

public class KeyFrame
{
    public Vector3 pos;
    public Quaternion rot;
    public string list;
    public float[] ray1Output;
    public float[] ray2Output;
    int tagCount = 66;

    public KeyFrame() { }

    public KeyFrame(Vector3 newPos, Quaternion newRot, RayPerceptionOutput.RayOutput[] ray1Outputs, RayPerceptionOutput.RayOutput[] ray2Outputs, string shoppingList)
    {
        pos = newPos;
        rot = newRot;
        ray1Output = new float[(tagCount + 2) * ray1Outputs.Length];
        ray2Output = new float[(tagCount + 2) * ray2Outputs.Length];
        for (int i = 0; i < ray1Outputs.Length; i++)
        {
            ray1Outputs[i].ToFloatArray(tagCount, i, ray1Output);
            ray2Outputs[i].ToFloatArray(tagCount, i, ray2Output);
        }
        list = shoppingList;
    }
}

