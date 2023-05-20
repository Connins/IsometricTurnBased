using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerUIContoller : MonoBehaviour
{
    [SerializeField] private Button host;
    [SerializeField] private Button client;

    private UIManager UIManager;
    // Start is called before the first frame update
    void Start()
    {
        UIManager = GetComponentInParent<UIManager>();
        host.onClick.AddListener(hostOnClick);
        client.onClick.AddListener(clientOnClick);
    }

    private void hostOnClick()
    {
        NetworkManager.Singleton.StartHost();
        UIManager.SwitchUI(3);
    }

    private void clientOnClick()
    {
        NetworkManager.Singleton.StartClient();
        UIManager.SwitchUI(3);
    }
}
