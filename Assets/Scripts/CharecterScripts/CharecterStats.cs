using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharecterStats : MonoBehaviour
{

    [SerializeField] private uint health;
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
    private Canvas canvas;
    public Slider healthBar;

    private void Awake()
    {
        turnManager = GetComponentInParent<TurnManager>();

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
        canvas.transform.transform.Rotate(0, 180, 0);
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


    public void TakeHit(uint damage)
    {
        //could do calcula tion of how much damage would be done based on stats but for now will keepo it simple
        health -= damage;
        if(health < 0)
        {
            Die();
        }

    }

    private void Die()
    {
        //play animation for death also make charecter unselectable.
        Debug.Log("play animation for death also make charecter unselectable");
    }
}

