using UnityEngine;

/// <summary>
/// Custom melee weapon
/// </summary>
public class Sword : MeleeWeapon
{
    // Sword rotation is added to the weapon gameobject in order to simulate an attack animation

    // Weapon rotation value is stored
    private Vector3 _attackRotation = new Vector3(70f, 0f, 0f);

    protected override void AttackBegin()
    {
        // The weapon is rotated for the value stored once the attack starts
        transform.Rotate(_attackRotation);
        base.AttackBegin();
    }

    protected override void AttackEnd()
    {
        // When the attack is finished, the rotation is undone by negating its original value
        transform.Rotate(-_attackRotation);
        base.AttackEnd();
    }

}
