using System;
using UnityEngine;

public class HealthSystem
{
    public event EventHandler OnHealthChanged;
    public event EventHandler OnDamage;
    public event EventHandler OnHeal;
    public event EventHandler OnDeath;

    private int _healthMax;
    private int _health;

    public HealthSystem(int healthMax)
    {
        _healthMax = healthMax;
        _health = _healthMax;
    }

    public int GetHealth()
    {
        return _health;
    }

    public void Damage(int damage)
    {
        _health -= damage;

        if(_health < 0)
        {
            _health = 0;
            OnDeath?.Invoke(this, EventArgs.Empty);
            return;
        }

        OnDamage?.Invoke(this, EventArgs.Empty);
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Heal(int heal)
    {
        _health += heal;

        if(_health > _healthMax)
        {
            _health = _healthMax;
        }

        OnHeal?.Invoke(this, EventArgs.Empty);
        OnHealthChanged?.Invoke(this, EventArgs.Empty);
    }

    /// <summary>
    /// Tries to get a HealthSystem from the GameObject
    /// The GameObject can have either the built in HealthSystemComponent script or any other script that creates
    /// the HealthSystem and implements the IGetHealthSystem interface
    /// </summary>
    /// <param name="getHealthSystemGameObject">GameObject to get the HealthSystem from</param>
    /// <param name="healthSystem">output HealthSystem reference</param>
    /// <param name="logErrors">Trigger a Debug.LogError or not</param>
    /// <returns></returns>
    public static bool TryGetHealthSystem(GameObject getHealthSystemGameObject, out HealthSystem healthSystem, bool logErrors = false)
    {
        healthSystem = null;

        if (getHealthSystemGameObject != null)
        {
            if (getHealthSystemGameObject.TryGetComponent(out IGetHealthSystem getHealthSystem))
            {
                healthSystem = getHealthSystem.GetHealthSystem();
                if (healthSystem != null)
                {
                    return true;
                }
                else
                {
                    if (logErrors)
                    {
                        Debug.LogError($"Got HealthSystem from object but healthSystem is null! Should it have been created? Maybe you have an issue with the order of operations.");
                    }
                    return false;
                }
            }
            else
            {
                if (logErrors)
                {
                    Debug.LogError($"Referenced Game Object '{getHealthSystemGameObject}' does not have a script that implements IGetHealthSystem!");
                }
                return false;
            }
        }
        else
        {
            // No reference assigned
            if (logErrors)
            {
                Debug.LogError($"You need to assign the field 'getHealthSystemGameObject'!");
            }
            return false;
        }
    }

    public float GetHealthNormalized()
    {
        return (float)_health / (float)_healthMax;
    }
}
