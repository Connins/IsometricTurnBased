using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{

    [SerializeField] private Button endTurn;
    [SerializeField] private GameObject Charecters;

    private TurnManager turnManager;
    // Start is called before the first frame update
    void Start()
    {
        endTurn.onClick.AddListener(endTurnOnClick);
        turnManager = Charecters.GetComponent<TurnManager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void endTurnOnClick()
    {
        turnManager.switchSides();
    }
}
