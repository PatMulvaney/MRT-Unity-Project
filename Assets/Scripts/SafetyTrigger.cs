using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class SafetyTrigger : MonoBehaviour
{
    PhotonView view;
    // Start is called before the first frame update
    void Start()
    {
        view = GetComponent<PhotonView>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("ObjectExperimental"))
        {
           int id =  other.gameObject.GetComponent<PhotonView>().ViewID;
           view.RPC("TriggerChangeLayer", RpcTarget.AllBuffered, id);
            Debug.Log("Objet entre dans le trigger");
        }
    }

    [PunRPC]
    public void TriggerChangeLayer(int id)
    {
        GameObject obj = PhotonView.Find(id).gameObject;
        Renderer rend = obj.GetComponent<Renderer>();
        Color color_guide = rend.material.color;
        color_guide.a = 0.45f;
        rend.material.color = color_guide;

        obj.layer = LayerMask.NameToLayer("Guide Layer");
    }
}
