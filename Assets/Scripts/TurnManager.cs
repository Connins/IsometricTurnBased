using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
public class TurnManager : NetworkBehaviour
{
    [SerializeField] private bool isHostTurn;
    [SerializeField] GameObject mouseController;
    [SerializeField] NetworkVariable<bool> turnVariable;
    [SerializeField] GameObject UIManager;
    private MapManager mapManager;

    private List<GameObject> goodGuyList = new List<GameObject>();
    private List<GameObject> badGuyList = new List<GameObject>();
    private List<GameObject> activePlayerList = new List<GameObject>();

    private bool localPlay;
    private bool youAreGoodGuys;
    private bool matchHappening;
    // Start is called before the first frame update
    void Start()
    {
        localPlay = GlobalParameters.IsLocalPlay;
        YouAreGoodGuys = GlobalParameters.YouAreGoodGuys;
        setupNetwork();
        matchHappening = GlobalParameters.MatchHappening;
        GameObject grid = GameObject.FindGameObjectWithTag("Grid");
        mapManager = grid.GetComponent<MapManager>();
    }

    public override void OnNetworkSpawn()
    {
        isHostTurn = turnVariable.Value;
        UIManager.GetComponent<UIManager>().EnablePlayerUI(YourTurn());
        UIManager.GetComponentInChildren<PlayerUIController>().PlayerTurnText(YourTurn());
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
        UIManager.GetComponentInChildren<PlayerUIController>().PlayerTurnText(YourTurnLocal());
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

        if(goodGuyList.Count == 0) 
        {
            endGame(false);
        }
        if(badGuyList.Count == 0)
        {
            endGame(true);
        }
    }

    public void CheckCapturePoints()
    {
        List<GameObject> capturePoints = mapManager.CapturePoints;
        int goodGuysCapturedPoints = 0;
        int badGuysCapturedPoints = 0;
        int numberOfCapturePoints = capturePoints.Count;

        foreach (GameObject capturePoint in capturePoints)
        {
            CaptureMechanics captureMechanics = capturePoint.GetComponent<CaptureMechanics>();
            if (captureMechanics.IsCaptured) 
            {
                if (captureMechanics.CapturedByGoodGuy)
                {
                    goodGuysCapturedPoints++;
                }
                else
                {
                    badGuysCapturedPoints++;
                }
            }
        }
        
        if (goodGuysCapturedPoints > numberOfCapturePoints / 2)
        {
            endGame(true);
        }
        if (badGuysCapturedPoints > numberOfCapturePoints / 2)
        {
            endGame(false);
        }
    }

    private void endGame(bool goodGuysWon)
    {
        mouseController.GetComponent<MouseController>().killStatsUI();
        matchHappening = false;
        UIManager.GetComponentInChildren<ResultsUIController>().EndGameText(goodGuysWon);
        UIManager.GetComponent<UIManager>().SwitchUI("ResultsCanvas");
    }
    public void charecterDoneAction(GameObject charecter)
    {
        activePlayerList.Remove(charecter);
        charecter.GetComponent<CharecterUIController>().setActionHighlight(false);

        if(mapManager.isCharecterOnCapturePoint(charecter))
        {
            charecter.GetComponent<CharecterStats>().OnCapturePoint = mapManager.getTile(charecter);
        }
        else
        {
            if (charecter.GetComponent<CharecterStats>().OnCapturePoint)
            {
                charecter.GetComponent<CharecterStats>().OnCapturePoint.GetComponent<CaptureMechanics>().ResetCapturePoints();
                charecter.GetComponent<CharecterStats>().OnCapturePoint = null;
            }
        }

        if (activePlayerList.Count == 0)
        {
            endingTurn();
        }
    }

    public void endingTurn()
    {
        LocalChangeTurnVariable();
        NetworkChangeTurnVariable();
        if (LocalPlay)
        {
            YouAreGoodGuys = YouAreGoodGuys == false;
        }
    }
    public bool activePlayer(GameObject charecter)
    {
        return activePlayerList.Contains(charecter);
    } 
    public void RefreshActiveCharecters()
    {
        foreach (GameObject charecter in activePlayerList)
        {
            charecter.GetComponent<CharecterUIController>().setActionHighlight(false);
        }
        if (isHostTurn)
        {
            activePlayerList = new List<GameObject>(goodGuyList);
        }
        else
        {
            activePlayerList = new List<GameObject>(badGuyList);
        }

        if (YourTurn() || localPlay)
        {
            foreach (GameObject charecter in activePlayerList)
            {
                charecter.GetComponent<CharecterUIController>().setActionHighlight(true);
            }
        }
        
    }

    private void setupNetwork()
    {
        if (GlobalParameters.IsHost)
        {
            NetworkManager.Singleton.StartHost();
        }
        else
        {
            NetworkManager.Singleton.StartClient();
        }
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
    public bool LocalPlay
    {
        get
        {
            return localPlay;
        }
    }

    public bool YouAreGoodGuys
    {
        get { return youAreGoodGuys; }
        set { youAreGoodGuys = value; }
    }

    public bool MatchHappening
    {
        get { return matchHappening; }
        set { matchHappening = value; }
    }
}
