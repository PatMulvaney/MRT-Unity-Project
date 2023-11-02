using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Control the player movements in Virtual Reality
/// </summary>

// Inherite from LocomotionProvider to prevent conflict between snap turn provider, and rotation from the headset
public class PlayerControl : LocomotionProvider
{
    private PhotonView view;

    // The components from the prefab (To prevent bugs)
    public Camera cam;
    public XRController left;
    public XRController right;
    public LineRenderer leftLine;
    public LineRenderer rightLine;
    public XRInteractorLineVisual leftVisual;
    public XRInteractorLineVisual rightVisual;
    public XRRig rig;

    private GameObject head = null;

    // Start is called before the first frame update
    void Start()
    {
        view = gameObject.GetComponent<PhotonView>();

        if(!view.IsMine)
        {
            cam.enabled = false;
            left.enabled = false;
            right.enabled = false;
            leftLine.enabled = false;
            rightLine.enabled = false;
            leftVisual.enabled = false;
            rightVisual.enabled = false;
            rig.enabled = false;
        }

        // We need to get the camera from the XR Rig and not the "real" camera component
        head = rig.cameraGameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if(view.IsMine)
        {
            // Input from joysticks
            Vector3 direction = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

            // Current rotation arround the y axis from the camera 
            Vector3 headRotation = new Vector3(0, head.transform.localEulerAngles.y, 0);

            direction = Quaternion.Euler(headRotation) * direction; 

            transform.Translate(direction * Time.deltaTime * 5);
        }
        
    }
}
