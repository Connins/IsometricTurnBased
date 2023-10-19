using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerUIContoller : MonoBehaviour
{
    [SerializeField] private Button host;
    [SerializeField] private Button client;
    [SerializeField] private GameObject SceneManager;

    [SerializeField] TMP_InputField hostIP;
    //[SerializeField] private GameObject charecters;

    //private TurnManager turnManager;

    private UIManager UIManager;
    // Start is called before the first frame update
    void Start()
    {
        UIManager = GetComponentInParent<UIManager>();
        host.onClick.AddListener(hostOnClick);
        client.onClick.AddListener(clientOnClick);
        hostIP = gameObject.GetComponentInChildren<TMP_InputField>();
    }

    private void hostOnClick()
    {
        GlobalParameters.IsHost = true;
        GlobalParameters.MatchHappening = true;
        UIManager.SwitchUI("LevelSelect");
    }

    private void clientOnClick()
    {
        GlobalParameters.IsHost = false;
        GlobalParameters.YouAreGoodGuys = false;
        GlobalParameters.MatchHappening = true;
        GlobalParameters.HostIP = hostIP.text;
        UIManager.SwitchUI("LevelSelect");
    }
}
