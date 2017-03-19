using UnityEngine;
using UnityEngine.UI;


class HUDComponent : MonoBehaviour
{
    public Slider healthSlider;
    public Slider staminaSlider;
    public Slider ammoSlider;

    public Slider bossHealthSlider;
    public GameObject bossHealthContainer;

    private float staminaRechargeRate = 0.25f;
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
     *         Health         *
     **************************/
    public void ModifyHealth(float value)
    {
        healthSlider.value += value;

        if (GetHealth() > healthSlider.maxValue)
        {
            healthSlider.value = healthSlider.maxValue;
        }
        else if (GetHealth() < healthSlider.minValue)
        {
            healthSlider.value = healthSlider.minValue;
        }
    }

    public float GetHealth()
    {
        return healthSlider.value;
    }


    /**************************
     *         Stamina        *
     **************************/
    public void ModifyStamina(float value)
    {
        if (value < 0)
        {
            staminaRechargeWait = true;
            staminaRechargeCounter = 0;
        }

        staminaSlider.value += value;

        if (GetStamina() > staminaSlider.maxValue)
        {
            staminaSlider.value = staminaSlider.maxValue;
        }
        else if (GetStamina() <= staminaSlider.minValue)
        {
            staminaSlider.value = staminaSlider.minValue;
        }
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
        return ammoSlider.value;
    }


    /**************************
     *       BossHealth       *
     **************************/
    public void ModifyBossHealth(float value)
    {
        bossHealthSlider.value += value;

        if (GetBossHealth() > bossHealthSlider.maxValue)
        {
            bossHealthSlider.value = bossHealthSlider.maxValue;
        }
        else if (GetBossHealth() < bossHealthSlider.minValue)
        {
            bossHealthSlider.value = bossHealthSlider.minValue;
        }
    }

    public float GetBossHealth()
    {
        return bossHealthSlider.value;
    }

    public void EnableBossHealthDisplay(bool setting)
    {
        bossHealthContainer.SetActive(setting);
    }
}
