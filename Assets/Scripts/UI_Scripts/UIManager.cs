using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas menuUI;
    [SerializeField] private Canvas networkUI;
    [SerializeField] private Canvas playerUI;

    private List<Canvas> UIList = new List<Canvas>();
    private Canvas currentUI;
    // Start is called before the first frame update
    void Start()
    {
        currentUI = menuUI;
        UIList.Add(menuUI);
        UIList.Add(networkUI);
        UIList.Add(playerUI);
        SwitchUI(0);
    }

    public void SwitchUI(int UIIndex)
    {
        currentUI.enabled = false;
        currentUI = UIList[UIIndex];
        currentUI.enabled = true;
    }

    public void ShowCurrentUI(bool show)
    {
        currentUI.enabled = show;
    }

    public void ShowUI(bool show, int UIIndex)
    {
        UIList[UIIndex].enabled = show;
    }

    public void EnablePlayerUI(bool enable)
    {
        playerUI.GetComponent<PlayerUIController>().EnablePlayerUI(enable);
    }

}
