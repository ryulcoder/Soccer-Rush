using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    public static Stamina instance;

    [SerializeField] float totalStamina;
    [SerializeField] float stamina;
    [SerializeField] float regenTime;

    GameManager GameManager;
    Player Player;

    public Slider StaminaSlider;
    //public Text StaminaText;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        GameManager = GameManager.Instance;
        Player = Player.Instance;

        totalStamina = GameManager.totalStamina;
        regenTime = GameManager.regenTime;

        stamina = totalStamina;

        StaminaSlider.minValue = 0;   // 최소값 설정
        StaminaSlider.maxValue = totalStamina; // 최대값 설정
        StaminaSlider.value = stamina;

        InvokeRepeating(nameof(ReGenStamina), 2, regenTime);
    }

    
    void Update()
    {
        if (StaminaSlider.value != stamina)
        {
            StaminaSlider.value = Mathf.Lerp(StaminaSlider.value, stamina, Time.deltaTime * 5);
            //StaminaText.text = stamina + " / " + totalStamina;
        }
    }

    void ReGenStamina()
    {
        if (stamina >= totalStamina || Player.getTackled)
        {
            return;
        }

        if (stamina + 0.05f * totalStamina > totalStamina)
            stamina = totalStamina;
        else
            stamina += 0.05f * totalStamina;
    }

    public bool UseStamina(float sta)
    {
        if (stamina - sta < 0) return false;

        if (!GameManager.Tutorial.isTuto)
            stamina -= sta;

        return true;
    }

    public void GetLimitStamina(string limitType)
    {
        int limitSta;

        switch (limitType)
        {
            /*case "AvoidMove":
                if (!(Player.stateInfo.IsName("Move_Left") || Player.stateInfo.IsName("Move_Right"))) 
                    return;

                limitSta = 5;
                break;*/

            case "AvoidJump":
                if (!Player.stateInfo.IsName("Jump_Run"))
                    return;

                limitSta = 5;
                break;

            case "AvoidSpin":
                if (!Player.stateInfo.IsName("Spin_Left"))
                    return;

                limitSta = 5;
                break;

            default:
                return;
        }

        if (stamina + limitSta > totalStamina)
            stamina = totalStamina;
        else
            stamina += limitSta;

        Debug.LogWarning("추가 스태미너 +5");
    }
}
