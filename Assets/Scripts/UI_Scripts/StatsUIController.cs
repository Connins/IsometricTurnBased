using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class StatsUIController : MonoBehaviour
{
    private GameObject charecter;

    [SerializeField] TMP_Text health;
    [SerializeField] TMP_Text strength;
    [SerializeField] TMP_Text defence;
    [SerializeField] TMP_Text move;
    [SerializeField] TMP_Text jump;

    // Start is called before the first frame update
    void Start()
    {
        transform.GetComponent<Canvas>().enabled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (charecter)
        {
            transform.GetComponent<Canvas>().enabled = true;
            health.text = "Health: " + charecter.GetComponentInChildren<CharecterStats>().Health + "/" + charecter.GetComponentInChildren<CharecterStats>().MaxHealth;
            strength.text = "Strength: " + charecter.GetComponentInChildren<CharecterStats>().Strength;
            defence.text = "Defence: " + charecter.GetComponentInChildren<CharecterStats>().Defence;
            move.text = "Move: " + charecter.GetComponentInChildren<CharecterStats>().Move;
            jump.text = "Jump: " + charecter.GetComponentInChildren<CharecterStats>().Jump;
        }
        else
        {
            transform.GetComponent<Canvas>().enabled = false;
        }
    }
    public GameObject Charecter
    {
        set { charecter = value; }
    }
}
