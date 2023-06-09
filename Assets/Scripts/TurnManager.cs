using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TurnManager : MonoBehaviour
{
    [SerializeField] private bool isPlayerTurn;
    [SerializeField] TMP_Text turnText;
    [SerializeField] GameObject mouseController;

    private List<GameObject> goodGuyList = new List<GameObject>();
    private List<GameObject> badGuyList = new List<GameObject>();
    private List<GameObject> activePlayerList = new List<GameObject>();
  
    // Start is called before the first frame update
    void Start()
    {
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

    public void removeCharecterFromList(GameObject charecter)
    {
        if (charecter.GetComponent<CharecterStats>().GoodGuy)
        {
            goodGuyList.Remove(charecter);
        }
        else
        {
            badGuyList.Remove(charecter);
        }
    }
    public void charecterDoneAction(GameObject charecter)
    {
        activePlayerList.Remove(charecter);
    }

    public bool activePlayer(GameObject charecter)
    {
        return activePlayerList.Contains(charecter);
    }

    public void switchSides()
    {
        isPlayerTurn = isPlayerTurn == false;
        
        if(isPlayerTurn)
        {
            activePlayerList = new List<GameObject>(goodGuyList);
            if(activePlayerList.Count > 0)
            {
                turnText.text = "Your turn";
            }
            else
            {
                switchSides();
            }
            
        }
        else
        {
            activePlayerList = new List<GameObject>(badGuyList);
            if (activePlayerList.Count > 0)
            {
                turnText.text = "Enemy turn";
            }
            else
            {
                switchSides();
            }
        }
    }
}
