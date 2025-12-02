using UnityEngine;
using UnityEngine.UI;

public class Enemy06_Hp : MonoBehaviour
{
    public Enemy06 enemy06;
    public Slider hpSlider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void Setup(Enemy06 enemy)
    {
        enemy06 = enemy;

        if (enemy06 != null && hpSlider != null)
        {
            hpSlider.maxValue = enemy06.maxHealth;
            hpSlider.value = enemy06.health;
        }

    }

    // Update is called once per frame
    void Update()
    {
        if (enemy06 == null || hpSlider == null) return;
        hpSlider.value = enemy06.health;
    }
}
