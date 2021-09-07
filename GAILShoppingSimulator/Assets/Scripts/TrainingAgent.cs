using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;


public class TrainingAgent : Agent
{
    private ShopperSettings m_ShopperSettings;

    public override void Initialize()
    {
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        //Debug.Log(sensor);
        
    }

    public void MoveAgent(ActionSegment<int> act)
    {
        
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)

    {
        
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        
    }

    public override void OnEpisodeBegin()
    {
        
    }
}
