using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class ObjectPosition : MonoBehaviour, IPunObservable
{
    private PhotonView view;
    private Rigidbody rb;

    private Vector3 realPosition;
    private Quaternion realRotation;

    // Start is called before the first frame update
    void Start()
    {
        view = gameObject.GetComponent<PhotonView>();
        rb = gameObject.GetComponent<Rigidbody>();
        realPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (!view.IsMine)
        {
            transform.position = Vector3.Lerp(transform.position, realPosition, Time.deltaTime * 15);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);

        }
        else if (stream.IsReading)
        {
            realPosition = (Vector3)stream.ReceiveNext();
        }
    }
}
