using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Gear script that can trigger a hook elsewhere
public class GearScript : Entity
{
    public GameObject GEAR;
    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, 45) * Time.deltaTime);
    }

    public override bool trigger(string[] args)
    {
        if (active) // If the gear is active
        {
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
        hookIsActive();
    }
}
