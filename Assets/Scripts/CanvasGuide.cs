using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.UI;

/// <summary>
/// A class to handle the guide's dashboard behavior. Used when there is not an operator in the room.
/// </summary>
public class CanvasGuide : MonoBehaviour
{
    // Used to retrieve the right controller's inputs (as in AppearClientLayer)
    private InputDevice rightController;
    bool controllerFoundRight = false;

    private List<InputDevice> devices = new List<InputDevice>();
    InputDeviceCharacteristics rightControllerChar = InputDeviceCharacteristics.Right;

    private Text RpcBleu;
    private Text RpcRouge;


    // Boolean to know in which states the guide's canvas is
    private bool hasPressedCanvas = false;
    private bool isCanvasActive = false;

    private bool hasReleaseCanvas = false;
    private bool hasPressedReleaseCanvas = false;

    
    public GameObject canvasGuide;
    public GameObject leftHand;

    private bool isLeftHandHolding = false;
    private bool isRightHandHolding = false;

    private bool isGuideRemoting = false;

    // Start is called before the first frame update
    void Start()
    {
        RpcBleu = GameObject.Find("Text RPC Bleu").GetComponent<Text>();
        RpcRouge = GameObject.Find("Text RPC Rouge").GetComponent<Text>();

        // getting the right controller
        InputDevices.GetDevicesWithCharacteristics(rightControllerChar, devices);

        if (devices.Count > 0)
        {
            // We are assuming that there is only 1 right controller and get it 
            rightController = devices[0];
            controllerFoundRight = true;
        }

        AppearClientLayer.GuideRemote += Remoting;
    }

    private void OnDisable()
    {
        AppearClientLayer.GuideRemote -= Remoting;
    }

    // Update is called once per frame
    void Update()
    {

        // If one of the hand is holding an object, we deactivate the dashboard.
        if(isRightHandHolding || isLeftHandHolding)
        {
            isCanvasActive = false;
            canvasGuide.SetActive(isCanvasActive);
        }

        // If we did not find a controller (because it was disconnected) we keep looking to find one
        if (!controllerFoundRight)
        {
            InputDevices.GetDevicesWithCharacteristics(rightControllerChar, devices);
            if (devices.Count > 0)
            {
                rightController = devices[0];
                controllerFoundRight = true;
            }
        }


        rightController.TryGetFeatureValue(CommonUsages.primaryButton, out bool canvasAppear);
        if (canvasAppear && (!isLeftHandHolding && !isRightHandHolding && !isGuideRemoting))
        {
            RpcBleu.text = "click for canvas appear !";
            GameObject Operator = GameObject.FindGameObjectWithTag("Operator");
            
            //If there is not an operator in the scene, we can activate the dashboard from the operator
            // HasPressedCanvas is used to not update the state at every frame (because canvasAppear is true while we are holding the button)
            if (Operator == null && hasPressedCanvas == false)
            {

                //Toogling the value of the dashboard
                isCanvasActive = !isCanvasActive;
                canvasGuide.SetActive(isCanvasActive);

                hasPressedCanvas = true;
                RpcRouge.text = "Canvas Guide changer !";
            }
        }

        // The button is released, the guide can make appear / disappear the canvas again
        else
        {
            hasPressedCanvas = false;
        }


        rightController.TryGetFeatureValue(CommonUsages.secondaryButton, out bool canvasRelease);
        if (canvasRelease)
        {
            // The canvas is active and we did not press the button
            if (hasPressedReleaseCanvas == false && isCanvasActive == true)
            {
                // The canvas is not released
                if (hasReleaseCanvas == false)
                {
                    // We get the world dashboard's position, we remove the parent and give it the new position
                    Vector3 worldPos = canvasGuide.transform.TransformPoint(canvasGuide.transform.localPosition);
                    canvasGuide.transform.parent = null;
                    canvasGuide.transform.localPosition = worldPos;

                    // The canvas is released, and we pressed the button
                    hasReleaseCanvas = !hasReleaseCanvas;
                    hasPressedReleaseCanvas = true;

                    RpcRouge.text = "On a release le canvas (basique) : " + canvasGuide.transform.position + " " + canvasGuide.transform.parent;
                    
                }

                //The canvas is released
                else
                {
                    //The new parent of the dashboard is the left hand, and we change the position of the dashboard
                    canvasGuide.transform.SetParent(leftHand.transform);
                    canvasGuide.transform.position = leftHand.transform.position;

                    // We give at a little offset to not be too close, and we give it a neutral rotation (to match the rotation of the capsule)
                    canvasGuide.transform.localPosition += new Vector3(0, 0, 0.3f);
                    canvasGuide.transform.localRotation = Quaternion.identity;

                    hasReleaseCanvas = !hasReleaseCanvas;
                    RpcRouge.text = "On rappelle le canvas : " + canvasGuide.transform.position + " " + canvasGuide.transform.parent.name;
                    hasPressedReleaseCanvas = true;    
                }
            }
            
        }
        else
        {
            //The button is released
            hasPressedReleaseCanvas = false;
        }

        RpcBleu.text = "Les booleens : " + "isCanvasActive : " + isCanvasActive + " " + "hasReleaseCanvas : " + hasReleaseCanvas + " " + "hasPressedRelease : " + hasPressedReleaseCanvas;
    }

    // They are called from the Controller events (from XR Ray Interactor of the prefab), when we select / Exit from an object
    public void RightHandHold()
    {
        isRightHandHolding = !isRightHandHolding;
    }

    public void LeftHandHold()
    {
        isLeftHandHolding = !isLeftHandHolding;
    }

    //For the remote manipulation
    public void Remoting()
    {
        isGuideRemoting = !isGuideRemoting;
    }
}
