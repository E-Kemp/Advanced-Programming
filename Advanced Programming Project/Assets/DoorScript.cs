using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : LevelController.LevelEntity
{
    public GameObject DOOR1, DOOR2, DOOR3;
    public BoxCollider2D COLLIDER;
    bool open = false;

    public override bool consoleTrigger(string[] args)
    {
        if(args[0] == "True" && !open)
        {
            CONSOLE.displayMessage("Open!");
            DOOR1.transform.Translate(-0.5f, 0, 0);
            DOOR2.transform.Translate(0.5f, 0, 0);
            COLLIDER.isTrigger = true;
            return open = true;
        }
        if(args[0] == "False" && open)
        {
            CONSOLE.displayMessage("Close!");
            DOOR1.transform.Translate(0.5f, 0, 0);
            DOOR2.transform.Translate(-0.5f, 0, 0);
            COLLIDER.isTrigger = false;
            return open = false;
        }
        return false;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        throw new System.NotImplementedException();
    }

    public override void setHookActive()
    {
        active = true;
        DOOR2.SetActive(true);
        DOOR3.SetActive(false);
    }
}
