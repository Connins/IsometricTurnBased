using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuUIController : MonoBehaviour
{
    [SerializeField] private Button localPlay;
    [SerializeField] private Button online;

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
        UIManager.SwitchUI(3);
    }

    private void onlineOnClick()
    {
        UIManager.SwitchUI(2);
    }
}
