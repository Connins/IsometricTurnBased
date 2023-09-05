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
        GlobalParameters.MatchHappening = true;
        GlobalParameters.IsHost = true;
        UIManager.SwitchUI("LevelSelect");
    }

    private void onlineOnClick()
    {
        UIManager.SwitchUI("MultiplayerMenu");
    }
}
