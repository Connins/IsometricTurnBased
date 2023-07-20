using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class StatsUIController : MonoBehaviour
{
    [SerializeField] Canvas charecterStatsCanvas;

    private GameObject charecter;

    [SerializeField] TMP_Text health;
    [SerializeField] TMP_Text strength;
    [SerializeField] TMP_Text defence;
    [SerializeField] TMP_Text move;
    [SerializeField] TMP_Text jump;

    [SerializeField] private Button cancel;


    // Start is called before the first frame update
    void Start()
    {
        charecterStatsCanvas.enabled = false;
        cancel.onClick.AddListener(cancelOnClick);
    }

    // Update is called once per frame
    void Update()
    {
        if (charecter)
        {
            charecterStatsCanvas.enabled = true;
            health.text = "Health: " + charecter.GetComponentInChildren<CharecterStats>().Health;
            strength.text = "Strength: " + charecter.GetComponentInChildren<CharecterStats>().Strength;
            defence.text = "Defence: " + charecter.GetComponentInChildren<CharecterStats>().Defence;
            move.text = "Move: " + charecter.GetComponentInChildren<CharecterStats>().Move;
            jump.text = "Jump: " + charecter.GetComponentInChildren<CharecterStats>().Jump;
        }
        else
        {
            charecterStatsCanvas.enabled = false;
        }
    }

    private void cancelOnClick()
    {
        Charecter = null;
    }

    public GameObject Charecter
    {
        set { charecter = value; }
    }
}
