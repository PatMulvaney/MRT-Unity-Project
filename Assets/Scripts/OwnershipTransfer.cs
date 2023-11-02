using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.XR;
using UnityEngine.UI;

/// <summary>
/// Transfer the ownership of the object grabbed (used with OnSelectEnter)
/// </summary>
public class OwnershipTransfer : MonoBehaviourPun
{
    public Text txtOwnership;

    // Start is called before the first frame update
    void Start()
    {
        txtOwnership = GameObject.Find("Debug Ownership").GetComponent<Text>();
        AppearClientLayer.RemoteBack += StartTransfer;
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void OnDisable()
    {
        AppearClientLayer.RemoteBack -= StartTransfer;
    }

    public void Transfer()
    {
        photonView.RequestOwnership();
    }

    // Gives the ownership of the object to the operator or guide if there is not an operator in the room
    public void TransferOperator()
    {
        txtOwnership.text = ("Resultat du transfert : " + (!gameObject.GetComponent<AppearClientLayer>().isSelected && !gameObject.GetComponent<AppearClientLayer>().isRemote));
        if (!gameObject.GetComponent<AppearClientLayer>().isSelected && !gameObject.GetComponent<AppearClientLayer>().isRemote)
        {
            GameObject Operator = GameObject.FindGameObjectWithTag("Operator");
            if (Operator == null)
            {
                PhotonView viewGuide = GameObject.FindGameObjectWithTag("Guide").GetComponent<PhotonView>();
                txtOwnership.text = txtOwnership.text + "Ownership transfere au guide";
                photonView.TransferOwnership(viewGuide.Owner);
            }
            else
            {
                PhotonView viewOperator = Operator.GetComponent<PhotonView>();
                photonView.TransferOwnership(viewOperator.Owner);
                txtOwnership.text = txtOwnership.text +  "Ownership transfere a l'operator " + viewOperator.ToString() + " " + viewOperator.ViewID;
            }

        }
    }

    public void StartTransfer()
    {
        StartCoroutine("CoroutineTransferOperator");
    }

    //To delay the transfer when the object is moving
    private IEnumerator CoroutineTransferOperator()
    {
        yield return new WaitForSeconds(1.5f);
        TransferOperator();
        yield return null;
    }
}
