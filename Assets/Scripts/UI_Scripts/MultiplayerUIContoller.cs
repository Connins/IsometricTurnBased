using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MultiplayerUIContoller : MonoBehaviour
{
    [SerializeField] private Button host;
    [SerializeField] private Button client;
    [SerializeField] private GameObject SceneManager;

    //[SerializeField] private GameObject charecters;

    //private TurnManager turnManager;

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
        GlobalParameters.IsHost = true;
        GlobalParameters.MatchHappening = true;
        SceneManager.GetComponent<SceneLoader>().LoadScene("TestLevel");
    }

    private void clientOnClick()
    {
        GlobalParameters.IsHost = false;
        GlobalParameters.YouAreGoodGuys = false;
        GlobalParameters.MatchHappening = true;
        SceneManager.GetComponent<SceneLoader>().LoadScene("TestLevel");
    }
}
