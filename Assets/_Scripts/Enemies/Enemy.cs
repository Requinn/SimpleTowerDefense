using JLProject.Spline;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Basic Stats")]
    [SerializeField]
    protected int _damage; //damage dealt to base
    [SerializeField]
    protected float _health; //damage enemy can take
    [SerializeField]
    protected float _speed; //how fast along the spling this moves
    [SerializeField]
    protected int _value = 3; //how much funds is returned on kill
    [SerializeField]
    protected SplineWalker _walkerComponent;
    [SerializeField]
    protected HealthBar _healthBar;

    public delegate void GoalReached(int damage);
    public GoalReached EndReached;

    public delegate void OnDeath(int valueGain);
    public OnDeath DeathEvent;

    private int _baseDamage, _baseValue;
    private float _baseHealth, _baseSpeed, _maxHealth;

    public void Awake() {
        _baseDamage = _damage;
        _baseHealth = _health;
        _baseSpeed = _speed;
        _baseValue = _value;
    }

    /// <summary>
    /// Initialize this unit with a multiplied value.
    /// </summary>
    /// <param name="path">The path this unit is to follow.</param>
    /// <param name="damageMult">Multiply base power by this value</param>
    /// <param name="hpMult">Multiply base health by this value</param>
    /// <param name="speedMult">Multiply base speed by this value</param>
    /// <param name="fundsMult">Multiply funds rewards by this value</param>
    public void Initialize(BezierSpline path, float damageMult = 1f, float hpMult = 1f, float speedMult = 1f, float fundsMult = 1f) {
        _damage = Mathf.FloorToInt(_baseDamage * damageMult);
        _health = _maxHealth = Mathf.Floor(_baseHealth * hpMult);
        _speed = _baseSpeed * speedMult;
        _value = Mathf.FloorToInt(_baseValue * fundsMult);
        _walkerComponent.Initialize(_speed, path);
        _walkerComponent.OnEndReached += DealDamage;
    }

    public float GetHealth() {
        return _health;
    }
    /// <summary>
    /// End reached, fire off the event and die.
    /// </summary>
    private void DealDamage() {
        if(EndReached != null) EndReached(_damage);
        Destroy(gameObject);
    }

    public void TakeDamage(float damage) {
        _health -= damage;
        _healthBar.SetHealth(_health / _maxHealth);
        if (_health <= 0f) {
            DeathEvent(_value);
            Destroy(gameObject);
        }
    }
    /// <summary>
    /// modify the speed of the unit
    /// </summary>
    /// <param name="newSpeed"></param>
    public void ModifySpeed(float speedMultiplier = -256) {
        if (speedMultiplier == -256) {
            _walkerComponent.ResetSpeed();
        }
        else {
            _walkerComponent.SetSpeed(_speed - (_speed * speedMultiplier));
        }
    }
}
