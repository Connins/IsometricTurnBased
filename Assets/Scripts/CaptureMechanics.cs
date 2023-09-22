using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class CaptureMechanics : NetworkBehaviour
{
    [SerializeField] NetworkVariable<int> captureHealthServerState;

    private bool isCaptured;
    private bool capturedByGoodGuy;
    private int captureHealth;
    private Canvas canvas;
    private Slider captureHealthBar;
    private int maxCaptureHealth = 20;
    private Highlight highlight;
    private TurnManager turnManager;
    private MapManager mapManager;

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        captureHealthBar = GetComponentInChildren<Slider>();
        captureHealthBar.maxValue = maxCaptureHealth;
        highlight = GetComponent<Highlight>();
        GameObject charecters = GameObject.FindGameObjectWithTag("CharectersGameObject");
        turnManager = charecters.GetComponent<TurnManager>();
        GameObject grid = GameObject.FindGameObjectWithTag("Grid");
        mapManager = grid.GetComponent<MapManager>();
    }
    public override void OnNetworkSpawn()
    {
        captureHealth = captureHealthServerState.Value;
        captureHealthBar.value = captureHealth;
    }

    private void OnEnable()
    {
        captureHealthServerState.OnValueChanged += OnCaptureHealthServerStateChanged;
    }
    private void OnCaptureHealthServerStateChanged(int previousValue, int newValue)
    {
        if (captureHealth != captureHealthServerState.Value)
        {
            Debug.Log("captureHealth does not match the network variable captureHealth doing very basic reconciliation");
            captureHealth = captureHealthServerState.Value;
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        isCaptured = false;
    }

    // Update is called once per frame
    void Update()
    {
        captureHealthBar.value = captureHealth;
        canvas.transform.SetPositionAndRotation(canvas.transform.position, Camera.main.transform.rotation);
    }

    public void ResetCapturePoints()
    {
        captureHealth = maxCaptureHealth;

        if (IsServer)
        {
            captureHealthServerState.Value = maxCaptureHealth;
        }
        else
        {
            UpdateServerCapturePointsServerRpc(maxCaptureHealth);
        }
    }
    public void beingCaptured(int damage, bool goodGuy)
    {
        captureHealth -= damage;

        if (!IsHost)
        {
            UpdateServerCapturePointsServerRpc(captureHealth);
        }

        GetComponent<DamageIndicatorController>().startDamageIndicatorCoroutine(-damage);

        if (captureHealth <= 0)
        {
            Captured(goodGuy);
        }
        
    }

    private void Captured(bool goodGuy)
    {
        if(!isCaptured)
        {
            isCaptured=true;
        }
        if(goodGuy)
        {
            highlight.changeOriginalColour("yellow");
            capturedByGoodGuy = true;
        }
        else
        {
            highlight.changeOriginalColour("red");
            capturedByGoodGuy = false;
        }
        ResetCapturePoints();
        turnManager.CheckCapturePoints();
    }

    [ServerRpc(RequireOwnership = false)]
    private void UpdateServerCapturePointsServerRpc(int value)
    {
        captureHealthServerState.Value = value;
    }

    //accessor functions
    public bool IsCaptured
    {
        get { return isCaptured; }
    }

    public bool CapturedByGoodGuy
    {
        get { return capturedByGoodGuy; } 
    } 
}
