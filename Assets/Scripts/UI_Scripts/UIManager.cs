using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Canvas menuUI;
    [SerializeField] private Canvas networkUI;
    [SerializeField] private Canvas playerUI;

    // Start is called before the first frame update
    void Start()
    {
        SwitchUI(1);
    }

    public void SwitchUI(int uiIndex)
    {
        menuUI.enabled = (uiIndex == 1);

        networkUI.enabled = (uiIndex == 2);

        playerUI.enabled = (uiIndex == 3);
    }

}
