using UnityEngine;


class UISystem : MonoBehaviour
{
    public static UISystem uiSystem;

    public HUDComponent hud;


    private void Start()
    {
        if (uiSystem != null) Destroy(uiSystem);
        else uiSystem = this;
        DontDestroyOnLoad(this);
    }


    /**************************
     *          HUD           *
     **************************/
    public float GetCurrentHealth()
    {
        return hud.GetHealth();
    }

    public void UpdateHealth(float amount)
    {
        hud.ModifyHealth(amount);
    }


    public float GetCurrentStamina()
    {
        return hud.GetStamina();
    }

    public void UpdateStamina(float amount)
    {
        hud.ModifyStamina(amount);
    }


    public float GetCurrentAmmo()
    {
        return hud.GetAmmo();
    }

    public void UpdateAmmo(float amount)
    {
        hud.ModifyAmmo(amount);
    }


    public void EnableBossHealthDisplay(bool setting)
    {
        hud.EnableBossHealthDisplay(setting);
    }

    public float GetCurrentBossHealth()
    {
        return hud.GetBossHealth();
    }

    public void UpdateBossHealth(float amount)
    {
        hud.ModifyBossHealth(amount);
    }
}
