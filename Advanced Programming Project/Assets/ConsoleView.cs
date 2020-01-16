using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

// ConsoleView Class to control the view aspects fo the console.
// With thanks to Eliot Lash for tutorial and initial code (heavily edited)
public class ConsoleView : MonoBehaviour
{
    public delegate void ScriptHandler(string str);
    public event ScriptHandler scriptEvent;

    ConsoleController CONTROLLER = new ConsoleController(); // Object of the custom controller class!!

    bool SHOW = false;

    public GameObject VIEW_CONTAINER; //Container for console view, should be a child of this GameObject
    public Text LOG_TEXT_AREA, MESSAGE_BOX;
    public InputField INPUT_FIELD;
    public LevelController LEVEL_CONTROLLER;

    private Color PINK = new Color(0.8666667f, 0.1568628f, 1, 1);

    void Start()
    {
        if (CONTROLLER != null)
        {
            CONTROLLER.logChanged += onLogChanged;
            CONTROLLER.screenMessage += displayMessage;
            CONTROLLER.script += script;
        }
        updateLogStr(CONTROLLER.log);
        VIEW_CONTAINER.SetActive(false);
    }


    void Update()
    {
        //Toggle visibility when tilde key pressed
        if (Input.GetKeyUp("`"))
        {
            VIEW_CONTAINER.SetActive(!VIEW_CONTAINER.activeSelf);
            if (VIEW_CONTAINER.activeSelf)
                keepActive();
            else
            {
                INPUT_FIELD.DeactivateInputField();
            }
                
        }
    }

    void onLogChanged(string[] newLog)
    {
        updateLogStr(newLog);
    }

    void updateLogStr(string[] newLog)
    {
        if (newLog == null)
        {
            LOG_TEXT_AREA.text = "";
        }
        else
        {
            LOG_TEXT_AREA.text = string.Join("\n", newLog);
        }
    }

    public void displayMessage(string message)
    {
        StartCoroutine(messageTransition(message)); // Start method that returns an IEnumerator (allows for wait times)
    }

    IEnumerator messageTransition(string message)
    {
        MESSAGE_BOX.text = message;
        MESSAGE_BOX.color = PINK;

        yield return new WaitForSeconds(5); // Wait 5 seconds

        for (float i = 1f; i >= -0.05f; i -= 0.05f)
        {
            MESSAGE_BOX.color = new Color(PINK.r, PINK.g, PINK.b, i);
            yield return new WaitForFixedUpdate();
        }
    }


    public void appendConsoleLogLine(string message)
    {
        CONTROLLER.appendLogLine(message);
    }


    /// 
    /// Event that should be called by anything wanting to submit the current input to the console.
    /// 
    public void runCommand()
    {
        CONTROLLER.runCommandString(INPUT_FIELD.text);
        INPUT_FIELD.text = "";
        keepActive();
    }

    public void keepActive()
    {
        INPUT_FIELD.ActivateInputField();
        INPUT_FIELD.text = "";
        INPUT_FIELD.Select();
    }

    public void script(string str)
    {
        scriptEvent(str);
    }

}
