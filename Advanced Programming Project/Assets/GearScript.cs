using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity = LevelController.LevelEntity; // Aliasing Level Entities to make coding easier


public class GearScript : Entity
{
    public Entity TARGET_ENTITY;
    private void Update()
    {
        transform.Rotate(new Vector3(0, 0, 45) * Time.deltaTime);
    }

    public override bool consoleTrigger(string[] args)
    {
        CONSOLE.displayMessage("Unsupported method");
        return false;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        TARGET_ENTITY.setHookActive();
        CONSOLE.displayMessage(TARGET_ENTITY.name + " IS ACTIVE");
    }
}
