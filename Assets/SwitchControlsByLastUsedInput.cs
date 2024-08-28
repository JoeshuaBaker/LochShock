using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class SwitchControlsByLastUsedInput : MonoBehaviour
{
    public GameObject mouseKeyboardControls;
    public GameObject gamepadControls;

    // Update is called once per frame
    void Update()
    {
        double lastUsedTimestamp = 0;
        InputDevice lastUsedDevice = null;
        foreach(InputDevice device in InputSystem.devices)
        {
            if(device.lastUpdateTime > lastUsedTimestamp)
            {
                lastUsedTimestamp = device.lastUpdateTime;
                lastUsedDevice = device;
            }
        }

        if(lastUsedDevice is Mouse || lastUsedDevice is Keyboard)
        {
            mouseKeyboardControls.SetActive(true);
            gamepadControls.SetActive(false);
        }
        else if(lastUsedDevice is Gamepad)
        {
            mouseKeyboardControls.SetActive(false);
            gamepadControls.SetActive(true);
        }
    }
}
