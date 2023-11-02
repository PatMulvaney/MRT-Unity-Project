using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTransparency : MonoBehaviour {
    public Material opaqueMat1;
    public Material opaqueMat2;
    public Material transparentMat;
    private Renderer rend;
    private bool transparent = false;
    private Material [] mats;

    // Start is called before the first frame update
    void Start() {
        rend = GetComponent<Renderer>();
        mats = rend.materials;
    }

    // Update is called once per frame
    void Update() {
        if (Input.GetKeyDown("-")) {
            if (!transparent) {
                mats[0] = transparentMat;
                mats[1] = transparentMat;
                rend.materials = mats;
                transparent = true;
            }
            else {
                mats[0] = opaqueMat1;
                mats[1] = opaqueMat2;
                rend.materials = mats;
                transparent = false;
            }
        }
    }
}
