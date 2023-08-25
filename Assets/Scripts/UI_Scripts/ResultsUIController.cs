using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResultsUIController : MonoBehaviour
{
    [SerializeField] TMP_Text resultsText;
    [SerializeField] private GameObject Charecters;
    private TurnManager turnManager;
    void Start()
    {
        turnManager = Charecters.GetComponent<TurnManager>();
    }

    public void EndGameText(bool goodGuysWon)
    {
        if (turnManager.LocalPlay)
        {
            if (goodGuysWon)
            {
                resultsText.text = "Player 1 Won!!!";
            }
            else
            {
                resultsText.text = "Player 2 Won!!!";
            }

        }
        else
        {
            if (goodGuysWon == turnManager.YouAreGoodGuys)
            {
                resultsText.text = "You Won!!!";
            }
            else
            {
                resultsText.text = "You Lost!!!";
            }
        }
    }
}
