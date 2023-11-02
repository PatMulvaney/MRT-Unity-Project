using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RemoteFixMultiple : MonoBehaviour
{
    public static bool hasFoundObject;

    private void Start()
    {
        hasFoundObject = false;
    }

    private void Update()
    {
        Debug.Log("has Found Object : " + hasFoundObject);
    }
}
