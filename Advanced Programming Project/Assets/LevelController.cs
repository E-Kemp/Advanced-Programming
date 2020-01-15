using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interpreter;
using System.Text;
using Inter = Interpreter.Main;
using Lexer = Interpreter.Lexer;
using Token = Interpreter.Token;

public class LevelController : MonoBehaviour
{
    public Entity TARGET;
    public Entity[] ENT_LIST;
    public ConsoleView CONSOLE;
    private List<Entity> TRIG_LIST = new List<Entity>();

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
        public bool doorTrigger;
        //public ConsoleView CONSOLE;

        public abstract bool trigger(string[] args); // Console trigger
        protected abstract void OnTriggerEnter2D(Collider2D collision);
        public virtual void setHookActive() { active = true; }
        public virtual bool checkActive() { return active; }

        //Triggers the message event
        protected void sendMessage(string str) { message(str); }
        //Triggers the console message event
        protected void consoleMessage(string str) { console(str); }
        //Triggers the getTarget event
        protected Entity getTargetEnt() { return getTarget(); }
        //Check if all hooks are now active
        protected bool hookIsActive() { return isActive(); }
    }


    public void Start()
    {
        // Assign event methods
        CONSOLE.scriptEvent += script;

        TARGET.message += message;
        TARGET.getTarget += getTarget;
        TARGET.console += console;
        TARGET.isActive += isActive;
        foreach(var ent in ENT_LIST)
        {
            ent.message += message;
            ent.getTarget += getTarget;
            ent.console += console;
            ent.isActive += isActive;
        }

        foreach(var ent in ENT_LIST)
        {
            if (ent.doorTrigger)
                TRIG_LIST.Add(ent);
        }
    }

    //~LevelController()
    //{
    //    TARGET.message += message;
    //    TARGET.getTarget += getTarget;
    //    foreach (var ent in ENT_LIST)
    //    {
    //        ent.message += message;
    //        ent.getTarget += getTarget;
    //    }
    //}

    public Entity getEntity(string hook)
    {
        foreach(Entity ent in ENT_LIST)
        {
            if (ent.HOOK.Equals(hook) && ent.active)
                return ent;
        }
        CONSOLE.displayMessage("Entity doesn't exist/isn't active!");
        return null;
    }

    public bool checkTrigger(string hook)
    {
        foreach (Entity ent in ENT_LIST)
        {
            if (ent.HOOK.Equals(hook) && ent.doorTrigger && ent.checkActive())
                //CONSOLE.appendConsoleLogLine("True!");
                return true;
        }
        return false;
    }

    #region Code Handling

    //Tokenise using the lexer in F#
    public void script(string line)
    {
        #region Lexing

        StringBuilder str = new StringBuilder();
        var hookList = new List<string>{TARGET.HOOK};
        var trigList = new List<bool>{TARGET.active};
        foreach (Entity ent in ENT_LIST)
        {
            hookList.Add(ent.HOOK);
            trigList.Add(ent.active);
        }

        var hooks = Inter.interpretStr(line, hookList, trigList);
        var tokenList = new List<Token>();
        // Add tokens to a C# list manually because there are type conflicts
        foreach (Token t in hooks)
        {
            tokenList.Add(t);
            str.Append(Lexer.printToken(t));
        }

        //CONSOLE.displayMessage(str.ToString());
        CONSOLE.appendConsoleLogLine(str.ToString());

        #endregion

        #region Parsing and executing

        //FIX THIS BIT

        if (tokenList[tokenList.Count-1].IsOP) // Multiple hooks and one operation
        {
            var entList = new List<Entity>();
            for(int t = 0; t < tokenList.Count-1; t++)
            {
                var ent = getEntity(Parser.getHook(tokenList[t]));
                if (ent == null)
                    return;
                else
                    entList.Add(ent);
            }
            bool op = Parser.getOP(tokenList[tokenList.Count - 1]);

            // If the hooks are triggers
            int trigCount = 0;
            foreach(var ent in entList)
            {
                if (TRIG_LIST.Contains(ent) && ent.checkActive())
                    trigCount++;
            }
            if(trigCount == TRIG_LIST.Count)
            {
                TARGET.trigger(new string[] { op.ToString() });
                return;
            }

            // If they're not triggers
            foreach(var ent in entList)
            {
                ent.trigger(new string[] { });
            }

        }

        #endregion
    }

    #endregion


    public void message(string str)
    {
        CONSOLE.displayMessage(str);
    }

    public void console(string str)
    {
        CONSOLE.appendConsoleLogLine(str);
    }

    public Entity getTarget()
    {
        return TARGET;
    }

    public bool isActive()
    {
        foreach (Entity ent in TRIG_LIST)
            if (!ent.checkActive())

                return false;
        TARGET.setHookActive();
        return true;
    }
}
