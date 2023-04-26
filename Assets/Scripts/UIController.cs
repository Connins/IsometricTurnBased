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
    [SerializeField] private Button attack;

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
        attack.onClick.AddListener(attackOnClick);
        attack.interactable = false;

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
        mouseController.playerHasBeenDeselected();    
    }

    private void waitOnClick()
    {
       mouseController.playerHasOfficialyMoved();
    }

    private void attackOnClick()
    {
        //mouseController.playerHasOfficialyMoved();
    }

    public void disableWait()
    {
        wait.interactable = false;
    }

    public void enableWait()
    {
        wait.interactable = true;
    }
    public void disableAttack()
    {
        attack.interactable = false;
    }

    public void enableAttack()
    {
        attack.interactable = true;
    }

}
