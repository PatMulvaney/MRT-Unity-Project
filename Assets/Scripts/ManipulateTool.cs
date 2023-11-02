using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManipulateTool : MonoBehaviour {
    private GameObject tool;
    private GameObject classicUser;
    private Transform guide;
    private bool held = false;
    private Vector3 tempVect;
    private bool setupComplete = false;
    private float tempScroll;

    // Start is called before the first frame update
    void Start() {
        tool = this.gameObject;
    }

    // Update is called once per frame
    void Update() {
        //If this object is being held
        if (held) {
            //Update the position of the object to the user's position
            tool.transform.position = guide.transform.position;
            //Scale the object according to the scroll wheel
            tempScroll = Input.GetAxis("Mouse ScrollWheel");
            if (tempScroll != 0) {
                scrollScale(tempScroll);
            }
            if (Input.GetKeyDown(KeyCode.Delete)) {
                held = false;
                tool.SetActive(false);
            }
        }

    }

    void OnMouseDown() {
        //Debug.Log("Mouse clicked on a tool");
        if (!setupComplete) {
            setup();
        }
        if (setupComplete) {
            tool.transform.position = guide.transform.position;
            //item.transform.rotation = guide.transform.rotation;
            //tool.transform.parent = classicUser.transform;
            held = true;
        }
    }

    void OnMouseUp() {
        held = false;
        tool.transform.parent = null;
        tool.transform.position = guide.transform.position;
    }

    void setup() {
        guide = GameObject.Find("Guide").GetComponent<Transform>();
        classicUser = GameObject.Find("Capsule Not VR(Clone)");
        if (classicUser != null) {
            setupComplete = true;
        }
    }
    //Scales the tool according to the mouse wheel
    private void scrollScale(float tempScroll) {
        tempVect = tool.transform.localScale;
        if (tool.tag == "Display") {
            tempVect.y += tempScroll;

            tempVect.z += tempScroll * 2;
        }
        if (tool.tag == "Mirror") {
            tempVect.x += tempScroll;
            tempVect.y += tempScroll * 1.2f;
        }
        if (tool.tag == "Whiteboard") {
            tempVect.x += tempScroll;
            tempVect.y += tempScroll;
        }
        if (tool.tag == "Car") {
            tempVect.x += tempScroll * .25f;
            tempVect.y += tempScroll * .25f;
            tempVect.z += tempScroll * .25f;
        }
        for (int i = 0; i < 3; i++) {
            if (tempVect[i] <= 0.1f) {
                tempVect[i] = 0.1f;
            }
        }
        tool.transform.localScale = tempVect;
    }
}
