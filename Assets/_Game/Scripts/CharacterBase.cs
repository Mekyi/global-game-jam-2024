using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] 
    internal int MaxHealth;
    [SerializeField] 
    internal int CurrentHealth;
    [SerializeField]
    internal float speed;
    public UnityAction<GameObject> OnDeath;
    internal Rigidbody2D rBody;
    internal Vector3 lookDir;
    internal float shootTimer;
    internal Vector3 position;
    internal bool canShoot => shootTimer <= 0f;

    private void Start()
    {
        CurrentHealth = MaxHealth;
    }

    public virtual void DealDamage(int amount)
    {
        CurrentHealth -= amount;
        if (CurrentHealth > 0)
            return;
        Debug.Log(gameObject.name + " is dead");
        Death();
    }

    private void Death()
    {
        OnDeath?.Invoke(gameObject);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
    public void Heal(int amount)
    {
        CurrentHealth = Mathf.Min(CurrentHealth + amount, MaxHealth);
    }
}
