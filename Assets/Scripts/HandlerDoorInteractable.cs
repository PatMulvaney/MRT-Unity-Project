using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class HandlerDoorInteractable : XRBaseInteractable
{

    public Text debugHover;
    public PhotonView doorView;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    protected void onHoverEntered(XRBaseInteractor interactor)
    {
        debugHover.text = "On a hover sur : " + gameObject.name;
        Debug.Log("Door handle Hover");

    }

    public void DoorOwnership()
    {
        doorView.RequestOwnership();
        debugHover.text = "On a request le ownership de la door : " + doorView.Owner.ToString();
    }
}
