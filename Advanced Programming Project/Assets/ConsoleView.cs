/// 
/// ConsoleView Class to control the view aspects fo the console.
/// With thanks to Eliot Lash for tutorial and original code (heavily edited)
/// 
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Entity = LevelController.LevelEntity; // Aliasing Level Entities to make coding easier

public class ConsoleView : MonoBehaviour
{
    ConsoleController CONSOLE = new ConsoleController(); // Object of the custom controller class!!

    bool DID_SHOW = false;

    public GameObject VIEW_CONTAINER; //Container for console view, should be a child of this GameObject
    public Text LOG_TEXT_AREA, MESSAGE_BOX;
    public InputField INPUT_FIELD;
    public LevelController LEVEL_CONTROLLER;

    private Color PINK = new Color(0.8666667f, 0.1568628f, 1, 1);

    void Start()
    {
        if (CONSOLE != null)
        {
            CONSOLE.visibilityChanged += onVisibilityChanged;
            CONSOLE.logChanged += onLogChanged;
            CONSOLE.screenMessage += displayMessage;
            CONSOLE.entityChanged += entityChanged;
            CONSOLE.hookChecker += getHookList;
        }
        updateLogStr(CONSOLE.log);
        setVisibility(false);
    }

    ~ConsoleView()
    {
        CONSOLE.visibilityChanged += onVisibilityChanged;
        CONSOLE.logChanged += onLogChanged;
        CONSOLE.screenMessage += displayMessage;
        CONSOLE.entityChanged += entityChanged;
        CONSOLE.hookChecker += getHookList;
    }

    void Update()
    {
        //Toggle visibility when tilde key pressed
        if (Input.GetKeyUp("`"))
        {
            toggleVisibility();
        }

        //Toggle visibility when 5 fingers touch.
        if (Input.touches.Length == 5)
        {
            if (!DID_SHOW)
            {
                toggleVisibility();
                DID_SHOW = true;
            }
        }
        else
        {
            DID_SHOW = false;
        }
    }

    void toggleVisibility()
    {
        setVisibility(!VIEW_CONTAINER.activeSelf);
    }

    void setVisibility(bool visible)
    {
        VIEW_CONTAINER.SetActive(visible);
    }

    void onVisibilityChanged(bool visible)
    {
        setVisibility(visible);
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

        for (float i = 1f; i >= 0f; i -= 0.05f)
        {
            MESSAGE_BOX.color = new Color(PINK.r, PINK.g, PINK.b, i);
            yield return new WaitForFixedUpdate();
        }
    }


    public void entityChanged(string hook, string[] args)
    {
        Entity ent = LEVEL_CONTROLLER.getEntity(hook);
        ent.consoleTrigger(args);
    }


    /// 
    /// Event that should be called by anything wanting to submit the current input to the console.
    /// 
    public void runCommand()
    {
        CONSOLE.runCommandString(INPUT_FIELD.text);
        INPUT_FIELD.text = "";
    }

    public List<Entity> getHookList()
    {
        List<Entity> hookList = new List<Entity>();
        hookList.AddRange(LEVEL_CONTROLLER.ENT_LIST);
        return hookList;
    }
}
