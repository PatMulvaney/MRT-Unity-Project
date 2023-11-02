using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngineInternal;

public class TestButtonHoverVR : XRBaseInteractable
{
    public Text text;
    private XRRayInteractor ray;

    public Text colorText;
    public GameObject cubeTest;

    public Camera cam;

    RaycastHit myHit;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    protected override void OnHoverEntered(XRBaseInteractor interactor)
    {
        ray = interactor.gameObject.GetComponent<XRRayInteractor>();
        text.text = "On a hover avec " + interactor.name + ray;
        SetCameraEvent(interactor);
    }

    protected override void OnActivate(XRBaseInteractor interactor)
    {
        if (ray.TryGetCurrent3DRaycastHit(out myHit))
        {
            text.text = "Activated";
        }
        else
        {
            text.text = "Not activated";
        }
        
    }

    private void SetCameraEvent(XRBaseInteractor interactor)
    {
        Canvas canv = gameObject.GetComponentInParent<Canvas>();
        Camera cam = interactor.gameObject.transform.root.gameObject.GetComponentInChildren<Camera>();
        canv.worldCamera = cam;
    }

}
