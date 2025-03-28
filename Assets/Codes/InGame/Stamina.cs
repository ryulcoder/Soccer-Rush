using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    [SerializeField] float totalStamina;
    [SerializeField] float stamina;
    [SerializeField] float regenRate;

    GameManager GameManager;

    public Slider StaminaSlider;

    bool isCoroutine;

    void Start()
    {
        GameManager = GameManager.Instance;

        totalStamina = GameManager.totalStamina;
        regenRate = GameManager.reGenRate;

        stamina = totalStamina;

        StaminaSlider.minValue = 0;   // 최소값 설정
        StaminaSlider.maxValue = totalStamina; // 최대값 설정
        StaminaSlider.value = totalStamina;
        StaminaSlider.wholeNumbers = true;

        InvokeRepeating(nameof(ReGenStamina), 2, 1); // 스테미너 리젠 함수 주기호출
    }

    // Update is called once per frame
    void Update()
    {
        if (stamina != totalStamina)
        {
            StaminaSlider.value = Mathf.Lerp(StaminaSlider.value, stamina, Time.deltaTime * 2);
        }

    }

    void ReGenStamina()
    {
        if (stamina + regenRate * 100  > totalStamina)
            regenRate = totalStamina;
        else
            stamina += regenRate * 100;
    }

    public void UseStamina(float sta)
    {
        if (sta < 0) return;
        
        stamina -= sta;
    }
}
