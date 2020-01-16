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


        //Add the hooks needed to open the door to a separate list
        foreach(var ent in ENT_LIST)
        {
            if (ent.doorTrigger)
                TRIG_LIST.Add(ent);
        }
    }

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

        if (tokenList[tokenList.Count - 1].IsOP) // Multiple hooks and one operation
        {
            var entList = new List<Entity>();
            for (int t = 0; t + 1 < tokenList.Count; t += 2)
            {
                //Syntax checking
                if (!(tokenList[t + 1].IsEX || tokenList[t + 1].IsOP))
                {
                    CONSOLE.appendConsoleLogLine(string.Format(
                            "SYNTAX ERROR: {0} can only be proceeded by an expression " +
                            "(i.e. AND) or an operator (i.e. +/-).",
                            Lexer.printToken(tokenList[t])));
                    return;
                }
                var ent = getEntity(Parser.getVA(tokenList[t]));
                if (ent == null)
                {
                    CONSOLE.appendConsoleLogLine(string.Format(
                            "SYNTAX ERROR: {0} does not exist.",
                            Lexer.printToken(tokenList[t])));
                    return;
                }
                    
                else
                    entList.Add(ent);
            }
            bool op = Parser.getOP(tokenList[tokenList.Count - 1]);

            // If the hooks are triggers
            int trigCount = 0;
            foreach (var ent in entList)
            {
                if (TRIG_LIST.Contains(ent) && ent.checkActive())
                    trigCount++;
            }
            if (trigCount == TRIG_LIST.Count)
            {
                TARGET.trigger(new string[] { op.ToString() });
                return;
            }

            // If they're not triggers
            foreach (var ent in entList)
            {
                if(ent.active)
                    ent.trigger(new string[] { op.ToString() });
                else
                    CONSOLE.appendConsoleLogLine(string.Format(
                         "SYNTAX ERROR: {0} is not active/doesn't exist, try one of the gears!", ent.HOOK));

            }

        }
        else
        {
            CONSOLE.appendConsoleLogLine(string.Format(
                         "SYNTAX ERROR: Script must be finished with an operator " +
                         "(i.e. +/-)."));
            return;
        }

        #endregion
    }

    #endregion

    #region Event methods

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

    #endregion
}
