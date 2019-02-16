using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Slows enemies in range
/// </summary>
public class Slow : Unit
{
    public new void OnTriggerEnter(Collider other) {
        Enemy e = other.GetComponent<Enemy>();
        if (e) {
            e.ModifySpeed(_impactDamage);
        }
    }

    public new void OnTriggerExit(Collider other) {
        Enemy e = other.GetComponent<Enemy>();
        if (e) {
            e.ModifySpeed(-256);
        }
    }
}
