using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnableDisable : MonoBehaviour
{
    public string key;
    public string child;
    private GameObject childObject;

    // Start is called before the first frame update
    void Awake()
    {
        childObject = transform.Find(child).gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(key)) {
            if (childObject.activeSelf == true) {
                childObject.SetActive(false);
            }
            else {
                childObject.SetActive(true);
            }
        }
    }
}
