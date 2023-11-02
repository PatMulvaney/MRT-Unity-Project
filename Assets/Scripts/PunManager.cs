using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR;

/// <summary>
/// Script to handle the multiplayer connections
/// </summary>
public class PunManager : MonoBehaviourPunCallbacks
{
    public Text txtDebug;
    public Text fpsDebug;
    public bool classic = false;

    public GameObject SpawnPoint2D;
    public GameObject SpawnPointVR;
    public GameObject CapsuleVRPrefab;
    public GameObject CapsuleNotVRPrefab;

    // Start is called before the first frame update
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(PhotonNetwork.NetworkClientState.ToString() != "Joined")
        {
            txtDebug.text = PhotonNetwork.NetworkClientState.ToString();
        }
        else
        {
            txtDebug.text = "Connected to room : " + PhotonNetwork.CurrentRoom.Name + ", Players Online : " + PhotonNetwork.CurrentRoom.PlayerCount + " DEVICE " + XRSettings.isDeviceActive + " " + XRSettings.loadedDeviceName;
        }

        fpsDebug.text = (1.0f / Time.deltaTime).ToString();

    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connecte au master");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        Debug.Log("On joining a lobby");
        RoomOptions opt = new RoomOptions();
        opt.MaxPlayers = 20;
        PhotonNetwork.JoinOrCreateRoom("Therapy", opt, TypedLobby.Default);
        Debug.Log("On room created");
    }

    public override void OnJoinedRoom()
    {
        if (XRSettings.isDeviceActive)
        {
            GameObject playerVR = PhotonNetwork.Instantiate(CapsuleVRPrefab.name, SpawnPointVR.transform.position, Quaternion.identity, 0);

        }
        else
        {
            GameObject playerClassic = PhotonNetwork.Instantiate(CapsuleNotVRPrefab.name, SpawnPoint2D.transform.position, Quaternion.identity, 0);
            classic = true;
        }
        
    }

}
