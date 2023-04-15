using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TurnManager : MonoBehaviour
{
    [SerializeField] private bool isPlayerTurn;
    [SerializeField] TMP_Text turnText;


    private List<GameObject> goodGuyList = new List<GameObject>();
    private List<GameObject> badGuyList = new List<GameObject>();
    private List<GameObject> activePlayerList = new List<GameObject>();
    private Canvas UI;
  
    // Start is called before the first frame update
    void Start()
    {
        UI = FindAnyObjectByType<Canvas>();
        if (isPlayerTurn)
        {
            activePlayerList = new List<GameObject>(goodGuyList);
        }
        else
        {
            activePlayerList = new List<GameObject>(badGuyList);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(activePlayerList.Count == 0)
        {
            switchSides();
        }
    }

    public void addGoodGuy(GameObject goodGuy)
    { 
        goodGuyList.Add(goodGuy);
    }

    public void addBadGuy(GameObject badGuy)
    {
        badGuyList.Add(badGuy);
    }

    public void charecterMoved(GameObject charecter)
    {
        activePlayerList.Remove(charecter);
    }

    public bool activePlayer(GameObject charecter)
    {
        return activePlayerList.Contains(charecter);
    }

    private void switchSides()
    {
        isPlayerTurn = isPlayerTurn == false;
       
        if(isPlayerTurn)
        {
            activePlayerList = new List<GameObject>(goodGuyList);
            turnText.text = "Your turn";
        }
        else
        {
            activePlayerList = new List<GameObject>(badGuyList);
            turnText.text = "Enemy turn";
        }
    }
}
