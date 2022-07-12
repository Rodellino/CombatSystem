using System.Collections;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    //--- Inspector Settings
    #region Settings
    [Tooltip("Character total health points")]
    [SerializeField] private float _health = 100f;
    public float Health { get => _health; }

    [Tooltip("Invulnerability time after receiving damage")]
    [SerializeField] private float _invTime = 0.5f;
    #endregion

    //--- General fields and properties
    #region Fields and properties
    [Header("Character Hitbox Collider")]
    [SerializeField] private Collider _characterHitBoxCollider;

    [Header("Weapon")]
    [SerializeField] private MeleeWeapon _meleeWeapon;

    [Header("Use controls")]
    [Tooltip("Tick the box if the character is going to be controlled by the player")]
    [SerializeField] private bool _useControls;

    private bool _invulnerable = false;
    #endregion

    private void Start()
    {
        // Instantiation of the InputHandler for input capture and handle
        if (_useControls) InputHandler.StartInputHandler(this);
    }

    public void HandleInput(PlayerInput.Input input)
    {
        // Switch statement in case more combat inputs/actions need to be added 
        switch (input.Action)
        {
            case PlayerInput.Action.MeleeAttack:
                // Attack management is delegated to the weapon
                _meleeWeapon.Attack();
                break;
            default:
                Debug.Log("An invalid input was given to the combat system, and wasn't handled");
                break;
        }
    }

    /// <summary>
    /// public method to receive and manage damage
    /// </summary>
    /// <param name="damage">Total damage points received</param>
    public void DamageHandler(float damage)
    {
        // If damage was received recently, invulnerability prevents new damage to be processed
        if (_invulnerable) return;

        // Damage handling is passed to an external method, to separate complex actions, such as adding animations
        HandleDamage(damage);  

        if (Health <= 0)
        {
            // If current health drops below 0, the player is dead, and is managed in a separate method
            HandleDeath();
        }
        else
        {
            // If damage is received and is not fatal, start a coroutine that grants invulnerability time
            StartCoroutine(InvulnerabilityTimer());
        }
    }

    private IEnumerator InvulnerabilityTimer()
    {
        // Sets character invulnerability on for the duration defined in _invTime
        _invulnerable = true;
        yield return new WaitForSeconds(_invTime);
        _invulnerable = false;
    }

    private void HandleDamage(float damage)
    {
        // Damage points substract from the current character's health
        _health -= damage;
    }

    private void HandleDeath()
    {
        // Death is managed through an example animation in which the character is faded out and its gameobject destroyed
        StartCoroutine(FadeOutDeath());
    }

    private float fadeOutTime = 2f; // Death fade out "animation" time in seconds
    private IEnumerator FadeOutDeath()
    {
        var charColor = this.GetComponent<Renderer>().material.color;
        float timer = fadeOutTime;
        while (charColor.a > 0)
        {
            charColor = new Color(charColor.r, charColor.g, charColor.b, (timer / fadeOutTime));
            this.GetComponent<Renderer>().material.color = charColor;
            timer -= Time.deltaTime;
            yield return null;
        }
        Destroy(this.gameObject);
    }
}
