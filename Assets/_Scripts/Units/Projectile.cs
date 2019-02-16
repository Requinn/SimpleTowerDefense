using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// projectile base class
/// </summary>
public class Projectile : MonoBehaviour
{
    protected float _damage;
    protected Rigidbody _rigidbody;

    public void Start() {
        
    }

    public void Initialize(Vector3 direction, float speed, float damage) {
        _damage = damage;
        _rigidbody = GetComponent<Rigidbody>();
        _rigidbody.velocity = direction * speed;
        Destroy(gameObject, 6f);
    }

    public virtual void OnTriggerEnter(Collider other) {
        Enemy e = other.GetComponent<Enemy>();
        if (e) {
            e.TakeDamage(_damage);
            Destroy(gameObject);
        }
    }
}
