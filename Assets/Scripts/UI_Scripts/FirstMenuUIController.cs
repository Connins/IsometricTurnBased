using UnityEngine;
using UnityEngine.UI;

public class FirstMenuUIController : MonoBehaviour
{
    [SerializeField] private Button startGame;
    [SerializeField] private GameObject SceneManager;

    private UIManager UIManager;

    // Start is called before the first frame update
    void Start()
    {
        UIManager = GetComponentInParent<UIManager>();

        startGame.onClick.AddListener(startGameOnClick);
        
    }
    private void startGameOnClick()
    {
        UIManager.SwitchUI("Menu");
    }
}
