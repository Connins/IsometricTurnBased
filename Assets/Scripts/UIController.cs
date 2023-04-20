using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    [SerializeField] private Button endTurn;
    [SerializeField] private Button wait;
    
    [SerializeField] private GameObject Charecters;
    private TurnManager turnManager;

    [SerializeField] private GameObject MouseController;
    private MouseController mouseController;

    // Start is called before the first frame update
    void Start()
    {
        endTurn.onClick.AddListener(endTurnOnClick);
        wait.onClick.AddListener(waitOnClick);
        wait.interactable = false;
        turnManager = Charecters.GetComponent<TurnManager>();
        mouseController = MouseController.GetComponent<MouseController>();
    }

    

    // Update is called once per frame
    void Update()
    {
        
    }

    private void endTurnOnClick()
    {
        turnManager.switchSides();
    }

    private void waitOnClick()
    {
       mouseController.playerHasOfficialyMoved();
    }

    public void disableWait()
    {
        //wait.enabled = false;
        wait.interactable = false;
    }

    public void enableWait()
    {
        //print("This is currently not working");
        //wait.enabled = true;
        wait.interactable = true;
    }
}
