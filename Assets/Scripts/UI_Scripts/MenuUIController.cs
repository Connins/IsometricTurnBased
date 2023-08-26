using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private Button localPlay;
    [SerializeField] private Button online;
    
    [SerializeField] private GameObject Charecters;
    private TurnManager turnManager;
    
    private UIManager UIManager;
    // Start is called before the first frame update
    void Start()
    {
        UIManager = GetComponentInParent<UIManager>();
        localPlay.onClick.AddListener(localPlayOnClick);
        online.onClick.AddListener(onlineOnClick);
        turnManager = Charecters.GetComponent<TurnManager>();
    }

    private void localPlayOnClick()
    {
        turnManager.IsLocalPlay();
        NetworkManager.Singleton.StartHost();
        UIManager.SwitchUI("PlayerUI");
        turnManager.MatchHappening = true;
    }

    private void onlineOnClick()
    {
        UIManager.SwitchUI("MultiplayerMenu");
    }
}
