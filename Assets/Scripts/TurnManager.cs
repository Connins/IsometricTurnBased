using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    [SerializeField] private bool isPlayerTurn;
    private List<GameObject> goodGuyList = new List<GameObject>();
    private List<GameObject> badGuyList = new List<GameObject>();
    private List<GameObject> activePlayerList = new List<GameObject>();
    
    // Start is called before the first frame update
    void Start()
    {
        if (isPlayerTurn)
        {
            print(activePlayerList.Count);
            activePlayerList = new List<GameObject>(goodGuyList);
            print(activePlayerList.Count);
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
        print(goodGuyList.Count);
        activePlayerList.Remove(charecter);
        print(goodGuyList.Count);
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
        }
        else
        {
            activePlayerList = new List<GameObject>(badGuyList);
        }
    }
}
