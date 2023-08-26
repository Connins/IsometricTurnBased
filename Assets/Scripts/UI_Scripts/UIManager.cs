using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas menuUI;
    [SerializeField] private Canvas networkUI;
    [SerializeField] private Canvas playerUI;
    [SerializeField] private Canvas resultsUI;

    private List<Canvas> UIList = new List<Canvas>();
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
        }

        currentUI = menuUI;
        
        SwitchUI("Menu");
    }

    private string ExtractIdentifier(GameObject canvasObject)
    {
        return canvasObject.name; 
    }

    public void SwitchUI(string identifier)
    {
        currentUI.enabled = false;
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
        playerUI.GetComponent<PlayerUIController>().EnablePlayerUI(enable);
    }

}
