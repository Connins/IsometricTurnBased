using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private Button localPlay;
    [SerializeField] private Button online;
    [SerializeField] private GameObject SceneManager;


    private UIManager UIManager;
    // Start is called before the first frame update
    void Start()
    {
        UIManager = GetComponentInParent<UIManager>();
        localPlay.onClick.AddListener(localPlayOnClick);
        online.onClick.AddListener(onlineOnClick);
    }

    private void localPlayOnClick()
    {
        GlobalParameters.IsLocalPlay = true;
        GlobalParameters.IsHost = true;
        GlobalParameters.MatchHappening = true;
        SceneManager.GetComponent<SceneLoader>().LoadScene("TestLevel");
    }

    private void onlineOnClick()
    {
        UIManager.SwitchUI("MultiplayerMenu");
    }
}
