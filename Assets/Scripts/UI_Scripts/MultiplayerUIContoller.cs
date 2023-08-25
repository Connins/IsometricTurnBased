using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerUIContoller : MonoBehaviour
{
    [SerializeField] private Button host;
    [SerializeField] private Button client;
    [SerializeField] private GameObject charecters;

    private TurnManager turnManager;

    private UIManager UIManager;
    // Start is called before the first frame update
    void Start()
    {
        UIManager = GetComponentInParent<UIManager>();
        host.onClick.AddListener(hostOnClick);
        client.onClick.AddListener(clientOnClick);
        turnManager = charecters.GetComponent<TurnManager>();
    }

    private void hostOnClick()
    {
        NetworkManager.Singleton.StartHost();
        UIManager.SwitchUI(2);
        turnManager.MatchHappening = true;
    }

    private void clientOnClick()
    {
        NetworkManager.Singleton.StartClient();
        turnManager.YouAreGoodGuys = false;
        UIManager.SwitchUI(2);
        turnManager.MatchHappening = true;
    }
}
