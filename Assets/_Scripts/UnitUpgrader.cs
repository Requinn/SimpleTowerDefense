using UnityEngine.UI;
using UnityEngine;

public class UnitUpgrader : MonoBehaviour{
    private Unit _unitToUpgrade;
    private int UpgradeIndex;
    [Header("Upgrade UI")]
    [SerializeField]
    private GameObject _upgradeCanvas;
    [SerializeField]
    private Text _unitName, _unitDamage, _unitRange, _unitRoF, _upgradeCost;
    [SerializeField]
    private GameObject _upgradeButton;

    public void Start() {
        _upgradeCanvas.SetActive(false);
    }

    /// <summary>
    /// Update the UI component
    /// </summary>
    private void UpdateUI() {
        if (_unitToUpgrade) {
            //update all of the UI text on selection
            _unitName.text = _unitToUpgrade.name;
            _unitDamage.text = "Damage: " + _unitToUpgrade.GetDamageValue().ToString();
            _unitRange.text = "Range: " + _unitToUpgrade.GetRange().ToString();
            _unitRoF.text = "Fire Delay: " + _unitToUpgrade.GetRateOfFire().ToString();
            //if we have upgrades ready to perform, enable the button
            if(_unitToUpgrade.GetUpgradeArraySize() > 0 && _unitToUpgrade.GetUpgradeCount() < _unitToUpgrade.GetUpgradeArraySize()) {
                _upgradeCost.text = "Upgrade\n" + _unitToUpgrade.GetUpgrade(_unitToUpgrade.GetUpgradeCount()).cost.ToString();
                _upgradeButton.SetActive(true);
            }else {
                _upgradeButton.SetActive(false);
            }
        }
        //if no unit is selected, then clear the UI
        else {
            _unitName.text = "";
            _unitDamage.text = "Damage: ";
            _unitRange.text = "Range: ";
            _unitRoF.text = "Fire Rate: ";
            _upgradeCost.text = "Upgrade/n";
        }
    }

    /// <summary>
    /// determine what we should do when a unit is clicked
    /// </summary>
    /// <param name="unit"></param>
    public void DetermineAction(Unit unit) {
        //if no unit selected
        if(_unitToUpgrade == null) {
            SelectUnit(unit);
        }
        //if picking another unit
        else if(_unitToUpgrade != unit) {
            //set the current unit's range display off
            _unitToUpgrade.SetRangeDisplayActive(false);
            SelectUnit(unit);
        }
        //if selecting the same unit
        else if(_unitToUpgrade == unit) {
            DeselectUnit(unit);
        }
    }

    /// <summary>
    /// Selected a unit, so updae ui and display its stats
    /// </summary>
    /// <param name="unit"></param>
    public void SelectUnit(Unit unit) {
        _unitToUpgrade = unit;
        UpgradeIndex = _unitToUpgrade.GetUpgradeCount();
        _unitToUpgrade.SetRangeDisplayActive(true);
        UpdateUI();
        _upgradeCanvas.SetActive(true);
    }

    /// <summary>
    /// deselect the unit, clear ui and turn it off
    /// </summary>
    /// <param name="unit"></param>
    public void DeselectUnit(Unit unit) {
        if(_unitToUpgrade == unit) {
            _unitToUpgrade.SetRangeDisplayActive(false);
            _unitToUpgrade = null;
            UpgradeIndex = 0;
            _upgradeCanvas.SetActive(false);
            UpdateUI();
        }
    }

    /// <summary>
    /// Check if we have funds and upgrade if we do
    /// </summary>
    public void TryUpgradeStats() {
        if(_unitToUpgrade.GetUpgrade(_unitToUpgrade.GetUpgradeCount()).cost <= GameController.Instance.GetFunds()) {
            GameController.Instance.ModifyFunds(-_unitToUpgrade.GetUpgrade(_unitToUpgrade.GetUpgradeCount()).cost);
            _unitToUpgrade.PerformUpgrade();
            //reselect the unit to update the ui and stuff
            SelectUnit(_unitToUpgrade);
        }
    }
}
