using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Unit base class
/// </summary>
public class Unit : MonoBehaviour
{
    protected enum AimType {
        First, //FILO
        Last, //LIFO
        Strongest, //HP > ALL
        Weakest //HP < ALL
    }
    [Header("Base Properties")]
    [SerializeField]
    protected int _buildCost = 15;
    [SerializeField]
    protected Projectile _bullet;
    [SerializeField]
    protected float _rangeRadius = 10;
    [SerializeField]
    protected float _impactDamage = 5;
    [SerializeField]
    protected float _bulletSpeed = 50f; 
    [SerializeField]
    protected float _rateOfFire = 1;
    [SerializeField]
    private UpgradeValues[] Upgrades;
    [SerializeField]
    protected GameObject _rangeMarker;

    protected bool _isSelected = false;
    public GameObject nobuildCollider;

    private int _upgradeCount = 0;
    private CapsuleCollider _rangeDetection;
    private float _timeSinceFire = 99f;
    [SerializeField]
    private List<Enemy> _targets = new List<Enemy>();
    private List<Enemy> _targetByHealth = new List<Enemy>();

    private Enemy _currentTarget = null;

    public void Initialize() {
        _rangeDetection = GetComponent<CapsuleCollider>();
        UpdateRadius();
        _rangeMarker.SetActive(true);
        nobuildCollider.SetActive(false);
    }

    public void TurnOn() {
        UpdateRadius();
        _rangeMarker.SetActive(false);
        nobuildCollider.SetActive(true);
    }

    public int GetCost() {
        return _buildCost;
    }

    public float GetDamageValue() {
        return _impactDamage;
    }

    public float GetRange() {
        return _rangeRadius;
    }

    public int GetUpgradeCount() {
        return _upgradeCount;
    }

    public float GetRateOfFire() {
        return _rateOfFire;
    }

    public int GetUpgradeArraySize() {
        return Upgrades.Length;
    }

    public UpgradeValues GetUpgrade(int upgradeIndex) {
        return Upgrades[upgradeIndex];
    }

    /// <summary>
    /// Upgrade the stats
    /// </summary>
    /// <param name="damage"></param>
    /// <param name="roF"></param>
    /// <param name="range"></param>
    internal void ModifyStats(float damage, float roF, float range) {
        _impactDamage += damage;
        _rateOfFire -= roF;
        _rangeRadius += range;
        UpdateRadius();
    }

    /// <summary>
    /// Called from the UpgradeSystem to perform the upgrade
    /// </summary>
    public void PerformUpgrade() {
        //redundancy check
        if (_upgradeCount < Upgrades.Length) {
            ModifyStats(Upgrades[_upgradeCount].damageIncrease, Upgrades[_upgradeCount].RoFIncrease, Upgrades[_upgradeCount].RangeIncrease);
            _upgradeCount++;
        }
    }

    public void FixedUpdate() {
        //since update runs once enabled, activate this so we can't build on top of the object
        if (!nobuildCollider.activeInHierarchy) {
            nobuildCollider.SetActive(true);
        }

        //while game is active
        if (GameController.Instance.GetGameActive()) {
            //seek target
            _currentTarget = GetPriorityTarget();
            //if we have a target
            if (_currentTarget != null) {
                //rotate to look at them
                transform.LookAt(_currentTarget.transform);
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, 0);
                //check if we can attack
                if(_timeSinceFire >= _rateOfFire) {
                    Attack(_currentTarget);
                }
                _timeSinceFire += Time.deltaTime;
            }
        }
    }

    protected void UpdateRadius() {
        _rangeDetection.radius = _rangeRadius;
        _rangeMarker.transform.localScale = new Vector3(_rangeRadius * 2, 0 , _rangeRadius * 2);
    }

    public void SetRangeDisplayActive(bool state) {
        _rangeMarker.SetActive(state);
    }

    /// <summary>
    /// Fire a bullet towards the current target
    /// </summary>
    /// <param name="target"></param>
    protected void Attack(Enemy target) {
        Projectile p = Instantiate(_bullet, transform.position, transform.rotation);
        _timeSinceFire = 0f;
        Vector3 fireDirection = (GetEnemyPredictedLocation(target) - transform.position).normalized;
        p.Initialize(fireDirection, _bulletSpeed, _impactDamage);
    }

    /// <summary>
    /// Get the firstmost target in the list, removing any nulls (dead) along the way.
    /// </summary>
    /// <returns></returns>
    private Enemy GetPriorityTarget() {
        Enemy selected = null;
        //if we have targets
        if (_targets.Count > 0) {
            //cycle through all targets and clean until you find a not null
            while (selected == null && _targets.Count > 0) {
                if (_targets.Count > 0 && _targets[0] == null) {
                    _targets.RemoveAt(0);
                }
                else {
                    selected = _targets[0];
                }
            }
        }
        return selected;
    }

    /// <summary>
    /// Predict where the enemy will be on the path so shots hit the target
    /// </summary>
    /// <param name="target"></param>
    /// <returns></returns>
    private Vector3 GetEnemyPredictedLocation(Enemy target) {
        SplineWalker enemyPath = target.GetComponent<SplineWalker>();
        Vector3 predictedLocation = target.transform.position;

        //get the time to reach target
        float dist = (predictedLocation - transform.position).magnitude;
        float timeToReach = dist / _bulletSpeed;

        //get the time along the spline added onto the progress of the enemy
        predictedLocation = enemyPath.GetPositionAt(enemyPath.GetProgress() + (timeToReach * Time.deltaTime));
        //Debug.DrawLine(transform.position, target.transform.position, Color.blue, 180f);
        //Debug.DrawLine(transform.position, predictedLocation, Color.red, 150f);
        return predictedLocation;
    }

    /// <summary>
    /// Comparator for health used in the target by strongest
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    static int CompareHealth(Enemy a, Enemy b) {
        return a.GetHealth().CompareTo(b.GetHealth());
    }

    /// <summary>
    /// Update the list to cull all nulls
    /// </summary>
    private void CleanList(int value) {
        List<Enemy> pendingRemoval = new List<Enemy>();
        foreach(Enemy e in _targets) {
            if(e == null) {
                pendingRemoval.Add(e);
            }
        }
        foreach(Enemy remove in pendingRemoval) {
            _targets.Remove(remove);
        }
        pendingRemoval.Clear();
    }

    /// <summary>
    /// When an enemy enters range, add then to the list
    /// </summary>
    /// <param name="other"></param>
    public void OnTriggerEnter(Collider other) {
        Enemy e = other.GetComponent<Enemy>();
        if (e) {
            e.DeathEvent += CleanList;
            _targets.Add(e);
        }
    }

    public void OnTriggerExit(Collider other) {
        _targets.Remove(other.GetComponent<Enemy>());
    }
}

public class TargetListObject {
    public int enterOrder;
    public Enemy enemy;

    public TargetListObject(int order, Enemy e) {
        enterOrder = order;
        enemy = e;
    }
}
[Serializable]
public class UpgradeValues {
    public float damageIncrease;
    public float RoFIncrease;
    public float RangeIncrease;
    public int cost;
}
