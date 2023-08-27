using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    
    public Dictionary<string, GameObject> UIDictionary = new Dictionary<string, GameObject>();
    private Canvas currentUI;
    // Start is called before the first frame update
    void Start()
    {
        GameObject[] UICanvases = GameObject.FindGameObjectsWithTag("UICanvas");

        foreach (GameObject canvasObject in UICanvases)
        {
            string identifier = ExtractIdentifier(canvasObject);
            UIDictionary.Add(identifier, canvasObject);
            if (canvasObject.GetComponent<Canvas>().enabled)
            {
                currentUI = canvasObject.GetComponent<Canvas>();
            }
        }        
    }

    private string ExtractIdentifier(GameObject canvasObject)
    {
        return canvasObject.name; 
    }

    public void SwitchUI(string identifier)
    {
        if(currentUI !=  null)
        {
            currentUI.enabled = false;
        }
        currentUI = UIDictionary[identifier].GetComponent<Canvas>();
        currentUI.enabled = true;
    }
    public void ShowCurrentUI(bool show)
    {
        currentUI.enabled = show;
    }

    public void ShowUI(bool show, string identifier)
    {
        UIDictionary[identifier].GetComponent<Canvas>().enabled = show;
    }

    public void EnablePlayerUI(bool enable)
    {
        UIDictionary["PlayerUI"].GetComponent<Canvas>().GetComponent<PlayerUIController>().EnablePlayerUI(enable);
    }

}
