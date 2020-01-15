using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GearLightScript : GearScript
{
    public delegate void PuzzleCheckHandler();
    public event PuzzleCheckHandler puzzleChecker;

    public LightScript[] LIGHT_ARRAY;

    public override bool trigger(string[] args)
    {
        if (active) // If the gear is active
        {
            foreach(var light in LIGHT_ARRAY)
            {
                light.switchState();
            }
            puzzleChecker();
            return true;
        }
        else
        {
            sendMessage(HOOK + " isn't active!");
            return false;
        }
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        setHookActive(); // Enable the gear
        GEAR.SetActive(false); // Remove the gear
        sendMessage(this.HOOK + " IS ACTIVE");
    }
}
