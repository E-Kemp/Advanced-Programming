using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

// Door script to control the door to the next level
public class DoorScript : Entity
{
    public GameObject DOOR1, DOOR2, DOOR3;
    public BoxCollider2D COLLIDER;
    public CircleCollider2D TRIG_COLLIDER;
    protected bool open = false;
    public int NEXT_SCENE_INDEX;

    public override bool trigger(string[] args)
    {
        if(args[0] == "True" && !open && active)
        {
            sendMessage("Open!");
            DOOR1.transform.Translate(-0.5f, 0, 0);
            DOOR2.transform.Translate(0.5f, 0, 0);
            COLLIDER.isTrigger = true;
            return open = true;
        }
        if(args[0] == "False" && open && active)
        {
            sendMessage("Close!");
            DOOR1.transform.Translate(0.5f, 0, 0);
            DOOR2.transform.Translate(-0.5f, 0, 0);
            COLLIDER.isTrigger = false;
            return open = false;
        }
        return false;
    }

    protected override void OnTriggerEnter2D(Collider2D collision)
    {
        SceneManager.LoadScene(NEXT_SCENE_INDEX); //Will only work in full build, watch a tutorial for this later
    }

    public override void setHookActive()
    {
        active = true;
        DOOR2.SetActive(true);
        DOOR3.SetActive(false);
    }
}
