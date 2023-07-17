using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;
public class TurnManager : NetworkBehaviour
{
    [SerializeField] private bool isHostTurn;
    [SerializeField] GameObject mouseController;
    [SerializeField] NetworkVariable<bool> turnVariable;
    [SerializeField] GameObject UIManager;

    private List<GameObject> goodGuyList = new List<GameObject>();
    private List<GameObject> badGuyList = new List<GameObject>();
    private List<GameObject> activePlayerList = new List<GameObject>();


    // Start is called before the first frame update
    void Start()
    {

    }

    public override void OnNetworkSpawn()
    {
        isHostTurn = turnVariable.Value;
        UIManager.GetComponent<UIManager>().EnablePlayerUI(YourTurn());
        RefreshActiveCharecters();
    }

    private void OnEnable()
    {
        turnVariable.OnValueChanged += OnTurnVariableStateChanged;
    }
    private void OnTurnVariableStateChanged(bool previousValue, bool newValue)
    {
        isHostTurn = newValue;
        UIManager.GetComponent<UIManager>().EnablePlayerUI(YourTurnLocal());
        RefreshActiveCharecters();
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void NetworkChangeTurnVariable()
    {
        if(IsServer)
        {
            turnVariable.Value = turnVariable.Value == false;
        }
        else
        {
            SwapSidesServerRpc();
        }
    }

    public void LocalChangeTurnVariable()
    {
        isHostTurn = isHostTurn == false;
    }

    [ServerRpc(RequireOwnership = false)]
    private void SwapSidesServerRpc()
    {
        turnVariable.Value = turnVariable.Value == false;
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
        if (activePlayerList.Count == 0)
        {
            LocalChangeTurnVariable();
            NetworkChangeTurnVariable();
        }
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

    public bool YourTurnLocal()
    {
        if (IsServer)
        {
            return isHostTurn;
        }
        else
        {
            return !isHostTurn;
        }
    }
    public void RefreshActiveCharecters()
    {
        if (isHostTurn)
        {
            activePlayerList = new List<GameObject>(goodGuyList);
        }
        else
        {
            activePlayerList = new List<GameObject>(badGuyList);
        }
    }
}
