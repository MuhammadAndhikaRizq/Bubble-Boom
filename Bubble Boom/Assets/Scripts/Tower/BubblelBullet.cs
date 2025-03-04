using System.Collections;
using UnityEngine;

public class BubbleBullet : MonoBehaviour
{
    private int damage;
    private Transform target;
    private float lifetime = 5f; // Waktu hidup proyektil

    public void Initialize(int damageAmount, Transform enemyTarget)
    {
        damage = damageAmount;
        target = enemyTarget;
        
        // Hancurkan proyektil setelah waktu tertentu
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Cek apakah objek yang ditabrak bisa menerima damage
        IDamageAble damagable = collision.gameObject.GetComponent<IDamageAble>();
        
        if (damagable != null)
        {
            damagable.TakeDamage(damage);
        }

        // Hancurkan proyektil setelah menabrak
        Destroy(gameObject);
    }
}
