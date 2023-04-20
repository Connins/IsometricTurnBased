using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] private uint range;
    [SerializeField] private uint weight;
    [SerializeField] private uint weaponDamage;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public uint Range
    {
        get { return range; }
    }

    public uint Weight
    {
        get { return weight; }
    }

    public uint WeaponDamage
    {
        get { return weaponDamage; }
    }
}
