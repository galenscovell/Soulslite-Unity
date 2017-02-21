using UnityEngine;
using UnityEngine.UI;


class HUDComponent : MonoBehaviour
{
    public Slider staminaSlider;
    public Slider ammoSlider;

    private float staminaRechargeRate = 0.05f;
    private bool staminaRechargeWait = false;
    private float staminaRechargeWaitTime = 4;
    private float staminaRechargeCounter = 0;


    private void Start()
    {
        
    }

    private void Update()
    {
        // Handle stamina
        if (staminaRechargeWait)
        {
            if (staminaRechargeCounter > staminaRechargeWaitTime)
            {
                staminaRechargeWait = false;
                staminaRechargeCounter = 0;
            }
            else
            {
                staminaRechargeCounter += Time.deltaTime;
            }
        }
        else
        {
            ModifyStamina(staminaRechargeRate);
        }
    }


    /**************************
     *         Stamina        *
     **************************/
    public void ModifyStamina(float value)
    {
        staminaSlider.value += value;

        if (GetStamina() > staminaSlider.maxValue)
        {
            staminaSlider.value = staminaSlider.maxValue;
        }
        else if(GetStamina() < staminaSlider.minValue)
        {
            staminaSlider.value = staminaSlider.minValue;
        }
    }

    public void RechargeStamina()
    {
        staminaRechargeWait = true;
    }

    public float GetStamina()
    {
        return staminaSlider.value;
    }


    /**************************
     *          Ammo          *
     **************************/
    public void ModifyAmmo(float value)
    {
        ammoSlider.value += value;

        if (GetAmmo() > ammoSlider.maxValue)
        {
            ammoSlider.value = ammoSlider.maxValue;
        }
        else if (GetAmmo() < ammoSlider.minValue)
        {
            ammoSlider.value = ammoSlider.minValue;
        }
    }

    public float GetAmmo()
    {
        return staminaSlider.value;
    }
}
