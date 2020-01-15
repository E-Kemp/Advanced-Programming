using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightScript : MonoBehaviour
{

    public GameObject LIGHT_ON, LIGHT_OFF;
    private bool ACTIVE = false;

    public void switchState()
    {
        if (ACTIVE)
        {
            LIGHT_OFF.SetActive(true);
            LIGHT_ON.SetActive(false);
            ACTIVE = false;
        }
        else
        {
            LIGHT_ON.SetActive(true);
            LIGHT_OFF.SetActive(false);
            ACTIVE = true;
        }
    }

    public bool getState()
    {
        return ACTIVE;
    }
}
