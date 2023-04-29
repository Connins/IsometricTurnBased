using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharecterStats : MonoBehaviour
{

    [SerializeField] private int health;
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
        healthBar.value = health;
    }

    // Update is called once per frame
    void Update()
    {

        //This is handling canvas update could put this in a seperate script.
        healthBar.value = health;
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

    public uint outpPutDamage()
    {
        //This is where we could make calculation on how much damage a charecter outputs keeping it simple for now
        return stength;
    }

    public void takeHit(uint damage)
    {
        //could do calculation of how much damage would be taken based on stats but for now will keep it simple
        health -= (int)damage;

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
        //play animation for death also make charecter unselectable.
        GetComponent<Animator>().SetTrigger("Death");
        turnManager.removeCharecterFromList(gameObject);
        mapManager.removeFromOccupied(transform.position);

    }
}

