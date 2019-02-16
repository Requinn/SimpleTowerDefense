using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : Projectile
{
    [SerializeField]
    private float _blastRadius = 5f;

    public override void OnTriggerEnter(Collider other) {
        bool _hit = false;
        RaycastHit[] _targets = Physics.SphereCastAll(transform.position, _blastRadius, Vector3.up);
        foreach (var t in _targets) {
            Enemy e = t.collider.GetComponent<Enemy>();
            if (e) {
                _hit = true;
                e.TakeDamage(_damage);
            }
        }
        //if we hit something
        if (_hit) {
            Destroy(gameObject);
        }
    }
}
