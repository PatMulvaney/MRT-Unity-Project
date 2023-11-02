using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

/// <summary>
/// A class to handle the controllers' inputs to change the layer / scale of an object.
/// Remote manipulation is also implemented here.
/// </summary>
public class AppearClientLayer : MonoBehaviour
{

    // Photon view of the object
    private PhotonView view;

    // XR Grab script of the object
    private XRGrabInteractable xrGrab;

    // Is the object grabbed
    public bool isSelected = false;

    // Is the client holding the object 
    private bool isClientHolding = false;

    [HideInInspector]
    public static bool isGuideHolding = false;

    // The controllers of the player
    private InputDevice rightController;
    private InputDevice leftController;

    public Text DeviceText;
    public Text PrimaryButtonText;
    public Text DeviceCharac;
    public Text RpcBleu;
    public Text RpcRouge;

    //  The player who is holding the object
    private GameObject ObjectGraber;

    // A list of the controllers we found
    private List<InputDevice> devices = new List<InputDevice>();

    //An array of color for the client
    private Color[] colorArray = {Color.white, Color.blue, Color.green, Color.red, Color.yellow, Color.cyan, Color.magenta, Color.gray, Color.black};

    //int to know where is the client in the color array
    int currentColor = 0;

    //Allow to not loop through the color every frame
    private bool hasPressedColorClient = false;

    // Characteristics of the devices we want to find
    InputDeviceCharacteristics rightControllerChar = InputDeviceCharacteristics.Right;
    InputDeviceCharacteristics leftControllerChar = InputDeviceCharacteristics.Left;

    bool controllerFoundRight = false;
    bool controllerFoundLeft = false;

    /*Remote manipulation part*/

    private bool isHovering = false;
    public bool isRemote = false;

    private bool hasPressedTrigger = false;

    //Position of the hand at the last frame
    Vector3 lastFrameHand;

    private bool isClientRemoting = false;
    private bool isGuideRemoting = false;

    //The hand to follow
    GameObject handHovering;

    //Rotation offset at the start of the manipulation
    Quaternion rotationOffset;

    //Event to prevent the guide to be able to open the dashboard
    public delegate void GuideRemoting();
    public static event GuideRemoting GuideRemote;

    //Event to give back the ownership
    public delegate void RemoteOwnershipBack();
    public static event RemoteOwnershipBack RemoteBack;

    private void OnEnable()
    {
        xrGrab = GetComponent<XRGrabInteractable>();

        // Fonctions to listen when we grab and release the object
        xrGrab.onSelectEntered.AddListener(OnSelectEnter);
        xrGrab.onSelectExited.AddListener(OnSelectExit);
        xrGrab.onHoverEntered.AddListener(IsHoveringObject);
        xrGrab.onHoverExited.AddListener(IsLeavingObject);
    }

    // Start is called before the first frame update
    void Start()
    {

        view = GetComponent<PhotonView>();
        RpcBleu = GameObject.Find("Text RPC Bleu").GetComponent<Text>();
        RpcRouge = GameObject.Find("Text RPC Rouge").GetComponent<Text>();

        // getting the right controller
        InputDevices.GetDevicesWithCharacteristics(rightControllerChar,  devices);

        if(devices.Count > 0)
        {
            //DeviceText.text = "Devices not null : " + devices.Count + " " + devices[0].name;

            // We are assuming that there is only 1 right controller and get it 
            rightController = devices[0];
            controllerFoundRight = true;
        }
        else
        {
            //DeviceText.text = "Devices null";
        }


        // Same for left controller
        InputDevices.GetDevicesWithCharacteristics(leftControllerChar, devices);

        if(devices.Count > 0)
        {
            DeviceCharac.text = "Devices left not null " + devices.Count + " " + devices[0].name;
            leftController = devices[0];
            controllerFoundLeft = true;
        }
        else
        {
            DeviceCharac.text = "Left device null";
        }

        

    }

    // Update is called once per frame
    void Update()
    {

        // If we did not find a controller (because it was disconnected) we keep looking to find one
        if(!controllerFoundRight)
        {
            InputDevices.GetDevicesWithCharacteristics(rightControllerChar, devices);
            if(devices.Count > 0)
            {
                rightController = devices[0];
                controllerFoundRight = true;
                DeviceText.text = "Devices not null : " + devices.Count + " " + devices[0].name;
            }
        }

        if(!controllerFoundLeft)
        {
            InputDevices.GetDevicesWithCharacteristics(leftControllerChar, devices);
            if(devices.Count > 0)
            {
                leftController = devices[0];
                controllerFoundLeft = true;
                DeviceCharac.text = "Devices left not null " + devices.Count + " " + devices[0].name;
            }
        }

        #region grabSelection

        // If the object is grabbed
        if (isSelected)
        {
            // Looking for the inputs of the controllers
            rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool primary);
            if(primary)
            {
                //If the client is not holding, it is the guide
                if(!isClientHolding)
                {
                    // The target is all buffered because we want to make the changements appear for players that are not connected yet
                    view.RPC("ChangeLayerToClient", RpcTarget.AllBuffered);
                }
                else
                {
                    // The client is holding and want to change the color of the object
                    if(!hasPressedColorClient) // To not change the color every frame
                    {
                        view.RPC("ChangeObjectColor", RpcTarget.AllBuffered);
                        hasPressedColorClient = true;
                    }   
                }
            }
            //if we are not holding the button, we released it
            else
            {
                //we can allow the client to change the color
                hasPressedColorClient = false;
            }

            // The guide can change back the layer with the secondary button
            rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondary);
            if(secondary && !isClientHolding)
            {
                view.RPC("ChangeLayerToGuide", RpcTarget.AllBuffered);
            }


            // With the left controller, both client and guide can change the scale of the object they are holding
            leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryLeft);
            if(primaryLeft)
            {
                // Checking if the object will not "disappear" if we scale it down too much
                if (!(gameObject.transform.localScale.sqrMagnitude <= 0.035f))
                {
                    // We are getting input every frame, we substract a "little" vector to not make it fast
                    gameObject.transform.localScale = gameObject.transform.localScale - new Vector3(0.005f, 0.005f, 0.005f);
                }
            }

            leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryLeft);
            if (secondaryLeft)
            {
                    gameObject.transform.localScale = gameObject.transform.localScale + new Vector3(0.005f, 0.005f, 0.005f);
            }
        }

        #endregion

        #region remoteSelection

        //Start of the remote manipulation
        if(isHovering && !isRemote)
        {
            rightController.TryGetFeatureValue(CommonUsages.trigger, out float triggerHover);
            if (triggerHover > 0.5f && !hasPressedTrigger)
            {
                /*To prevent the possibility to manipulate multiple objects at the same time
                 *It is not the best way to do it (I think)*/
                if(!RemoteFixMultiple.hasFoundObject)
                {
                    view.TransferOwnership(handHovering.transform.root.gameObject.GetComponentInChildren<PhotonView>().Owner);

                    //Rotation offset between the hand and the object at the start
                    rotationOffset = Quaternion.Inverse(handHovering.transform.rotation) * gameObject.transform.rotation;

                    //First position of the hand
                    lastFrameHand = handHovering.transform.position;

                    isRemote = true;
                    gameObject.GetComponent<Rigidbody>().isKinematic = true;

                    if (handHovering.transform.root.gameObject.CompareTag("Client"))
                    {
                        isClientRemoting = true;
                    }
                    else if (handHovering.transform.root.gameObject.CompareTag("Guide"))
                    {
                        isGuideRemoting = true;
                    }

                    handHovering.GetComponent<XRInteractorLineVisual>().enabled = false;

                    if (GuideRemote != null)
                    {
                        GuideRemote();
                    }

                    RemoteFixMultiple.hasFoundObject = true;
                }
            }
        }

        //The object will start to follow the movements
        if(isRemote)
        {
            float speedPos;
            Vector3 distance;

            Vector3 currentFrameHand = handHovering.transform.position;
            Quaternion handRotation = handHovering.transform.rotation;

            //Calculate the position difference for the hand between 2 frames
            Vector3 HandDifference = currentFrameHand - lastFrameHand;

            //Create a factor to amplify the movements depending on the distance
            distance = gameObject.transform.position - handHovering.transform.position;
            speedPos = distance.sqrMagnitude;
            speedPos = Mathf.Clamp(speedPos, 5f, 20f);

            gameObject.transform.position = Vector3.Lerp(gameObject.transform.position, gameObject.transform.position + (HandDifference * speedPos), Time.deltaTime * 15);
            gameObject.transform.rotation = handHovering.transform.rotation * rotationOffset;

            lastFrameHand = handHovering.transform.position;

            //Ending the remote manipulation
            rightController.TryGetFeatureValue(CommonUsages.trigger, out float triggerRemote);
            if (triggerRemote < 0.5f)
            {
                if(!hasPressedTrigger)
                {
                    gameObject.GetComponent<Rigidbody>().isKinematic = false;

                    isRemote = false;
                    handHovering.GetComponent<XRInteractorLineVisual>().enabled = true;
                    handHovering = null;

                    if(GuideRemote != null)
                    {
                        GuideRemote();
                    }

                    if (RemoteBack != null)
                    {
                        RemoteBack();
                    }

                    RemoteFixMultiple.hasFoundObject = false;
                }
                
            }

            rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryRemote);
            if (primaryRemote)
            {
                //If the client is not holding, it is the guide
                if (!isClientRemoting)
                {
                    // The target is all buffered because we want to make the changements appear for players that are not connected yet
                    view.RPC("ChangeLayerToClient", RpcTarget.AllBuffered);
                }
                else
                {
                    // The client is holding and want to change the color of the object
                    if (!hasPressedColorClient) // To not change the color every frame
                    {
                        view.RPC("ChangeObjectColor", RpcTarget.AllBuffered);
                        hasPressedColorClient = true;
                    }
                }
            }
            //if we are not holding the button, we released it
            else
            {
                //we can allow the client to change the color
                hasPressedColorClient = false;
            }

            rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryRemote);
            if (secondaryRemote && !isClientRemoting)
            {
                view.RPC("ChangeLayerToGuide", RpcTarget.AllBuffered);
            }


            // With the left controller, both client and guide can change the scale of the object they are holding
            leftController.TryGetFeatureValue(CommonUsages.primaryButton, out bool primaryLeftRemote);
            if (primaryLeftRemote)
            {
                // Checking if the object will not "disappear" if we scale it down too much
                if (!(gameObject.transform.localScale.sqrMagnitude <= 0.035f))
                {
                    // We are getting input every frame, we substract a "little" vector to not make it fast
                    gameObject.transform.localScale = gameObject.transform.localScale - new Vector3(0.005f, 0.005f, 0.005f);
                }
            }

            leftController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool secondaryLeftRemote);
            if (secondaryLeftRemote)
            {
                gameObject.transform.localScale = gameObject.transform.localScale + new Vector3(0.005f, 0.005f, 0.005f);
            }

        }

        #endregion
    }

    // Called when we grab an object
    private void OnSelectEnter(XRBaseInteractor obj)
    {
        isSelected = true;

        //We retrieve the "player" who is grabing the object
        ObjectGraber = obj.transform.root.gameObject;

        // For the client to not collide with the object he is holding (if he collides, some weird comportments appear)
        Physics.IgnoreLayerCollision(13, 13, true);

        // To know which player is holding the object
        if(ObjectGraber.CompareTag("Client"))
        {
            isClientHolding = true;
        }

        else if(ObjectGraber.CompareTag("Guide"))
        {
            isGuideHolding = true;
        }

        Debug.Log("Is selected = true");

        //When grabbing an object which was remote manipulated
        if(isRemote)
        {
            handHovering.GetComponent<XRInteractorLineVisual>().enabled = true;
            isRemote = false;
            gameObject.GetComponent<Rigidbody>().isKinematic = false;
            RemoteFixMultiple.hasFoundObject = false;
        }

        obj.GetComponent<XRInteractorLineVisual>().enabled = false;
    }

    // Called when we release an object
    private void OnSelectExit(XRBaseInteractor obj)
    {
        isSelected = false;

        // The client can have back his collisions when he release an object
        Physics.IgnoreLayerCollision(13, 13, false);

        ObjectGraber = null;
        isClientHolding = false;
        isGuideHolding = false;

        obj.GetComponent<XRInteractorLineVisual>().enabled = true;
    }

    //For the remote manipulation
    private void IsHoveringObject(XRBaseInteractor interactor)
    {
        isHovering = true;

        if(handHovering == null)
        {
            handHovering = interactor.gameObject;
        }
        else if(handHovering != null && !isRemote)
        {
            handHovering = interactor.gameObject;
        }

        RpcRouge.text = "On est en train de hover " + handHovering.name;
    }

    private void IsLeavingObject(XRBaseInteractor interactor)
    {
        isHovering = false;
        RpcRouge.text = "On a quitté le hover";
    }

    // Rpc to change the layer and the material
    // Changing the layer first will end up with the material not changing, so we change the material first
    [PunRPC]
    public void ChangeLayerToClient()
    {
        Renderer rend = gameObject.GetComponent<MeshRenderer>();
        Color color_client = rend.material.color;
        color_client.a = 1f;
        rend.material.color = color_client;

        gameObject.layer = LayerMask.NameToLayer("Client Layer");
        PrimaryButtonText.text = "Primary button pressed !" + gameObject.layer;
        RpcBleu.text = "On a appelé la RPC Bleu !";
    }

    [PunRPC]
    public void ChangeLayerToGuide()
    {
        Renderer rend = gameObject.GetComponent<Renderer>();
        Color color_guide = rend.material.color;
        color_guide.a = 0.45f;
        rend.material.color = color_guide;

        gameObject.layer = LayerMask.NameToLayer("Guide Layer");
        PrimaryButtonText.text = "Secondary button pressed !" + gameObject.layer;
        RpcRouge.text = "On a appelé la RPC Rouge !";
    }

    //Rpc for the client to change the object's color
    [PunRPC]
    void ChangeObjectColor()
    {
        gameObject.GetComponent<Renderer>().material.color = colorArray[(currentColor++)%9];
    }
}
