using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public LevelEntity[] ENT_LIST;
    public ConsoleView CONSOLE;

    public abstract class LevelEntity : MonoBehaviour
    {
        public string NAME = ""; // Entity name
        public string HOOK = ""; // Hook name to use in the console
        public bool active = false;
        public ConsoleView CONSOLE;

        public abstract bool consoleTrigger(string[] args); // Console trigger

        protected abstract void OnTriggerEnter2D(Collider2D collision);

        public void setHookActive() { active = true; }
    }

    public LevelEntity getEntity(string hook)
    {
        foreach(LevelEntity ent in ENT_LIST)
        {
            if (ent.HOOK.Equals(hook) && ent.active)
                return ent;
        }
        CONSOLE.displayMessage("Entity doesn't exist/isn't active!");
        throw new System.Exception("Entity doesn't exist/isn't active!");
    }

}
