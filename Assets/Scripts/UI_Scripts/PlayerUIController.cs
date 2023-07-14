using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{

    [SerializeField] private Button endTurn;
    [SerializeField] private Button wait;
    [SerializeField] private Button attack;

    [SerializeField] TMP_Text turnText;

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
    private void endTurnOnClick()
    {
        turnManager.NetworkSwitchSides();
        mouseController.playerHasBeenDeselected();    
    }

    private void waitOnClick()
    {
        mouseController.setInWaitMode(true);
    }

    private void attackOnClick()
    {
        mouseController.setInAttackMode(true);
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

    public void disableEndTurn()
    {
        endTurn.interactable = false;
    }

    public void enableEndTurn()
    {
        endTurn.interactable = true;
    }

    public void EnablePlayerUI(bool enable)
    {
        endTurn.interactable = enable;
        if (enable)
        {
            turnText.text = "Your turn";
        }
        else
        {
            turnText.text = "Enemy turn";
        }
    }

    public void DisablePlayerUI()
    {
        endTurn.interactable = false;
        attack.interactable = false;
        wait.interactable = false;
    }
}
