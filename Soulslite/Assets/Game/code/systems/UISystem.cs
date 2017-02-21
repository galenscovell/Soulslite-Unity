using System.Collections;
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
    public float GetCurrentStamina()
    {
        return hud.GetStamina();
    }

    public void UpdateStamina(float amount)
    {
        hud.ModifyStamina(amount);
    }
}
