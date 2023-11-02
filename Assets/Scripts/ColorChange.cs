using System.Collections;
using System.Collections.Generic;
using System.Security.AccessControl;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;
using Photon.Pun;

public class ColorChange : XRBaseInteractable
{
    // For debug with the operator => in play mode in the editor, drag the operator camera here
    [Tooltip("For debug with the operator => in play mode in the editor, drag the operator camera here")]
    public Camera cam;

    // Photon view of the color wheel to send RPC
    private PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    // Overriding the OnActivate function (Grab + Trigger)
    protected override void OnActivate(XRBaseInteractor interactor)
    {
        RaycastHit myHit;

        // Getting the raycast from the player's controller
        XRRayInteractor rayInteractor = interactor.gameObject.GetComponent<XRRayInteractor>();

        // If the ray hit somewhere valide, we retrieve the information of hit point
        if (rayInteractor.TryGetCurrent3DRaycastHit(out myHit))
        {
            // Getting the view ID of the player that launched the ray
            int viewID = interactor.transform.root.GetComponentInChildren<PhotonView>().ViewID;
            Debug.Log("On a touche : " + myHit.collider.name);

            // Getting the renderer of the material to get the main texture as a Texture2D to use .textureCoord (not working with Texture not 2D)
            // The main texture is the color wheel itself
            Renderer rend = myHit.collider.gameObject.GetComponent<Renderer>();
            Texture2D tex = rend.material.mainTexture as Texture2D;
            Debug.Log("TEXTURE 2D : " + tex);
            Vector2 pixelUV = myHit.textureCoord;
            Debug.Log("pixel UV : " + pixelUV);

            // The .textureCoord gives us the UV coordinate (between 0 and 1), so we multiply with the real texture width and height to have the good hitpoint coordinate
            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;

            Color c = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);

            float r = c.r;
            float g = c.g;
            float b = c.b;
            float a = c.a;

            // RPC do not accept complex type, so we send basics int and float
            view.RPC("ChangeColor", RpcTarget.AllBuffered, viewID, r, g, b, a);
        }
    }

    [PunRPC]
    private void ChangeColor(int id, float red, float green, float blue, float alpha)
    {
        Color newColor = new Vector4(red, green, blue, alpha);

        // Changing the color of the player's capsule that launched the raycast
        GameObject obj = PhotonView.Find(id).gameObject;
        Debug.Log("L'objet qui a appele la RPC est : " + obj);
        if(obj.name == "Capsule")
        {
            obj.GetComponent<Renderer>().material.color = newColor;
        }
        else
        {
            obj.transform.Find("Capsule").gameObject.GetComponent<Renderer>().material.color = newColor;
        }
        

    }

    // For debug with the operator
    private void OnMouseDown()
    {
        RaycastHit hit;
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, 100.0f))
        {
            Debug.Log("On a touche : " + hit.collider.name);

            Renderer rend = hit.collider.gameObject.GetComponent<Renderer>();
            Texture2D tex = rend.material.mainTexture as Texture2D;
            Debug.Log("TEXTURE 2D : " + tex);
            Vector2 pixelUV = hit.textureCoord;
            Debug.Log("pixel UV : " + pixelUV);
            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;
            Color c = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);
            Debug.Log("Pouet " + c.ToString());
        }
    }
}
