using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusBars : MonoBehaviour
{
    public float m_HealthPercent = 1f;
    public float m_StaminaPercent = 1f;

    public Image m_HealthFill;
    public Image m_StaminaFill;

    public void SetHealth(float health)
    {
        m_HealthPercent = health;
        m_HealthFill.fillAmount = m_HealthPercent;
    }

    public void SetStamina(float stamina)
    {
        m_StaminaPercent = stamina;
        m_StaminaFill.fillAmount = m_StaminaPercent;
    }
}
