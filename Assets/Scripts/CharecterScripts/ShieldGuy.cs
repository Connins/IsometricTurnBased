using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ShieldGuy : CharecterStats
{
    private float shieldDamageAngle = 134.9f;
    private float shieldDamageModifier = 0.5f;

    public override void TakeHit(uint damage, float angle)
    {
        //could do calculation of how much damage would be taken based on stats but for now will keep it simple
        int damageAfterModifiers = (int)Mathf.Round(damage * angleDamageModifier(angle));
        if (!IsHost)
        {
            //Not the host should call this update as reconciliation can happen before local damage change occurs if we dont do it this way
            UpdateServerHealthServerRpc(damageAfterModifiers);
        }

        health -= damageAfterModifiers;

        GetComponent<CharecterUIController>().startDamageIndicatorCoroutine(-damageAfterModifiers);

        if (health <= 0)
        {
            die();
        }
        else
        {
            if (angle > shieldDamageAngle)
            {
                GetComponent<Animator>().Play("Blocking");
            }
            else
            {
                GetComponent<Animator>().Play("TakeHit");
            }
        }
    }

    private float angleDamageModifier(float angle)
    {
        float modifier = 1f;

        if (angle < backDamageAngle)
        {
            modifier = backDamageModifier;
        }

        if (angle > shieldDamageAngle)
        {
            modifier = shieldDamageModifier;
        }

        return modifier;
    }

}

