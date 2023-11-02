using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OperatorInstantiation : MonoBehaviour
{
    [Tooltip("Objects to instantiate")]
    public GameObject cubePrefab;
    public GameObject spherePrefab;
    public GameObject capsulePrefab;

    public GameObject inputID;

    [Tooltip("Spawn Point to spawn the objects")]
    public GameObject spawnPoint;

    // Lists to keep references of the objects we spawned
    private List<GameObject> cubeList;
    private List<GameObject> sphereList;
    private List<GameObject> capsuleList;

    // List to keep reference of the viewID from the objects instantiated
    private List<int> photonIdList;

    // Start is called before the first frame update
    void Start()
    {
        cubeList = new List<GameObject>();
        sphereList = new List<GameObject>();
        capsuleList = new List<GameObject>();
        photonIdList = new List<int>();
        spawnPoint = GameObject.Find("SpawnPoint Debug");
    }

    void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            InstantiateCube();
        }

    }
    // Every functions will instantiate / destroy through the network
    public void InstantiateCube()
    {
        //GameObject cube = Instantiate(cubePrefab, new Vector3(-8f, 0.4f, -2.9f), Quaternion.identity);
        GameObject cube = PhotonNetwork.Instantiate(cubePrefab.name, spawnPoint.transform.position, Quaternion.identity);
        cubeList.Add(cube);
        photonIdList.Add(cube.GetPhotonView().ViewID);
    }

    public void InstantiateSphere()
    {
        GameObject sphere = PhotonNetwork.Instantiate(spherePrefab.name, spawnPoint.transform.position, Quaternion.identity);
        sphereList.Add(sphere);
        photonIdList.Add(sphere.GetPhotonView().ViewID);
    }

    public void InstantiateCapsule()
    {
        GameObject capsule = PhotonNetwork.Instantiate(capsulePrefab.name, spawnPoint.transform.position, Quaternion.identity);
        capsuleList.Add(capsule);
        photonIdList.Add(capsule.GetPhotonView().ViewID);
    }

    // Destroying the last cube spawned
    public void DestroyCube()
    {
        if (cubeList.Count != 0)
        {
            PhotonNetwork.Destroy(cubeList[cubeList.Count - 1]);
            photonIdList.Remove((cubeList[cubeList.Count - 1]).GetPhotonView().ViewID);
            cubeList.Remove(cubeList[cubeList.Count - 1]);
        }
    }

    // Destroying the last sphere spawned
    public void DestroySphere()
    {
        if (sphereList.Count != 0)
        {
            PhotonNetwork.Destroy(sphereList[sphereList.Count - 1]);
            photonIdList.Remove((sphereList[sphereList.Count - 1]).GetPhotonView().ViewID);
            sphereList.Remove(sphereList[sphereList.Count - 1]);
        }
    }

    // Destroying the last capsule spawned
    public void DestroyCapsule()
    {
        if (capsuleList.Count != 0)
        {
            PhotonNetwork.Destroy(capsuleList[capsuleList.Count - 1]);
            photonIdList.Remove((capsuleList[capsuleList.Count - 1]).GetPhotonView().ViewID);
            capsuleList.Remove(capsuleList[capsuleList.Count - 1]);
        }
    }

    public void DestroyAllCube()
    {
        foreach(GameObject obj in cubeList)
        {
            photonIdList.Remove(obj.GetPhotonView().ViewID);
            PhotonNetwork.Destroy(obj);
        }
        cubeList.Clear();
        Debug.Log("La liste des cubes : " + cubeList.Count);
    }
    public void DestroyAllSphere()
    {
        foreach (GameObject obj in sphereList)
        {
            photonIdList.Remove(obj.GetPhotonView().ViewID);
            PhotonNetwork.Destroy(obj);
        }

        sphereList.Clear();
    }

    public void DestroyAllCapsule()
    {
        foreach (GameObject obj in capsuleList)
        {
            photonIdList.Remove(obj.GetPhotonView().ViewID);
            PhotonNetwork.Destroy(obj);
        }

        capsuleList.Clear();
    }

    // Allow to the input field to appear and enter the viewID
    public void EnterID()
    {
        if (photonIdList.Count != 0)
        {
            inputID.SetActive(!inputID.activeSelf);
        }
    }

    public void DestroyByID()
    {

        // Converting the string from the input field to an int and find the object corresponding to this ID.
        GameObject obj = PhotonView.Find(Int32.Parse(inputID.GetComponent<InputField>().text)).gameObject;

        if (cubeList.Contains(obj))
        {
            cubeList.Remove(obj);
        }
        else if(sphereList.Contains(obj))
        {
            sphereList.Remove(obj);
        }
        else
        {
            capsuleList.Remove(obj);
        }

        PhotonNetwork.Destroy(PhotonView.Find(Int32.Parse(inputID.GetComponent<InputField>().text)));
    }
}