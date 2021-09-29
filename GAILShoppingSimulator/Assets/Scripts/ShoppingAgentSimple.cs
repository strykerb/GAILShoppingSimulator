using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;

public class ShoppingAgentSimple : Agent
{
    private ShopperSettings m_ShopperSettings;
    private Rigidbody m_AgentRb;
    //private DungeonEscapeEnvController m_GameController;
    
    [SerializeField] private Transform targetTransform;
    /*
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;
    [SerializeField] private MeshRenderer floorMeshRenderer;
    */
    [SerializeField] private float[] spawnXValues;
    [SerializeField] private float speed;
    [SerializeField] private string goalTag;
    private RayPerceptionSensorComponent3D[] sensors;

    public bool useVectorObs;

    public override void Initialize()
    {
        m_AgentRb = GetComponent<Rigidbody>();
        sensors = GetComponents<RayPerceptionSensorComponent3D>();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (useVectorObs)
        {
            sensor.AddObservation(transform.InverseTransformDirection(m_AgentRb.velocity));
        }
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        var dirToGo = Vector3.zero;
        var rotateDir = Vector3.zero;

        var action = act[0];
        switch (action)
        {
            case 1:
                dirToGo = transform.forward * 1f;
                break;
            case 2:
                dirToGo = transform.forward * -1f;
                break;
            case 3:
                rotateDir = transform.up * 1f;
                break;
            case 4:
                rotateDir = transform.up * -1f;
                break;
        }
        transform.Rotate(rotateDir, Time.deltaTime * 200f);
        m_AgentRb.AddForce(dirToGo * speed, ForceMode.VelocityChange);
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)

    {
        AddReward(-1f / MaxStep);
        MoveAgent(actionBuffers.DiscreteActions);
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.D))
        {
            discreteActionsOut[0] = 3;
        }
        else if (Input.GetKey(KeyCode.W))
        {
            discreteActionsOut[0] = 1;
        }
        else if (Input.GetKey(KeyCode.A))
        {
            discreteActionsOut[0] = 4;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            discreteActionsOut[0] = 2;
        }
    }

    public override void OnEpisodeBegin()
    {
        float targetX = spawnXValues[Random.Range(0, spawnXValues.Length)];
        targetTransform.localPosition = new Vector3(targetX, 0, Random.Range(-3.5f, 3.5f));

        m_AgentRb.velocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("agent"))
        {
            SetReward(-3f);
            EndEpisode();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        //if we find a key and it's parent is the main platform we can pick it up
        if (col.gameObject.CompareTag(goalTag))
        {
            SetReward(3f);
            NextItem();
        } 
    }

    void NextItem()
    {
        float targetX = spawnXValues[Random.Range(0, spawnXValues.Length)];
        targetTransform.localPosition = new Vector3(targetX, 0, Random.Range(-3.5f, 3.5f));

        m_AgentRb.velocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));

        /*
        // Change detectable Tag Test
        foreach (RayPerceptionSensorComponent3D sensor in sensors)
        {
            sensor.DetectableTags = new List<string> { "wall", "null", "Aisle", "agent" };
        }
        */
    }
}
