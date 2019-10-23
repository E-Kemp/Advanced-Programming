﻿/// 
/// ConsoleController Class to manage the console inputs and execute tasks respectively.
/// With thanks to Eliot Lash for tutorial and original code (heavily edited, this class was very broken at the start)
/// 
using UnityEngine;
using System;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    public event MessageChangedHandler messageChanged;
    #endregion


    /// Object to hold information about each command
    /// command - actual command entered in console
    /// handler - method name in code
    /// help - description of the command (for the help menu)
    class CommandRegistration
    {
        public string command { get; private set; }
        public CommandHandler handler { get; private set; }
        public string help { get; private set; }

        public CommandRegistration(string command, CommandHandler handler, string help)
        {
            this.command = command;
            this.handler = handler;
            this.help = help;
        }
    }

    /// 
    /// How many log lines should be retained?
    /// Note that strings submitted to appendLogLine with embedded newlines will be counted as a single line.
    /// 
    const int scrollbackSize = 20;

    Queue<string> scrollback = new Queue<string>(scrollbackSize);
    Stack<string> commandHistory = new Stack<string>();
    Dictionary<string, CommandRegistration> commands = new Dictionary<string, CommandRegistration>();

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
    }


    void registerCommand(string command, CommandHandler handler, string help)
    {
        commands.Add(command, new CommandRegistration(command, handler, help));
    }


    public void appendLogLine(string line)
    {
        Debug.Log(line);

        if (scrollback.Count >= ConsoleController.scrollbackSize)
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
            if (reg.handler == null)
            {
                appendLogLine(string.Format("Unable to process command '{0}', handler was null.", command));
            }
            else
            {
                reg.handler(args);
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
            messageChanged(output);
        }
        else appendLogLine("No arguments to echo!");
    }
    
    public void help(string[] args)
    {
        if (args.Length == 1)
        {
            CommandRegistration reg = commands[args[0]];
            if (reg.handler != null)
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

    #endregion
}