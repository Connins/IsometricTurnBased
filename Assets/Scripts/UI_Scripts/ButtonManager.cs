using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonManager
{
    private Dictionary<string, Button> buttonDictionary = new Dictionary<string, Button>();

    public void InitializeCanvasButtons(GameObject gameObjectWithCanvas)
    {
        Button[] buttons = gameObjectWithCanvas.GetComponentsInChildren<Button>();

        foreach (Button button in buttons)
        {
            buttonDictionary.Add(button.name, button);
        }
    }

    public Button GetButton(string buttonName)
    {
        if (buttonDictionary.TryGetValue(buttonName, out Button button))
        {
            return button;
        }
        else
        {
            Debug.LogError("Button with name '" + buttonName + "' not found.");
            return null;
        }
    }
}