using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

/// <summary>
/// Control for the player not in VR (Operator)
/// </summary>
public class ClassicPlayerControl : MonoBehaviour
{

    // Rotation for the camera to look up and down
    private float xRotation = 0;
    private bool stopRotation = false;
    private bool menuOpen = false;
    private bool removeModifierHeld = false;
    private bool stopRotationHeld = false;
    public GameObject menu;
    private PhotonView view;

    public InputAction spawnDisplay;
    public InputAction spawnMirror;
    public InputAction spawnWhiteboard;
    public InputAction spawnCarLow;
    public InputAction spawnCarHigh;
    public InputAction removeModifier;
    public InputAction stopRotationButton;
    public InputAction menuButton;

    public GameObject display;
    public GameObject mirror;
    public GameObject whiteboard;
    public GameObject carLow;
    public GameObject carHigh;


    private List<GameObject> displayList = new List<GameObject>();
    private List<GameObject> mirrorList = new List<GameObject>();
    private List<GameObject> whiteboardList = new List<GameObject>();
    private List<GameObject> carLowList = new List<GameObject>();
    private List<GameObject> carHighList = new List<GameObject>();
    // The operator's camera
    [Tooltip("The operator's camera")]
    public Camera cam;

    // The transform of the camera
    private Transform camTrans;

    // Start is called before the first frame update
    void Awake()
    {
        view = gameObject.GetComponent<PhotonView>();
        camTrans = cam.GetComponent<Transform>();

        // If it is not the operator's view, we deactivate the camera to prevent "camera switch" between players
        if(!view.IsMine)
        {
            cam.enabled = false;
        }


        // Place the operator's camera as the worldcamera for the canvas to allow interactions with the canvas' buttons
        GameObject canvasObj = GameObject.FindGameObjectWithTag("CanvasOperator");
        canvasObj.GetComponent<Canvas>().worldCamera = cam;
        Cursor.lockState = CursorLockMode.Locked;
        GameObject canvas = GameObject.Find("Canvas");

        //Create the 2D player menu and disable it
        menu = Instantiate(menu);
        menu.transform.SetParent(canvas.transform);
        menu.transform.position = canvas.transform.position;
        menu.SetActive(false);

        //Activate InputAction objects
        spawnDisplay.Enable(); spawnMirror.Enable(); spawnWhiteboard.Enable(); spawnCarLow.Enable(); spawnCarHigh.Enable(); removeModifier.Enable(); stopRotationButton.Enable(); menuButton.Enable();


        //Connect the InputActions with relevant methods
        spawnDisplay.performed += ctx => { if (!removeModifierHeld) { spawnTool("Display"); } else { removeTool("Display"); } };
        spawnMirror.performed += ctx => { if (!removeModifierHeld) { spawnTool("Mirror"); } else { removeTool("Mirror"); } };
        spawnWhiteboard.performed += ctx => { if (!removeModifierHeld) { spawnTool("Whiteboard"); } else { removeTool("Whiteboard"); } };
        spawnCarLow.performed += ctx => { if (!removeModifierHeld) { spawnTool("CarLow"); } else { removeTool("CarLow"); } };
        spawnCarHigh.performed += ctx => { if (!removeModifierHeld) { spawnTool("CarHigh"); } else { removeTool("CarHigh"); } };

        removeModifier.performed += ctx => { removeModifierHeld = true; };
        removeModifier.canceled += ctx => { removeModifierHeld = false; };
        stopRotationButton.performed += ctx => { stopRotationHeld = true; if (menuOpen == false) { stopRotationHeld = true; } };
        stopRotationButton.canceled += ctx => { stopRotationHeld = false; if (menuOpen == false) { stopRotationHeld = false; } };
        menuButton.performed += ctx => { if (view.IsMine) { menuToggle(); } };



    }

    // Update is called once per frame
    void Update()
    {
        if(view.IsMine)
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");

            //Stop rotation of the player view so held objects can be spun
            if (Input.GetKeyDown(KeyCode.LeftControl)&&menuOpen==false) {
                stopRotation = true;
            }
            if (Input.GetKeyUp(KeyCode.LeftControl) && menuOpen == false) {
                stopRotation = false;
            }

            // Shift is here to freeze the rotation of the operator, to easily click on buttons
            // Y Rotation still applies after the stop rotation bool is disabled
            if (!stopRotation)
            {
                // the minus is here to not have reversed control of the camera up and down
                xRotation -= mouseY;

                // Clamping so that we can not do a complete rotation around the x axis
                xRotation = Mathf.Clamp(xRotation, -90, 90);

                // We change the CAMERA rotation arround the x axis (to look up and down)
                camTrans.localRotation = Quaternion.Euler(xRotation, 0, 0);

                // We change the OPERATOR (capsule) rotation arround the y axis (left and right)
                transform.Rotate(Vector3.up, mouseX);
            }

            // We translate the position of the operator (capsule) with the axis (directionnal arrows) 
            transform.Translate(Input.GetAxis("Horizontal") * 5 * Time.deltaTime, 0, Input.GetAxis("Vertical") * 5 * Time.deltaTime);

        }
    }

    private void spawnTool(string toolName) {
        //Spawns a tool into the experimental room based on the input string
        Transform spawnPoint = GameObject.Find("SpawnPoint " + toolName).transform;
        if (spawnPoint == null) {
            Debug.Log("broke");
        }
        if (toolName == "Display") {
            displayList.Add(Instantiate(display, spawnPoint.position, spawnPoint.rotation));
        }
        if (toolName == "Mirror") {
            mirrorList.Add(Instantiate(mirror, spawnPoint.position, spawnPoint.rotation));
        }
        if (toolName == "Whiteboard") {
            whiteboardList.Add(Instantiate(whiteboard, spawnPoint.position, spawnPoint.rotation));
        }
        if (toolName == "CarLow") {
            carLowList.Add(Instantiate(carLow, spawnPoint.position, spawnPoint.rotation));
        }
        if (toolName == "CarHigh") {
            carHighList.Add(Instantiate(carHigh, spawnPoint.position, spawnPoint.rotation));
        }

    }

    private void removeTool(string toolName) {
        //Removes the most recently spawned tool of that type based on the input string
        if (toolName == "Display") {
            int end = displayList.Count - 1;
            Destroy(displayList[end]);
            if (end >= 1) { 
                displayList.RemoveAt(end);
                displayList.TrimExcess();
            }
        }
        if (toolName == "Mirror") {
            int end = mirrorList.Count - 1;
            Destroy(mirrorList[end]);
            if (end >= 1) {
                mirrorList.RemoveAt(end);
                mirrorList.TrimExcess();
            }
        }
        if (toolName == "Whiteboard") {
            int end = whiteboardList.Count - 1;
            Destroy(whiteboardList[end]);
            if (end >= 1) {
                whiteboardList.RemoveAt(end);
                whiteboardList.TrimExcess();
            }
        }
        if (toolName == "CarLow") {
            int end = carLowList.Count - 1;
            Destroy(carLowList[end]);
            if (end >= 1) {
                carLowList.RemoveAt(end);
                carLowList.TrimExcess();
            }
        }
        if (toolName == "CarHigh") {
            int end = carHighList.Count - 1;
            Destroy(carHighList[end]);
            if (end >= 1) {
                carHighList.RemoveAt(end);
                carHighList.TrimExcess();
            }

        }

    }

    private void menuToggle() {
        menuOpen = !menuOpen;

        if (menuOpen) {
            menu.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            stopRotation = true;
        }
        else {
            menu.SetActive(false);
            Cursor.lockState = CursorLockMode.Locked;
            stopRotation = false;
        }
        if (stopRotationHeld) {
            stopRotation = true;
        }
    }
}
