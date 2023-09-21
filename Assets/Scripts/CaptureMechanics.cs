using Unity.Netcode;
using UnityEditor.Build;
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

    private void Awake()
    {
        canvas = GetComponentInChildren<Canvas>();
        captureHealthBar = GetComponentInChildren<Slider>();
        captureHealthBar.maxValue = maxCaptureHealth;
        highlight = GetComponent<Highlight>();
        GameObject charecters = GameObject.FindGameObjectWithTag("CharectersGameObject");
        turnManager = charecters.GetComponent<TurnManager>();
    }
    public override void OnNetworkSpawn()
    {
        captureHealth = captureHealthServerState.Value;
        captureHealthBar.value = captureHealth;
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

    public void beingCaptured(int damage, bool goodGuy)
    {
        if (IsServer)
        {
            captureHealthServerState.Value -= damage;
        }
        captureHealth -= damage;

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

        turnManager.CheckCapturePoints();
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
