using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Handles placing a unit down on the field
/// </summary>
public class UnitPlacementHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject _validPlacementMarker;
    [SerializeField]
    private Material _validMaterial, _invalidMaterial;
    [SerializeField]
    private Unit[] _placeableUnits;
    private int _currentPlacingID = -1;
    private Unit _unitToPlace, _heldUnit;
    private bool _isPlacingUnit = false;
    private bool _isValidLocation = true;

    public void Start() {
        _validPlacementMarker.SetActive(false);
    }

    /// <summary>
    /// Given button input, determine what to do
    /// </summary>
    /// <param name="UnitID"></param>
    public void DeterminePlacementAction(int UnitID) {
        //check if we have the funds period
        if (GameController.Instance.GetFunds() >= _placeableUnits[UnitID].GetCost()) {
            //if we aren't placing something, just go into placing stuff 
            if (!_isPlacingUnit) {
                _currentPlacingID = UnitID;
                StartPlacingUnit(_placeableUnits[UnitID]);
            }
            //if we are placing something, and it's the same unit
            else if (_isPlacingUnit && _currentPlacingID == UnitID) {
                _currentPlacingID = -1;
                //if we were placing something else, remove it and replace it with the new one
                StopPlacingUnit();
            }
            //if we are placing something, and it's not the same
            else if (_isPlacingUnit && _currentPlacingID != UnitID) {
                _currentPlacingID = UnitID;
                if (_heldUnit) {
                    Destroy(_heldUnit.gameObject);
                    _heldUnit = null;
                }
                StartPlacingUnit(_placeableUnits[UnitID]);
            }
        }
        else {
            //You have no funds!
        }
    }

    /// <summary>
    /// Assign a unit to be placed on the field
    /// </summary>
    /// <param name="toPlace"></param>
    private void StartPlacingUnit(Unit toPlace) {
        if (!_isPlacingUnit) {
            _isPlacingUnit = true;
            _heldUnit = Instantiate(toPlace, Input.mousePosition, Quaternion.identity);
            _heldUnit.name = toPlace.name;
            _heldUnit.Initialize();
            _heldUnit.GetComponent<Collider>().enabled = false;
            _heldUnit.nobuildCollider.SetActive(false);
            _heldUnit.enabled = false;
            _validPlacementMarker.SetActive(true);
        }
    }

    /// <summary>
    /// Stop placing a unit destroy it.
    /// </summary>
    private void StopPlacingUnit() {
        _isPlacingUnit = false;
        if (!_heldUnit) { return; }
        Destroy(_heldUnit.gameObject);
        _heldUnit = null;
        _validPlacementMarker.SetActive(false);
    }

    /// <summary>
    /// Place a unit down and enable it
    /// </summary>
    private void PlaceUnitDown() {
        GameController.Instance.ModifyFunds(-_heldUnit.GetCost());
        _isPlacingUnit = false;
        _heldUnit.GetComponent<Collider>().enabled = true;
        _heldUnit.enabled = true;
        _heldUnit.TurnOn();
        _heldUnit = null;
        _validPlacementMarker.SetActive(false);
    }

    /// <summary>
    /// Handles the color of the marker, as well as assigning where we can place objects
    /// </summary>
    /// <param name="isValid"></param>
    public void SetValidPlacement(bool isValid) {
        _isValidLocation = isValid;
        if (_isValidLocation) {
            _validPlacementMarker.GetComponent<Renderer>().material = _validMaterial;
        }else {
            _validPlacementMarker.GetComponent<Renderer>().material = _invalidMaterial;
        }
    }

    private Ray ray;
    Plane hPlane;
    float dist = 0f;
    public void Update() {
        if (_isPlacingUnit && _heldUnit) {
            //Ray from mouse to world
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            //Plane to prject mouse position on to
            hPlane = new Plane(Vector3.up, transform.position);
            //if the mouse hits the plane
            if(hPlane.Raycast(ray, out dist)) {
                _heldUnit.transform.position = ray.GetPoint(dist) + new Vector3(0,1f,0);
                _validPlacementMarker.transform.position = ray.GetPoint(dist) + new Vector3(0, .5f, 0);
            }
            //if for w/e reason it doesn't hit
            else {
                _heldUnit.transform.position = Vector3.zero;
                _validPlacementMarker.transform.position = Vector3.zero;
            }

            //stops from placing under ui elements
            if (EventSystem.current.IsPointerOverGameObject(-1)){
                return;
            }
            //We clicked to place unit
            if (Input.GetMouseButton(0) && _isValidLocation) {
                PlaceUnitDown();
            }
            else if(Input.GetMouseButton(0) && !_isValidLocation) {
                //idk maybe do some kind hey you can't do that
            }
        }
    }
}
