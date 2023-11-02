using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary>
/// This class is for the movement of the client. The main difference is that the client has a rigidbody.
/// With classics components from Photon (as for the guide), the position is not perfectly sync and leads on some big mistakes about the position
/// </summary>
public class ClientPosition : MonoBehaviour, IPunObservable
{

    private Rigidbody r;

    private Vector3 realPosition;

    [Tooltip("The photonView of the capsule")]
    public PhotonView view;

    // Start is called before the first frame update
    void Start()
    {
        r = gameObject.GetComponent<Rigidbody>();
        realPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        // We update smoothly the position of the capsule for all the other players
        if(!view.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, Time.deltaTime * 15);
        }
    }

    // We send and receive the values we are interested in
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if(stream.IsWriting)
        {
            stream.SendNext(transform.position);
            stream.SendNext(transform.rotation);
        }
        else if (stream.IsReading)
        {
            realPosition = (Vector3)stream.ReceiveNext();
            transform.rotation = (Quaternion)stream.ReceiveNext();
        }
    }
}
