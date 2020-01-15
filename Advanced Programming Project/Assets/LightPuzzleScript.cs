using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Entity = LevelController.Entity; // Aliasing Level Entities to make coding easier


public class LightPuzzleScript : Entity
{
    public LightScript[] LIGHT_ARRAY;
    public GearLightScript[] GEAR_SCRIPTS;
    //private bool COMPLETE = false;

    private void Start()
    {
        foreach(var gear in GEAR_SCRIPTS)
        {
            gear.puzzleChecker += checkPuzzle;
        }
    }

    public void checkPuzzle()
    {
            foreach(var light in LIGHT_ARRAY)
            {
            if (!light.getState())
                return;
            }
            setHookActive();
            sendMessage("Puzzle complete!");
            hookIsActive();
    }



    public override bool trigger(string[] args)
    {
        return active;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        throw new System.NotImplementedException();
    }
}
