using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public class TurnManager : NetworkBehaviour
{
    [SerializeField] private bool isPlayerTurn;
    [SerializeField] GameObject mouseController;
    [SerializeField] NetworkVariable<bool> turnVariable;
    [SerializeField] GameObject UIManager;

    private List<GameObject> goodGuyList = new List<GameObject>();
    private List<GameObject> badGuyList = new List<GameObject>();
    private List<GameObject> activePlayerList = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {
        if (isPlayerTurn)
        {
            activePlayerList = new List<GameObject>(goodGuyList);
        }
        else
        {
            activePlayerList = new List<GameObject>(badGuyList);
        }

    }

    public override void OnNetworkSpawn()
    {
        UIManager.GetComponent<UIManager>().EnablePlayerUI(YourTurn());
    }

    private void OnEnable()
    {
        turnVariable.OnValueChanged += OnTurnVariableStateChanged;
    }
    private void OnTurnVariableStateChanged(bool previousValue, bool newValue)
    {
        UIManager.GetComponent<UIManager>().EnablePlayerUI(YourTurn());
    }
    // Update is called once per frame
    void Update()
    {
        if (activePlayerList.Count == 0)
        {
            SwitchSides();
        }
    }

    public void addGoodGuy(GameObject goodGuy)
    {
        goodGuyList.Add(goodGuy);
    }

    public void addBadGuy(GameObject badGuy)
    {
        badGuyList.Add(badGuy);
    }

    public void removeCharecterFromList(GameObject charecter)
    {
        if (charecter.GetComponent<CharecterStats>().GoodGuy)
        {
            goodGuyList.Remove(charecter);
        }
        else
        {
            badGuyList.Remove(charecter);
        }
    }
    public void charecterDoneAction(GameObject charecter)
    {
        activePlayerList.Remove(charecter);
    }

    public bool activePlayer(GameObject charecter)
    {
        return activePlayerList.Contains(charecter);
    }

    public bool YourTurn()
    {
        if (IsServer)
        {
            return turnVariable.Value;
        }
        else
        {
            return !turnVariable.Value;
        }
    }

    public void NetworkSwitchSides()
    {
        if (IsServer)
        {
            SwitchSidesClientRpc();
        }
        else
        {
            SwitchSidesServerRpc();
            SwitchSides();
        }
    }

    [ClientRpc]
    public void SwitchSidesClientRpc() { SwitchSides(); }

    [ServerRpc(RequireOwnership = false)]
    public void SwitchSidesServerRpc() { SwitchSides(); }
    public void SwitchSides()
    {

        isPlayerTurn = isPlayerTurn == false;
        if (IsServer)
        {
            turnVariable.Value = turnVariable.Value == false;
        }
        if (isPlayerTurn)
        {
            activePlayerList = new List<GameObject>(goodGuyList);
            if(activePlayerList.Count > 0)
            {

            }
            else
            {
                SwitchSides();
            }
            
        }
        else
        {
            activePlayerList = new List<GameObject>(badGuyList);
            if (activePlayerList.Count > 0)
            {

            }
            else
            {
                SwitchSides();
            }
        }
    }
}
