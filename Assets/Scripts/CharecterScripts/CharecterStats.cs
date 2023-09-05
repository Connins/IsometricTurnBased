using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using Unity.Networking.Transport;
using UnityEngine;
using UnityEngine.UI;

public class CharecterStats : NetworkBehaviour
{

    [SerializeField] NetworkVariable<int> healthServerState;
    private int health;
    [SerializeField] private uint maxHealth;
    [SerializeField] private uint move;
    [SerializeField] private uint jump;
    [SerializeField] private uint strength;
    [SerializeField] private uint defense;
    [SerializeField] private uint speed;
    [SerializeField] private uint magic;
    [SerializeField] private uint magicDefense;
    [SerializeField] private bool goodGuy;
    [SerializeField] private float attackAnimationTime;

    private TurnManager turnManager;
    private MapManager mapManager;

    private Canvas canvas;
    public Slider healthBar;

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
            Debug.Log("Health network variable changes are detected before actual health change has occured" +
                " This is stopping any reconciliation we need ticking system to avoid this");
            //Debug.Log("Health does not match the network variable health doing very basic reconciliation");
            //health = healthServerState.Value;
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
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas = GetComponentInChildren<Canvas>();
        healthBar = GetComponentInChildren<Slider>();
        healthBar.maxValue = maxHealth;
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

    //this only works if server is also a client as ServerRPC command just calls wanted function
    //it does not call a clientRPC command
    public void NetworkTakeHit(uint damage)
    {
        if(IsServer)
        {
            TakeHitClientRPC(damage);
            healthServerState.Value -= (int)damage;
        }
        else
        {
            TakeHit(damage);
            TakeHitServerRPC(damage);
        }
    }

    [ServerRpc(RequireOwnership = false)]
    public void TakeHitServerRPC(uint damage)
    {
        TakeHit(damage);
        healthServerState.Value -= (int)damage;
    }
    
    [ClientRpc]
    public void TakeHitClientRPC(uint damage)
    {
        TakeHit(damage);
    }

    public void TakeHit(uint damage)
    {
        //could do calculation of how much damage would be taken based on stats but for now will keep it simple
        if (IsServer)
        {
            healthServerState.Value -= (int)damage;
        }
        health -= (int)damage;
        GetComponent<CharecterUIController>().startDamageIndicatorCoroutine(-(int)damage);

        if (health <= 0)
        {
            die();
        }
        else
        {
            GetComponent<Animator>().Play("TakeHit");
        }
    }

    private void die()
    {
        GetComponent<Animator>().SetTrigger("Death");
        GetComponent<CapsuleCollider>().enabled = false;
        mapManager.removeFromOccupied(transform.position);
        turnManager.removeCharecterFromList(gameObject);
        healthBar.gameObject.SetActive(false);
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
}

