using UnityEngine;

/// <summary>Editable player values shared by movement and future combat scripts.</summary>
public sealed class PlayerAttributes : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField, Min(0f)] private float moveSpeed = 5f;
    [SerializeField, Min(0f)] private float jumpVelocity = 10f;

    [Header("Player")]
    [SerializeField, Min(1)] private int maxHealth = 100;
    [SerializeField, Min(0)] private int currentHealth = 100;
    [SerializeField, Min(0)] private int attackPower = 10;
    [SerializeField, Min(0)] private int defense = 0;

    public float MoveSpeed => moveSpeed;
    public float JumpVelocity => jumpVelocity;
    public int MaxHealth => maxHealth;
    public int CurrentHealth => currentHealth;
    public int AttackPower => attackPower;
    public int Defense => defense;

    private void OnValidate()
    {
        maxHealth = Mathf.Max(1, maxHealth);
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
    }

    public void TakeDamage(int amount)
    {
        currentHealth = Mathf.Max(0, currentHealth - Mathf.Max(0, amount - defense));
    }

    public void Heal(int amount)
    {
        currentHealth = Mathf.Min(maxHealth, currentHealth + Mathf.Max(0, amount));
    }
}
