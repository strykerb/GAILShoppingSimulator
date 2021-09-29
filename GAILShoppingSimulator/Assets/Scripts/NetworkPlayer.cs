using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using Photon.Pun;
using UnityEngine.XR.Interaction.Toolkit;

public class NetworkPlayer : MonoBehaviour
{
    
    public Transform head;
    public Transform body;
    public Transform leftHand;
    public Transform rightHand;
    public AudioSource spaceBubble;

    private PhotonView photonView;
    private NetworkManager network;
    private NetworkPlayerSpawner spawner;
    private Transform headRig;
    private Transform leftHandRig;
    private Transform rightHandRig;
    //private Rigidbody rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        photonView = GetComponent<PhotonView>();
        spawner = FindObjectOfType<NetworkPlayerSpawner>();
        XRRig rig = FindObjectOfType<XRRig>();
        headRig = rig.transform.Find("Camera Offset/Main Camera");
        leftHandRig = rig.transform.Find("Camera Offset/LeftHand Controller");
        rightHandRig = rig.transform.Find("Camera Offset/RightHand Controller");


    // Teleport XR Rig to appropriate spawn location
    rig.transform.position = spawner.currentSpawn;

        if (photonView.IsMine)
        {
            rightHand.gameObject.SetActive(false);
            leftHand.gameObject.SetActive(false);
            head.gameObject.SetActive(false);
            body.gameObject.SetActive(false);
            gameObject.tag = "Untagged";
            Destroy(GetComponent<Rigidbody>());
            //rig.GetComponentInChildren<Canvas>().enabled = false;
        } 
        else
        {
            spaceBubble.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (photonView.IsMine)
        {
            MapPosition(head, headRig, true);
            MapPosition(leftHand, leftHandRig, true);
            MapPosition(rightHand, rightHandRig, true);
            MapPosition(body, headRig, false);
        }
    }

    void MapPosition(Transform target, Transform rigTransform, bool trackRotation)
    {
        target.position = rigTransform.position;
        if (trackRotation)
        {
            target.rotation = rigTransform.rotation;
        }
    }
}
