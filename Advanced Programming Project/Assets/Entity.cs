using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interpreter;
using System.Text;

// Abstract entity class that can involve anything a player could interact with
public abstract class Entity : MonoBehaviour
{
    public delegate void MessageHandler(string str);
    public event MessageHandler message;

    public delegate void ConsoleHandler(string str);
    public event ConsoleHandler console;

    public delegate Entity GetTargetHandler();
    public event GetTargetHandler getTarget;

    public delegate bool IsActiveListenerHandler();
    public event IsActiveListenerHandler isActive;

    public string NAME = ""; // Entity name
    public string HOOK = ""; // Hook name to use in the console
    public bool active = false;
    public bool doorTrigger; // Whether this entity is needed to open the door

    // Console trigger
    public abstract bool trigger(string[] args);
    // Inherited from the collision 
    protected abstract void OnTriggerEnter2D(Collider2D collision);
    // Sets the hooks to active (i.e. when a gear is collided with)
    public virtual void setHookActive() { active = true; }
    // Checks whether the entity's hook is active
    public virtual bool checkActive() { return active; }


    //%% Methods to call the events &&//

    //Triggers the message event
    protected void sendMessage(string str) { message(str); }
    //Triggers the console message event
    protected void consoleMessage(string str) { console(str); }
    //Triggers the getTarget event
    protected Entity getTargetEnt() { return getTarget(); }
    //Check if all hooks are now active
    protected bool hookIsActive() { return isActive(); }
}
