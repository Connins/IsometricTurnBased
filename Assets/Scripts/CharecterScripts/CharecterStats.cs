using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharecterStats : NetworkBehaviour
{

    [SerializeField] NetworkVariable<int> healthServerState;
    protected int health;
    [SerializeField] private uint maxHealth;
    [SerializeField] private uint move;
    [SerializeField] private uint jump;
    [SerializeField] private uint strength;
    [SerializeField] private uint defense;
    [SerializeField] private uint speed;
    [SerializeField] private uint magic;
    [SerializeField] private uint magicDefense;
    [SerializeField] private float attackAnimationTime;

    [SerializeField] private bool goodGuy;

    private TurnManager turnManager;
    private MapManager mapManager;

    private Canvas canvas;
    public Slider healthBar;

    private GameObject onCapturePoint;

    protected float backDamageAngle = 44.9f;
    protected float backDamageModifier = 1.5f;

    public override void OnNetworkSpawn()
    {
        health = healthServerState.Value;
        healthBar.value = health;
    }
    private void OnEnable()
    {
        healthServerState.OnValueChanged += OnHealthServerStateChanged;
    }
    private void OnHealthServerStateChanged(int previousValue, int newValue)
    {
        if (IsServer)
        {
            return;
        }
        
        if(health != healthServerState.Value)
        {
            Debug.Log("Health does not match the network variable health doing very basic reconciliation");
            health = healthServerState.Value;
        }
    }

    private void Awake()
    {
        turnManager = GetComponentInParent<TurnManager>();
        mapManager = FindAnyObjectByType<MapManager>();

        if (goodGuy)
        {
            turnManager.addGoodGuy(gameObject);
        }
        else
        {
            turnManager.addBadGuy(gameObject);
        }
        canvas = GetComponentInChildren<Canvas>();
        healthBar = GetComponentInChildren<Slider>();
        healthBar.maxValue = maxHealth;
        onCapturePoint = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        healthBar.value = health;
         //This is handling canvas update could put this in a seperate script.
        canvas.transform.SetPositionAndRotation(canvas.transform.position, Camera.main.transform.rotation);
    }

    public uint outPutDamage()
    {
        //This is where we could make calculation on how much damage a charecter outputs keeping it simple for now
        return strength;
    }
    public virtual void TakeHit(uint damage, float angle)
    {
        //could do calculation of how much damage would be taken based on stats but for now will keep it simple
        int damageAfterModifiers = (int)Mathf.Round(damage * angleDamageModifier(angle));
        if (!IsHost)
        {
            //Not the host should call this update as reconciliation can happen before local damage change occurs if we dont do it this way
            UpdateServerHealthServerRpc(damageAfterModifiers);
        }

        health -= damageAfterModifiers;

        GetComponent<CharecterUIController>().startDamageIndicatorCoroutine(-damageAfterModifiers);

        if (health <= 0)
        {
            die();
        }
        else
        {
            GetComponent<Animator>().Play("TakeHit");
        }
    }

    private  float angleDamageModifier(float angle)
    {
        float modifier = 1f;

        if(angle < backDamageAngle)
        {
            modifier = backDamageModifier;
        }

        return modifier;
    }
    protected void die()
    {
        GetComponent<Animator>().SetTrigger("Death");
        GetComponent<CapsuleCollider>().enabled = false;
        mapManager.removeFromOccupied(gameObject);
        turnManager.removeCharecterFromList(gameObject);
        healthBar.gameObject.SetActive(false);
    }

    [ServerRpc(RequireOwnership = false)]
    protected void UpdateServerHealthServerRpc(int damage)
    {
        healthServerState.Value -= damage;
    }

    ////////////////////////////////////////////////////////////////////////////////////////////
    //Here is the accessor functions for stats

    public int Health
    {
        get { return health; }
    }

    public uint MaxHealth
    {
        get { return maxHealth; }
    }

    public uint Strength
    {
        get { return strength; }
    }

    public uint Defence
    {
        get { return defense; }
    }
    public uint Move
    {
        get { return move; }
    }
    public uint Jump
    {
        get { return jump; }
    }

    public bool GoodGuy
    {
        get { return goodGuy; }
    }

    public float AttackAnimationTime
    {
        get { return attackAnimationTime; }
    }

    public GameObject OnCapturePoint
    {
        get { return onCapturePoint; }
        set { onCapturePoint = value; }
    }
}

