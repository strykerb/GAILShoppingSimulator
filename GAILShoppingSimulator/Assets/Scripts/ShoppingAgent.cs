using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using System.Collections.Generic;
using UnityEngine.XR;
using Unity.MLAgents.Policies;
using Unity.MLAgents.Demonstrations;
using UnityEngine.XR.Interaction.Toolkit;
using System.Collections;
using System.Linq;

public class ShoppingAgent : Agent
{
    [SerializeField] private Transform m_AgentHead;
    [SerializeField] private float speed;
    [SerializeField] DeviceBasedContinuousMoveProvider movement;
    public string goalTag;
    public ShoppingList list;
    public CheckStand checkOut;
    public ExitManager exit;
    public bool Stage1;
    public float[] spawnPoint;
    public bool isRespawning;
    
    private ShopperSettings m_ShopperSettings;
    private Rigidbody m_AgentRb;
    private RayPerceptionSensorComponent3D raySensor;
    List<Collider> collisions;
    BehaviorParameters behavParams;
    List<InputDevice> hmd;
    List<InputDevice> controllers;
    DemonstrationRecorder demo;
    public Dictionary<string, GameObject> shelfDict;
    public List<LocationCloner> clones;

    public Vector3 rotationalVelocity;
    private Vector3 prevPos;
    private Vector3 prevRot;
    private Vector3 moveDirection;
    private float startTime;
    private float respawnTime = 5f;
    private float collectReward = 1f;
    private float checkoutReward = 5f;
    private float exitReward = 10f;
    private float exploreReward = 0.001f;
    private float goalInSightReward = 0.002f;
    private float collisionPunishment = -1f;
    private float walkIntoWallPunishment = -0.005f;
    private float rotateSpeed = 200f;
    float totalGainedfromSight = 0f;
    float rotateDir;
    float rotateAmount;
    float magnitude = 0f;
    private int counter;
    private int numCollisions;
    int maxCount = 100; 
    bool placedItem = false;
    bool startCheckout = false;
    public bool atCheckStand = false;
    bool canMove = false;
    bool recordingPerformanceData = true;


    private void Start()
    {
        hmd = new List<InputDevice>();
        controllers = new List<InputDevice>();
        if (behavParams.IsInHeuristicMode())
        {
            InputDeviceCharacteristics HMDChars = InputDeviceCharacteristics.HeadMounted;
            InputDevices.GetDevicesWithCharacteristics(HMDChars, hmd);
            InputDeviceCharacteristics ControllerChars = InputDeviceCharacteristics.Controller;
            InputDevices.GetDevicesWithCharacteristics(ControllerChars, controllers);
            if (movement != null)
            {
                movement.enabled = false;
            }
            demo.Record = false;
        }
        //rotateAmount = 1.0f;
    }

    public override void Initialize()
    {
        behavParams = GetComponent<BehaviorParameters>();
        m_AgentRb = GetComponent<Rigidbody>();
        demo = GetComponent<DemonstrationRecorder>();

        clones = new List<LocationCloner>();
        raySensor = GetComponentInChildren<RayPerceptionSensorComponent3D>();
        
        collisions = new List<Collider>();
        rotationalVelocity = Vector3.zero;
        prevPos = transform.position;
        prevRot = transform.rotation.eulerAngles;

        //checkOut = FindObjectOfType<CheckStand>();
        //m_GoalSensor = GetComponent<VectorSensorComponent>();
    }

    public override void OnEpisodeBegin()
    {
        list.InitializeList();
        shelfDict = new Dictionary<string, GameObject>();
        //shelfDict.Clear();
        //SetGoalTag(goalTag);
        list.layoutManager.GenerateEnvironment();
        //UpdateWorldTags();
        collisions.Clear();
        //float targetX = spawnXValues[Random.Range(0, spawnXValues.Length)];
        transform.localPosition = new Vector3(spawnPoint[0], 0, spawnPoint[1]);
        //transform.localPosition = new Vector3(-8f, 0, 6.75f);
        prevPos = transform.localPosition;
        counter = 0;
        isRespawning = false;
        m_AgentRb.velocity = Vector3.zero;
        transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
        startTime = Time.unscaledTime;
        numCollisions = 0;
        UpdateWorldTags();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        if (m_AgentRb)
        {
            sensor.AddObservation(transform.InverseTransformDirection(m_AgentRb.velocity));
        }
        else
        {
            sensor.AddObservation(Vector3.zero);
        }
        
    }

    public void MoveAgent(ActionSegment<float> act)
    {
        // Listen to XR Input if in Hueristic mode
        if (behavParams.IsInHeuristicMode())
        {
            return;
        }
        rotateAmount = act[0];
        //Debug.Log(act[0]);
        //Vector3 rotateDir = transform.up * rotateAmount;
        //transform.Rotate(rotateDir, Time.deltaTime * rotateSpeed);
        Vector3 rotateDir = transform.up;
        transform.Rotate(rotateDir, Time.deltaTime * rotateSpeed * rotateAmount);
    }

    public void TakeActions(ActionSegment<int> act)
    {
        Vector3 forwardVec;

        // Agent moving forwards or standing still
        if (act[0] == 1)
        {
            magnitude = 1;
        } /*else if (act[0] == 2)
        {
            magnitude = -1;
        } */else if (act[0] == 0)
        {
            magnitude = 0f;
        }

        if (behavParams.IsInHeuristicMode())
        {
            forwardVec = new Vector3(m_AgentHead.forward.x, 0, m_AgentHead.forward.z);
        } else
        {
            forwardVec = new Vector3(transform.forward.x, 0, transform.forward.z);
        }

        if (!behavParams.IsInHeuristicMode() || (behavParams.IsInHeuristicMode() && canMove))
        {
            m_AgentRb.AddForce(forwardVec * speed * magnitude, ForceMode.VelocityChange);
        }

        // Don't handle the rest if in Hueristic Mode
        if (behavParams.IsInHeuristicMode())
        {
            return;
        }

        
        // Agent interaction with objectives
        if (act[0] == 2)
        {
            // Checkout
            if (atCheckStand && list.shoppingComplete && !checkOut.busy && !list.checkedOut)
            {
                checkOut.client = list;
                checkOut.StartCheckout();
                startCheckout = false;
                SetReward(checkoutReward);
                Debug.Log("Agent rewarded for checkout.");
                StartCoroutine(StopAgentForSeconds(10f));
                //EndEpisode();
            }

            // Collect item from shelf
            else if (collisions.Count > 0)
            {
                ShoppingItem item = collisions[collisions.Count - 1].gameObject.GetComponent<ShoppingItem>();
                if (list.shoppingList.Contains(item.name))
                {
                    list.placeItem(item);
                    SetReward(collectReward);
                    Debug.Log("Agent rewarded for item collection.");
                    //CollectItem();
                    placedItem = false;
                    if (Stage1)
                    {
                        EndEpisode();
                    }
                    StartCoroutine(StopAgentForSeconds(2f));
                    //EndEpisode();
                }
            }
        }
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //AddReward(-1f / MaxStep);
        RaycastHit hit;
        Vector3 lookDir;
        Vector3 originPoint = new Vector3(transform.position.x, 1.118f, transform.position.z + 0.25f);
        if (behavParams.IsInHeuristicMode())
        {
            lookDir = m_AgentHead.forward;
        } else
        {
            lookDir = transform.forward;
        }
        // Does the ray intersect any objects excluding the player layer
        if (Physics.Raycast(originPoint, lookDir, out hit, Mathf.Infinity))
        {
            //Debug.DrawRay(transform.position, lookDir * hit.distance, Color.yellow);
            //Debug.Log("Hit " + hit.collider.gameObject.tag + ", Dist = " + hit.distance);
            if (hit.collider.gameObject.CompareTag(goalTag))
            {
                float score = goalInSightReward * 1 / hit.distance;
                totalGainedfromSight += score;
                if (totalGainedfromSight < 1)
                {
                    AddReward(score);
                } else
                {
                    //Debug.Log("gained 1.0f reward from sight");
                }
            } 
            else if (!behavParams.IsInHeuristicMode() && hit.distance < 0.35f)
            {
                AddReward(walkIntoWallPunishment);
            }
        }
        MoveAgent(actionBuffers.ContinuousActions);
        TakeActions(actionBuffers.DiscreteActions);
        counter++;
        if (counter > maxCount)
        {
            float dist = Vector3.Distance(transform.localPosition, prevPos);
            //Debug.Log("distance: " + dist);
            if (dist > 2.0f)
            {
                //Debug.Log("rewarding distance traveled: " + dist);
                AddReward(exploreReward);
            }

            prevPos = transform.localPosition;
            counter = 0;
        }

        if (Mathf.Abs(transform.localPosition.x) > 15f || Mathf.Abs(transform.localPosition.z) > 15f)
        {
            Debug.Log("Agent out of bounds. resetting.");
            EndEpisode();
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        var continuousActionsOut = actionsOut.ContinuousActions;
        var discreteActionsOut = actionsOut.DiscreteActions;
        //Debug.Log(actionsOut.ContinuousActions[0] + ", "+ actionsOut.DiscreteActions[0]);
        float degrees = Quaternion.FromToRotation(prevRot, m_AgentHead.forward).eulerAngles.y;
        rotateAmount = degrees/(rotateSpeed * Time.deltaTime);
        if (rotateAmount > 80f){rotateAmount -= 90f;}
        //Debug.Log(rotateAmount);
        rotateAmount = Mathf.Clamp(rotateAmount, -1.0f, 1.0f);
        //TestCube.transform.Rotate(rotateDir, Time.deltaTime * rotateSpeed * rotateAmount);

        // Do not record rotation past possible range
        continuousActionsOut[0] = rotateAmount;
        prevRot = m_AgentHead.forward;
        discreteActionsOut[0] = 0;

        //float rotateAngle = m_AgentHead.rotation.eulerAngles.y - prevRot.y;
        //float rotateAngle = m_AgentHead.rotation.eulerAngles.y;
        //Debug.Log(rotateAngle + ": " + rotateAngle/360);

        //Debug.Log("prev Rot: " + prevRot + ", rotate angle: " + rotateAngle + ", continuous action rotation: " + continuousActionsOut[0]);
        //prevRot = m_AgentHead.rotation.eulerAngles;

        //Debug.Log(controllers.Count);
        controllers[0].TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickL);
        controllers[1].TryGetFeatureValue(CommonUsages.primary2DAxis, out Vector2 joystickR);
        //Debug.Log(joystickL);
        //Debug.Log(joystickR);
        if (joystickL[1] > 0.5 || joystickR[1] > 0.5)
        {
            discreteActionsOut[0] = 1;
            magnitude = 1;
        }
        else
        {
            magnitude = 0;
        }
        if (placedItem)
        {
            discreteActionsOut[0] = 2;
            placedItem = false;
            SetReward(collectReward);
            Debug.Log("Agent rewarded for item collection.");
        }
        else if (startCheckout)
        {
            discreteActionsOut[0] = 2;
            startCheckout = false;
            SetReward(checkoutReward);
            Debug.Log("Agent rewarded for checkout.");
        }
    }

    
    void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.CompareTag("agent"))
        {
            /*
            LocationCloner other = col.gameObject.GetComponent<LocationCloner>();
            if (!other) { other = col.gameObject.GetComponentInParent<LocationCloner>(); }
            if (other.original.isRespawning || isRespawning)
            {
                Debug.Log("agent is respawning - ignoring collision: " + GetComponentInParent<LayoutManager>().gameObject.name);
                return;
            }
            SetReward(collisionPunishment);
            */
            numCollisions++;
            /*
            Debug.Log("agent collision - penalty: " + other.original);
            other.original.isRespawning = true;
            other.original.EndEpisode();
            StartCoroutine("RespawnAgentInSeconds");
            isRespawning = true;
            */
        } 
        else if (col.gameObject.CompareTag(goalTag))
        {
            if (list.shoppingList.Contains(col.gameObject.name))
            {
                if (col.gameObject.GetComponent<ShoppingItem>() != null)
                {
                    collisions.Add(col);
                }
            } else if (col.GetComponentInParent<CheckStand>() != null)
            {
                atCheckStand = true;
            }
        }/*
        else if (col.gameObject.CompareTag("checkout"))
        {
            if (col.GetComponentInParent<CheckStand>() != null)
            {
                atCheckStand = true;
            }
        }*/
    }
    

    void OnTriggerExit (Collider col)
    {
        if (collisions.Contains(col))
        {
            collisions.Remove(col);
        } 
        //else if (col.CompareTag("checkout") || col.CompareTag("goal"))
        else if (col.CompareTag("wall"))
        {
            if (col.GetComponentInParent<CheckStand>() != null)
            {
                atCheckStand = false;
            }
        }
    }

    private void Update()
    {
        // sums rotational velocity every step, so that decision 
        // requester can get an average measure every 5 steps.
        /*
        if (hmd.Count != 0)
        {
            hmd[0].TryGetFeatureValue(CommonUsages.deviceAngularVelocity, out Vector3 rotVelocity);
            rotationalVelocity.Set(rotationalVelocity.x + rotVelocity.x, rotationalVelocity.y + rotVelocity.y, 0);
        }
        */
        if (!demo.Record && controllers.Count != 0)
        {
            controllers[0].TryGetFeatureValue(CommonUsages.triggerButton, out bool leftTriggerPressed);
            controllers[1].TryGetFeatureValue(CommonUsages.triggerButton, out bool rightTriggerPressed);
            if (leftTriggerPressed && rightTriggerPressed)
            {
                demo.Record = true;
                canMove = true;
                //movement.enabled = true;
            }
        }
    }

    
    public void UpdateWorldTags()
    {
        foreach (string color in shelfDict.Keys.ToList())
        {
            if (!list.shoppingList.Contains(color))
            {
                // If this color is not on the shopping list, Recursively
                // set all of it's children's tags to "null"
                shelfDict[color].GetComponent<ShelfManager>().SetShelfTag("shelf");
                shelfDict.Remove(color);
            }
        }
        /*
        for(int i = 0; i < list.colors.Length; i++)
        {
            if (!list.shoppingList.Contains(list.colors[i]))
            {
                // If this color is not on the shopping list, Recursively
                // set all of it's children's tags to "null"
                shelfDict[list.colors[i]].GetComponent<ShelfManager>().RemoveGoalTag();
            } 
        }
        */
        if (list.shoppingComplete)
        {
            if (!list.checkedOut)
            {
                checkOut.SetAsGoal(true);
                //SetGoalTag("checkout");
                Debug.Log(this.name + " is now seeking checkout");
            }
            else
            {
                //SetGoalTag("exit");
                exit.SetGoalTag(true);
                checkOut.SetAsGoal(false);
            }
        } 
        else
        {
            exit.SetGoalTag(false);
            checkOut.SetAsGoal(false);
        }
    }
    
    public void CollectItem()
    {
        placedItem = true;

        UpdateWorldTags();
    }

    public void CheckOut()
    {
        startCheckout = true;
    }

    public void ExitStore()
    {
        SetReward(exitReward);
        Debug.Log("Agent successfully exited store");
        if (recordingPerformanceData)
        {
            FindObjectOfType<DataExporter>().SaveShoppingData(Time.unscaledTime - startTime, numCollisions);
        }

        EndEpisode();
    }

    IEnumerator RespawnAgentInSeconds()
    {
        Debug.Log("Respawning in seconds: " + GetComponentInParent<LayoutManager>().gameObject.name);
        
        yield return new WaitForSeconds(respawnTime);
        EndEpisode();
        yield return true;
    }

    IEnumerator StopAgentForSeconds(float time)
    {
        speed = 0f;
        rotateSpeed = 0f;
        yield return new WaitForSeconds(time);
        speed = 0.5f;
        rotateSpeed = 200f;
        yield return true;
    }

    public void SetGoalTag(string newTag)
    {
        raySensor.DetectableTags = new List<string> { "agent", "shelf", newTag, "Aisle" };
    }
}