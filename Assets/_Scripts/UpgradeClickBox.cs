using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Goes on the unit hit box to be clicked on
/// </summary>
public class UpgradeClickBox : MonoBehaviour
{
    public void OnMouseDown() {
        GameController.Instance.GetUpgrader().DetermineAction(GetComponentInParent<Unit>());
    }
}
