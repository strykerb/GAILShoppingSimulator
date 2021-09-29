using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    public Dictionary<int, Player> playerList;
    // Start is called before the first frame update
    void Start()
    {
        ConnectToServer();
    }

    // Update is called once per frame
    void ConnectToServer()
    {
        PhotonNetwork.ConnectUsingSettings();
        Debug.Log("Trying to connect to server...");

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Server.");
        base.OnConnectedToMaster();
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.MaxPlayers = 10;
        roomOptions.IsVisible = true;
        roomOptions.IsOpen = true;
        PhotonNetwork.JoinOrCreateRoom("Room1", roomOptions, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("joined a room");
        playerList = PhotonNetwork.CurrentRoom.Players;
        foreach (int id in playerList.Keys)
        {
            Debug.Log("Player " + id);
        }
        base.OnJoinedRoom();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log("A new player joined a room:" + newPlayer.ActorNumber);
        playerList = PhotonNetwork.CurrentRoom.Players;
        foreach (int id in playerList.Keys)
        {
            Debug.Log("Player " + id);
        }
        base.OnPlayerEnteredRoom(newPlayer);
    }
}
