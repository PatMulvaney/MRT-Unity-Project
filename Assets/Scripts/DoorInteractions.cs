using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit;

public class DoorInteractions : MonoBehaviour
{


    public Animator anim;
    private bool animating = false;
    private bool isOpen = false;

    // Start is called before the first frame update

    IEnumerator OpenDoor()
    {
        isOpen = !isOpen;
        anim.SetBool("isOpen", isOpen);
        animating = true;
        yield return new WaitForSeconds(2);
        animating = false;
        Debug.Log("Door Open");
    }

    private void OnMouseDown()
    {
        Debug.Log("CLicked");
        if (animating == false) {
            StartCoroutine(OpenDoor());
        }
    }
}
