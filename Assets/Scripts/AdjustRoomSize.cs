using System.Collections;
using System.Collections.Generic;
using UnityEngine;

 
public class AdjustRoomSize : MonoBehaviour
{
    public bool inverse;
    public float resizeAmount;
    public string resizeDirection;
    public GameObject room;
    public GameObject roomLight;
    public GameObject clientCamera;
    public GameObject guideCamera;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.Alpha5))
        {
            resize(resizeAmount);
        }
    }

    void resize(float amount)
    {

        transform.localScale = new Vector3(2f, 1f, 2f);
        transform.position = new Vector3(-15f, transform.position.y, transform.position.z); 
        roomLight.transform.position = new Vector3(transform.position.x, 14f, transform.position.z);
        roomLight.GetComponent<Light>().range = 21;
        clientCamera.transform.position = new Vector3(transform.position.x, 19.88f, transform.position.z);
        //transform.position = new Vector3(transform.position.x + (amount / 2), transform.position.y, transform.position.z);
        //transform.localScale = new Vector3(transform.localScale.x + amount, transform.localScale.y, transform.localScale.z);
    }

    public void OnValueChanged(float value)
    {
        //room.intensity = value * 8;
        /*if (direction == "x" && inverse == false)
        {
            transform.position = new Vector3(transform.position.x + (amount / 2), transform.position.y, transform.position.z);
            transform.localScale = new Vector3(transform.localScale.x + amount, transform.localScale.y, transform.localScale.z);
        }*/
    }
}