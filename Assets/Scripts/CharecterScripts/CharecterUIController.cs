using System.Collections;
using TMPro;
using UnityEngine;

public class CharecterUIController : MonoBehaviour
{
    private DamageIndicatorController damageTextController;
    [SerializeField] private GameObject directionHighlight;
    [SerializeField] private GameObject actionHighlight;

    void Start()
    {
        damageTextController = GetComponent<DamageIndicatorController>();
    }

    public void startDamageIndicatorCoroutine(int value)
    {
        damageTextController.startDamageIndicatorCoroutine(value);
    }

    public void setDirectionHighlight(bool value)
    {
        directionHighlight.GetComponent<MeshRenderer>().enabled = value;
    }

    public void setActionHighlight(bool value)
    {
        actionHighlight.GetComponent<MeshRenderer>().enabled = value;
    }
}
