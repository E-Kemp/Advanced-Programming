using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;





// ConsoleController Class to manage the console inputs and execute tasks respectively.
// With thanks to Eliot Lash for tutorial and original code (heavily edited, this class was very broken at the start)
public class ConsoleController
{

    #region Event declarations
    // Used to communicate with ConsoleView
    public delegate void LogChangedHandler(string[] log);
    public event LogChangedHandler logChanged;

    public delegate void MessageChangedHandler(string message);
    public event MessageChangedHandler screenMessage;

    public delegate void ScriptHandler(string text);
    public event ScriptHandler script;
    #endregion

    // Delegate used for the command handling
    public delegate void CommandHandler(string[] args);
    
    
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


    // Max size of the command history
    const int scrollbackSize = 20;

    Queue<string> scrollback = new Queue<string>(scrollbackSize);
    Stack<string> commandHistory = new Stack<string>();
    Dictionary<string, CommandRegistration> commands = new Dictionary<string, CommandRegistration>();

    public string[] log { get; private set; }

    public ConsoleController()
    {
        appendLogLine("Welcome to the game! Type 'help' for a list of commands and other help.");
        registerCommand("babble", babble, "Example command that demonstrates how to parse arguments. babble [word] [# of times to repeat]");
        registerCommand("help", help, "Print this help.");
        registerCommand("reload", reload, "Reload game.");
    }


    private void registerCommand(string command, CommandHandler handler, string help)
    {
        commands.Add(command, new CommandRegistration(command, handler, help));
    }
    
    // Add a new line to the console log
    public void appendLogLine(string line)
    {
        Debug.Log(line);

        if (scrollback.Count >= scrollbackSize)
        {
            scrollback.Dequeue();
        }
        scrollback.Enqueue(line);

        log = scrollback.ToArray();

        logChanged?.Invoke(log);
    }
    
    // Display a message in-game
    public void displayMessage(string msg)
    {
        screenMessage(msg);
    }

    // Run a command entered into the console
    public void runCommandString(string commandString)
    {
        appendLogLine("$ " + commandString);

        string[] commandSplit = parseArguments(commandString);

        if(commands.ContainsKey(commandSplit[0]))
        {
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
        else
        {
            script(commandString);
        }

        
    }


    public void runCommand(string command, string[] args)
    {
        CommandRegistration reg = null;
        if (!commands.ContainsKey(command))
        {
            appendLogLine(string.Format("Unknown command '{0}', type 'help' for help.", command));
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
    
    public void help(string[] args)
    {
        if (args == null || args.Length == 0)
        {
            foreach(CommandRegistration com in commands.Values)
            {
                appendLogLine(com.command + ": " + com.help);
                
            }
            appendLogLine("To run a script, type the hook (i.e. GEAR) followed by an operator or expression (i.e. +/- or AND)");
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

    public void reload(string[] args)
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    #endregion

}