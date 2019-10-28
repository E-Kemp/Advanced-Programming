using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorScript : LevelController.LevelEntity
{

    public override bool consoleTrigger(string[] args)
    {
        CONSOLE.displayMessage("DOOR!");
        return true;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        throw new System.NotImplementedException();
    }
}
