using UnityEngine;

public class HpStamina : MonoBehaviour
{
    public float maxHp = 100f;
    public float maxStamina = 100f;
    public float currentHp;
    public float currentStamina;


    void Start()
    {
        ResetHP();
    }
    public void ResetHP()
    {
        currentHp = maxHp;
        currentStamina = maxStamina;
    }

    public void TakeDamage(float damage)
    {
        currentHp -= damage;
        if (currentHp < 0)
        {
            currentHp = 0;
        }
    }

    public void Heal(float health)
    {
                currentHp += health;
        if (currentHp > maxHp)
        {
            currentHp = maxHp;
        }
    }
}
