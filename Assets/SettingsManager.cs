using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;

public class SettingsManager : MonoBehaviour
{
    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject backWall;
    public GameObject floor;
    public float scaleSpeed = .01f;
    public InputAction increaseRoomScale;
    public InputAction decreaseRoomScale;


    // Start is called before the first frame update
    void Start()
    {
        increaseRoomScale.Enable();
        decreaseRoomScale.Enable();
        increaseRoomScale.performed += ctx => { ScaleRoom(true); };
        decreaseRoomScale.performed += ctx => { ScaleRoom(false); };
    }


    private void ScaleRoom(bool bigger) {
        //Set up all the temporary scale and position objects
        Vector3 floorScale = floor.transform.localScale;
        Vector3 floorPos = floor.transform.position;
        Vector3 backWallPos = backWall.transform.position;
        Vector3 rightWallPos = rightWall.transform.position;
        Vector3 leftWallPos = leftWall.transform.position;
        Vector3 backWallScale = backWall.transform.localScale;
        Vector3 rightWallScale = rightWall.transform.localScale;
        Vector3 leftWallScale = leftWall.transform.localScale;

        if (bigger) {
            //Increase the scale and adjust the position of the floor and walls
            floorScale.y = floorScale.z = floorScale.x += scaleSpeed;
            floorPos.x -= scaleSpeed * 5;
            backWallPos.x -= scaleSpeed * 10;
            backWallPos.z -= scaleSpeed * 5;
            backWallScale.z += scaleSpeed * 3.33333f;
            leftWallPos.x -= scaleSpeed * 10;
            leftWallPos.z -= scaleSpeed * 5;
            leftWallScale.z += scaleSpeed * 3.33333f;
            rightWallPos.x -= scaleSpeed * 10;
            rightWallPos.z += scaleSpeed * 5;
            rightWallScale.z += scaleSpeed * 3.33333f;

        }
        else {
            //Decrease the scale and adjust the position of the floor and walls
            floorScale.y = floorScale.z = floorScale.x -= scaleSpeed;
            floorPos.x += scaleSpeed * 5;
            backWallPos.x += scaleSpeed * 10;
            backWallPos.z += scaleSpeed * 5;
            backWallScale.z -= scaleSpeed * 3.33333f;
            leftWallPos.x += scaleSpeed * 10;
            leftWallPos.z += scaleSpeed * 5;
            leftWallScale.z -= scaleSpeed * 3.33333f;
            rightWallPos.x += scaleSpeed * 10;
            rightWallPos.z -= scaleSpeed * 5;
            rightWallScale.z -= scaleSpeed * 3.33333f;
        }
        //Apply the changes to the floor and wall gameobjects
        floor.transform.localScale = floorScale;
        floor.transform.position = floorPos;
        backWall.transform.position = backWallPos;
        backWall.transform.localScale = backWallScale;
        leftWall.transform.position = leftWallPos;
        leftWall.transform.localScale = leftWallScale;
        rightWall.transform.position = rightWallPos;
        rightWall.transform.localScale = rightWallScale;
    }
}
