using UnityEngine;
using System.Collections;
/// <summary>
/// Abstract class for <see cref="CombatSystem"/> melee Weapons to inherit from
/// </summary>
public abstract class MeleeWeapon : MonoBehaviour, IWeapon
{
    // Damage dealt by the weapon
    [SerializeField] protected float _damage;

    // Collider used to detect contact between the weapon and other objects when attacking
    [SerializeField] protected Collider _weaponCollider;

    // Duration the weapon collider should be active. IE: swinging a sword
    [SerializeField] protected float _attackActiveTime;

    // Cooldown time between attacks, to prevent spamming
    [SerializeField] protected float _timeBetweenAttacks;
    protected bool _canAttack = true;   // control field
    protected bool _attacking = false;   // control field

    [Tooltip("String with the character tag the weapon can deal damage to. If empty, will deal damage to all")]
    [SerializeField] protected string _enemyTag;

    protected void Awake()
    {
        // The weapon collider must begin disabled
        _weaponCollider.enabled = false;

        if (_enemyTag == "") Debug.LogWarning("No enemy tag was set. Damage will be sent to all colliders hit");
    }

    // Public method to be called by the CombatSystem to perform the attack
    public virtual void Attack()
    {
        if (_canAttack)
        {
            // If not already attacking, the weapon collider is activated,
            // and a coroutine timer begins to disable it once the attack is finished
            StartCoroutine(PerformAttack());
        }
    }

    protected IEnumerator PerformAttack()
    {
        AttackBegin();
        yield return new WaitForSeconds(_attackActiveTime);
        AttackEnd();
    }

    protected virtual void AttackBegin()
    {
        _canAttack = false; // _canAttack is set to false, to prevent "multiattacking"
        _weaponCollider.enabled = true;
        _attacking = true;
    }

    protected virtual void AttackEnd()
    {
        _weaponCollider.enabled = false;
        _attacking = false;
        // Coroutine timer is substracted from the time between attacks, into a remaining time variable
        // until next attack is available. This is done because the value can end up being negative
        var resetTimeLeft = _timeBetweenAttacks - _attackActiveTime;
        StartCoroutine(WaitBetweenAttacks(resetTimeLeft));
    }

    protected IEnumerator WaitBetweenAttacks(float time)
    {
        // If there's time remaining to wait between attacks, the coroutine will wait for
        // that amount. In any case, the control field to allow attacking again will be enabled
        if(time > 0) yield return new WaitForSeconds(time);
        _canAttack = true;
    }

    // OnTriggerEnter detects collisions, allowing to interact with the gameobject the collider is attatched to
    // Combat is managed through this interaction
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Player has hit " + other.tag);

        if (other.CompareTag(_enemyTag))
        {
            if (other.TryGetComponent<CombatSystem>(out CombatSystem character))
            {
                character.DamageHandler(_damage);
            }
        }
    }
}