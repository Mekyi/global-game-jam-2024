using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

public class CharacterBase : MonoBehaviour
{
    [SerializeField] 
    internal float maxHealth;
    [SerializeField] 
    internal float currentHealth;
    [SerializeField]
    internal float speed;
    public UnityAction<GameObject> OnDeath;
    internal Rigidbody2D rBody;
    internal Vector3 lookDir;
    internal float shootTimer;
    internal Vector3 position;
    internal bool canShoot => shootTimer <= 0f;
    internal GrayscaleShaderScaler grayScaler;

    internal virtual void Start()
    {
        Heal(maxHealth);
        grayScaler = GetComponent<GrayscaleShaderScaler>();
    }

    public virtual void DealDamage(float amount)
    {
        currentHealth -= amount;
        SetGrayScale();
        if (currentHealth > 0)
            return;
        Death();
    }

    protected virtual void SetGrayScale()
    {
        grayScaler?.SetColorScale(1 - currentHealth / maxHealth);
    }

    internal virtual void Death()
    {
        OnDeath?.Invoke(gameObject);
    }

    public void Die()
    {
        Destroy(gameObject);
    }
    public void Heal(float amount)
    {
        currentHealth = Mathf.Min(currentHealth + amount, maxHealth);
        SetGrayScale();
    }
}
