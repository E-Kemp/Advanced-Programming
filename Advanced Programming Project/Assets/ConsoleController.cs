/// 
/// ConsoleController Class to manage the console inputs and execute tasks respectively.
/// With thanks to Eliot Lash for tutorial and original code (heavily edited, this class was very broken at the start)
/// 
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using System.Text;
using Inter = Interpreter.Main;
using Lexer = Interpreter.Lexer;
using Token = Interpreter.Token;
using Entity = LevelController.LevelEntity; // Aliasing Level Entities to make coding easier


public delegate void CommandHandler(string[] args);


public class ConsoleController
{

    #region Event declarations
    // Used to communicate with ConsoleView
    public delegate void LogChangedHandler(string[] log);
    public event LogChangedHandler logChanged;

    public delegate void VisibilityChangedHandler(bool visible);
    public event VisibilityChangedHandler visibilityChanged;

    public delegate void MessageChangedHandler(string message);
    public event MessageChangedHandler screenMessage;

    public delegate void EntityInteractionHandler(string hook, string[] args);
    public event EntityInteractionHandler entityChanged;

    public delegate List<Entity> HookChecker();
    public event HookChecker hookChecker;
    #endregion


    /// Nested class to hold information about each command
    /// command - actual command entered in console
    /// handler - method name in code
    /// help - description of the command (for the help menu)
    class CommandRegistration
    {
        public string command { get; private set; }
        public CommandHandler Handler { get; private set; }
        public string help { get; private set; }

        public CommandRegistration(string command, CommandHandler handler, string help)
        {
            this.command = command;
            this.Handler = handler;
            this.help = help;
        }
    }



    const int scrollbackSize = 20;

    Queue<string> scrollback = new Queue<string>(scrollbackSize);
    Stack<string> commandHistory = new Stack<string>();
    Dictionary<string, CommandRegistration> commands = new Dictionary<string, CommandRegistration>();

    // List of hooks
    public string[] log { get; private set; }

    const string repeatCmdName = "!!";

    public ConsoleController()
    {
        registerCommand("babble", babble, "Example command that demonstrates how to parse arguments. babble [word] [# of times to repeat]");
        registerCommand("echo", echo, "Echoes arguments back as array (for testing argument parser)");
        registerCommand("help", help, "Print this help.");
        registerCommand(repeatCmdName, repeatCommand, "Repeat last command.");
        registerCommand("reload", reload, "Reload game.");
        registerCommand("resetprefs", resetPrefs, "Reset & saves PlayerPrefs.");
        registerCommand("history", history, "Prints the entire history of this session into the console.");
        registerCommand("object", obj, "Interact with an object!");


        registerCommand("test", test, "For testing purposes");
    }


    void registerCommand(string command, CommandHandler handler, string help)
    {
        commands.Add(command, new CommandRegistration(command, handler, help));
    }


    public void appendLogLine(string line)
    {
        Debug.Log(line);

        if (scrollback.Count >= scrollbackSize)
        {
            scrollback.Dequeue();
        }
        scrollback.Enqueue(line);

        log = scrollback.ToArray();
        if (logChanged != null)
        {
            logChanged(log);
        }
    }
    
    public void displayMessage(string msg)
    {
        screenMessage(msg);
    }


    public void runCommandString(string commandString)
    {
        appendLogLine("$ " + commandString);

        string[] commandSplit = parseArguments(commandString);

        if (commandSplit.Length < 2) 
        {
            runCommand(commandString.ToLower(), null);
            commandHistory.Push(commandString);
        }  
        else if (commandSplit.Length >= 2) 
        {
            int numArgs = commandSplit.Length - 1;
            string[] args = new string[numArgs];
            Array.Copy(commandSplit, 1, args, 0, numArgs);

            runCommand(commandSplit[0].ToLower(), args);
            commandHistory.Push(commandString);
        }
    }


    public void runCommand(string command, string[] args)
    {
        CommandRegistration reg = null;
        if (!commands.ContainsKey(command))
        {
            appendLogLine(string.Format("Unknown command '{0}', type 'help' for list.", command));
        }
        else
        {
            reg = commands[command];
            if (reg.Handler == null)
            {
                appendLogLine(string.Format("Unable to process command '{0}', handler was null.", command));
            }
            else
            {
                reg.Handler(args);
            }
        }
    }

    static string[] parseArguments(string commandString) 
    {
        return commandString.Split(' ');
    }


    #region Command handlers


    public void babble(string[] args)
    {
        appendLogLine("Run a command with 'command argument1, argument2, ...'");
    }

    public void echo(string[] args)
    {
        if (args != null)
        {
            string output = "'" + args[0];
            for(int i = 1; i < args.Length; i++)
            {
                output += " " + args[i];
            }
            output += "'";
            appendLogLine(output);
            screenMessage(output);
        }
        else appendLogLine("No arguments to echo!");
    }
    
    public void help(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            foreach(CommandRegistration com in commands.Values)
            {
                appendLogLine(com.command + ": " + com.help);
            }
        }
        if (args.Length == 1)
        {
            CommandRegistration reg = commands[args[0]];
            if (reg.Handler != null)
            {
                appendLogLine(reg.help);
            }
        }
        else appendLogLine("Invalid arguments!");
    }
    
    public void repeatCommand(string[] args) 
    {
        runCommandString(commandHistory.Peek());
    }


    public void reload(string[] args)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }


    public void resetPrefs(string[] args)
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
    }


    public void history(string[] args)
    {
        Stack<string> output = commandHistory;
        foreach(string str in output)
        {
            appendLogLine(str);
        }
    }


    /// Object specific command handlers (temporary until interpreter is done)
    public void obj(string[] args)
    {
        entityChanged(args[0], null);
    }

    public void test(string[] args)
    {
        StringBuilder str = new StringBuilder();

        //foreach(Token token in fsList)
        //    str.Append(Lexer.printToken(token));

        var hookList = new List<string>();
        var trigList = new List<bool>();
        foreach (Entity ent in hookChecker())
        {
            hookList.Add(ent.HOOK);
            trigList.Add(ent.active);
        }


        var hooks = Inter.interpretStr(string.Join(" ", args), hookList, trigList);
        
        var tokenList = new List<Token>();
        
        str.Append("TEST\n");
        foreach (Token t in hooks)
        {
            tokenList.Add(t);
            str.Append(Lexer.printToken(t));
        }

        appendLogLine(str.ToString());

        execute(tokenList);

    }

    #endregion



    public void execute(List<Token> tokenList)
    {
        if (tokenList[0].IsVA && tokenList[1].IsOP)
        {
            var val = Interpreter.Parser.getHook(tokenList[0]);
            var op = Interpreter.Parser.getOP(tokenList[1]);

            //appendLogLine(val + op.ToString());

            entityChanged(val, op.ToString().Split(' '));

        }

    }

}