using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdjustRoomBrightness : MonoBehaviour
{
    public Light roomLight;

    public void OnValueChanged(float value)
    {
        roomLight.intensity = value * 8;
    }
}
