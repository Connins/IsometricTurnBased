using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;


public class CaptureMechanics : NetworkBehaviour
{

    [SerializeField] NetworkVariable<int> captureHealthServerState;
    private int captureHealth;

    private Canvas canvas;
    private Slider captureHealthBar;
    private int maxCaptureHealth = 20;

    public override void OnNetworkSpawn()
    {
        captureHealth = captureHealthServerState.Value;
        captureHealthBar.value = captureHealth;
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        captureHealthBar = GetComponentInChildren<Slider>();
        captureHealthBar.maxValue = maxCaptureHealth;
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

        GetComponent<CharecterUIController>().startDamageIndicatorCoroutine(-damage);

        if (captureHealth <= 0)
        {
            Captured(goodGuy);
        }
        
    }

    private void Captured(bool goodGuy)
    {
        if(goodGuy)
        {
            //swap material to yellow
        }
        else
        {
            //swap material to red
        }
    }
}
