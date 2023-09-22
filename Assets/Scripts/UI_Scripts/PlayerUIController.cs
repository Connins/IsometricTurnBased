using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{

    private Button endTurn;
    private Button wait;
    private Button attack;
    private Button capture;

    private ButtonManager buttonManager = new ButtonManager();

    [SerializeField] TMP_Text turnText;

    [SerializeField] private GameObject Charecters;
    private TurnManager turnManager;

    [SerializeField] private GameObject MouseController;
    private MouseController mouseController;

    private void Awake()
    {
        buttonManager.InitializeCanvasButtons(transform.gameObject);
        endTurn = buttonManager.GetButton("EndTurn");
        wait = buttonManager.GetButton("Wait");
        attack = buttonManager.GetButton("Attack");
        capture = buttonManager.GetButton("Capture");

        endTurn.onClick.AddListener(endTurnOnClick);
        wait.onClick.AddListener(waitOnClick);
        wait.interactable = false;
        attack.onClick.AddListener(attackOnClick);
        attack.interactable = false;
        capture.onClick.AddListener(captureOnClick);
        capture.interactable = false;

        turnManager = Charecters.GetComponent<TurnManager>();
        mouseController = MouseController.GetComponent<MouseController>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void endTurnOnClick()
    {
        turnManager.endingTurn();
        mouseController.playerHasBeenDeselected();    
    }

    private void waitOnClick()
    {
        mouseController.setInWaitMode(true);
    }

    private void attackOnClick()
    {
        mouseController.setInAttackMode(true);
    }

    private void captureOnClick()
    {
        mouseController.setInCaptureMode(true);
    }

    public void enableButton(bool enable, string button)
    {
        buttonManager.GetButton(button).interactable = enable;
    }

    public void EnablePlayerUI(bool enable)
    {
        endTurn.interactable = enable;  
    }

    public void PlayerTurnText(bool yourTurn)
    {
        if (turnManager.LocalPlay)
        {
            if (yourTurn)
            {
                turnText.text = "Player 1 Turn";
            }
            else
            {
                turnText.text = "Player 2 Turn";
            }

        }
        else
        {
            if (yourTurn)
            {
                turnText.text = "Your turn";
            }
            else
            {
                turnText.text = "Enemy turn";
            }
        }
    }
    public void DisablePlayerUI()
    {
        endTurn.interactable = false;
        attack.interactable = false;
        wait.interactable = false;
        capture.interactable = false;
    }

    public void DefaultPlayerUI()
    {
        attack.interactable = false;
        wait.interactable = false;
        capture.interactable = false;
    }
}
