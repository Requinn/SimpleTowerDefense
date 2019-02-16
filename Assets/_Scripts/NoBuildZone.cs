using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoBuildZone : MonoBehaviour
{
    [SerializeField]
    private UnitPlacementHandler _placer;

    private void Start() {
        if(_placer == null) {
            _placer = GameController.Instance.GetPlacementHandler();
        }
    }
    public void OnMouseEnter() {
        _placer.SetValidPlacement(false);
    }

    public void OnMouseExit() {
        _placer.SetValidPlacement(true);
    }
}
