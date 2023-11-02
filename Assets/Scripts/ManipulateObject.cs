using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;


public class ManipulateObject : MonoBehaviour
{
    private GameObject shape;
    private GameObject classicUser;
    private Transform guide;
    private bool held = false;
    private Vector3 TempVect;
    private Renderer shapeRenderer;
    public InputAction colourChange;
    public InputAction increaseScale;
    public InputAction increaseScaleScroll;
    public InputAction decreaseScale;
    public InputAction decreaseScaleScroll;
    public InputAction delete;
    public InputAction rotate;
    public InputAction move;

    private int colourPosition = 0;
    public float upperLimit = 2f;
    public float lowerLimit = .1f;
    public float scaleSpeed = .005f;
    public float scrollScaleSpeed = 0.1f;



    void Awake() {
        guide = GameObject.Find("Guide").GetComponent<Transform>();
        classicUser = GameObject.Find("Capsule Not VR(Clone)");
        shape = this.gameObject;
        shapeRenderer = shape.GetComponent<Renderer>();
        //Enable all the InputActions
        colourChange.Enable(); increaseScale.Enable(); decreaseScale.Enable(); delete.Enable(); rotate.Enable(); move.Enable(); increaseScaleScroll.Enable(); decreaseScaleScroll.Enable();

        //Connect InputActions with relevant methods
        colourChange.performed += ctx => { CycleColour(); };
        increaseScaleScroll.performed += ctx => { ChangeScale(true, true); };
        decreaseScaleScroll.performed += ctx => { ChangeScale(false, true); };
        delete.performed += ctx => Delete();

    }

    // Update is called once per frame
    void Update() {
        //If this object is being held by a user
        if (held) {
            //Hold left control to rotate object
            if (rotate.ReadValue<float>() > .01f) {
                transform.Rotate(Input.GetAxis("Mouse X")*1.25f, Input.GetAxis("Mouse Y") * 1.25f, 0, Space.Self);
            }
            //BUGGY Hold right mouse to move the object
            //Look into vector3 movetowards/away
            if (move.ReadValue<float>() > .01f) {
                transform.Translate(Input.GetAxis("Mouse X")*.25f, Input.GetAxis("Mouse Y")*.5f, Input.GetAxis("Mouse X")*.25f, Space.Self);
            }
            //Update the position of the object to the user's position
            else {
                shape.transform.position = guide.transform.position;
            }
            //Allow for held down inputs to work
            if(increaseScale.ReadValue<float>() > .01f) { ChangeScale(true, false); };
            if(decreaseScale.ReadValue<float>() > .01f) { ChangeScale(false, false); };


        }
    }

    void OnMouseDown(){
        shape.GetComponent<Rigidbody>().useGravity = false;
        shape.GetComponent<Rigidbody>().isKinematic = true;
        shape.transform.position = guide.transform.position;
        //item.transform.rotation = guide.transform.rotation;
        shape.transform.parent = classicUser.transform;
        held = true;
    }
    
    public void OnGrab() {
        held = true;
    }

    public void OnRelease() {
        held = false;
    }

    void OnMouseUp() {
        held = false;
        shape.GetComponent<Rigidbody>().useGravity = true;
        shape.GetComponent<Rigidbody>().isKinematic = false;
        shape.transform.parent = null;
        shape.transform.position = guide.transform.position;
    }

    void CycleColour() {
        //Changes the colour of the object in the order blue->green->yellow->red->blue...
        if (colourPosition == 0) {
            shapeRenderer.material.color = new Color(0, 0, 1, .5f); //blue
        }
        if (colourPosition == 1) {
            shapeRenderer.material.color = new Color(0, 1, 0, .5f); //green
        }
        if (colourPosition == 2) {
            shapeRenderer.material.color = new Color(1, 0.92f, 0.016f, .5f); //yellow
        }
        if (colourPosition == 3) {
            shapeRenderer.material.color = new Color(1, 0, 0, .5f); //red
            colourPosition = 0;
            return;
        }
        colourPosition++;
    }

    void ChangeScale(bool bigger, bool scroll) {
        //Changes the scale of the object, increasing or decreasing its size
        TempVect = shape.transform.localScale;
        //If scaling by mouse scroll wheel, use different speed
        if (scroll) {
            if (bigger) {
                TempVect.x += scrollScaleSpeed;
            }
            else {
                TempVect.x -= scrollScaleSpeed;
            }
        }        
        else if (bigger) {
                TempVect.x += scaleSpeed;
        }
        else {
                TempVect.x -= scaleSpeed;
        }
        
        //The upper and lower limits define how large or small the object can become
        if (TempVect.x > upperLimit) {
            TempVect.x = upperLimit;
        }
        else if (TempVect.x < lowerLimit) {
            TempVect.x = lowerLimit;
        }
        TempVect.y = TempVect.z = TempVect.x;
        transform.localScale = TempVect;
    }
    void Delete() {
        held = false;
        shape.SetActive(false);
    }

    public void ObjectGrabbedXR() {
        shape.GetComponent<Rigidbody>().useGravity = false;
        shape.GetComponent<Rigidbody>().isKinematic = true;
        shape.transform.position = guide.transform.position;
        //item.transform.rotation = guide.transform.rotation;
        shape.transform.parent = classicUser.transform;
        held = true;
    }
    public void ObjectReleasedXR() {
        held = false;
        shape.GetComponent<Rigidbody>().useGravity = true;
        shape.GetComponent<Rigidbody>().isKinematic = false;
        shape.transform.parent = null;
        shape.transform.position = guide.transform.position;
    }
}
