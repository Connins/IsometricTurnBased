using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class CharecterStats : NetworkBehaviour
{

    [SerializeField] NetworkVariable<int> healthServerState;
    [SerializeField] private uint maxHealth;
    [SerializeField] private uint move;
    [SerializeField] private uint jump;
    [SerializeField] private uint stength;
    [SerializeField] private uint defense;
    [SerializeField] private uint speed;
    [SerializeField] private uint magic;
    [SerializeField] private uint magicDefense;
    [SerializeField] private bool goodGuy;

    private TurnManager turnManager;
    private MapManager mapManager;

    private Canvas canvas;
    public Slider healthBar;

    private void OnEnable()
    {
        healthServerState.OnValueChanged += OnHealthServerStateChanged;
    }

    private void OnHealthServerStateChanged(int previousValue, int newValue)
    {
        int damage = newValue - previousValue;
        GetComponent<CharecterUIController>().startDamageIndicatorCoroutine((int)damage);

        if (healthServerState.Value <= 0)
        {
            die();
        }
        else
        {
            GetComponent<Animator>().Play("TakeHit");
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
        healthBar.value = healthServerState.Value;
    }

    // Update is called once per frame
    void Update()
    {

        //This is handling canvas update could put this in a seperate script.
        healthBar.value = healthServerState.Value;
        canvas.transform.SetPositionAndRotation(canvas.transform.position, Camera.main.transform.rotation);
    }

    //accessor functions
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

    public uint outPutDamage()
    {
        //This is where we could make calculation on how much damage a charecter outputs keeping it simple for now
        return stength;
    }

    [ServerRpc(RequireOwnership = false)]
    public void takeHitServerRPC(uint damage)
    {
        //could do calculation of how much damage would be taken based on stats but for now will keep it simple
        healthServerState.Value -= (int)damage;
    }

    private void die()
    {
        GetComponent<Animator>().SetTrigger("Death");
        mapManager.removeFromOccupied(transform.position);
        turnManager.removeCharecterFromList(gameObject);
        healthBar.gameObject.SetActive(false);
    }
}

