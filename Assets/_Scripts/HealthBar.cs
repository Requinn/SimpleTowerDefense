using UnityEngine.UI;
using UnityEngine;

//update the enemy's healthbar
public class HealthBar : MonoBehaviour
{
    [SerializeField]
    private Image _healthBar;

    public void SetHealth(float healthPercent) {
        _healthBar.fillAmount = healthPercent;
    }
}
