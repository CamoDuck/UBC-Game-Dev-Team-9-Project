using Godot;
using System;
using System.Transactions;

public abstract partial class Ability : Node
{
    /// <summary> # of seconds where the player is unable to cast other abilites </summary>
    protected float windupTime;
    /// <summary> # of seconds before ability finishes casting and goes on cooldown </summary>
    protected float castTime;
    /// <summary> # of seconds that the ability cannot be used again after being cast </summary>
    protected float cooldown; 
    /// <summary> action used to trigger the ability </summary>
    protected string inputAction;

    float timer = -1;

    // This is where the actual ability is implemented (movement, damage, etc)
    protected abstract void AbilityImplementation();

    public void UseAbility()
    {
        timer = 0;
        AbilityImplementation();
    }

    public bool IsReady()
    {
        return timer == -1;
    }

    public bool IsWindingUp()
    {
        return !IsReady() && timer <= windupTime;
    }

    public bool IsCasting()
    {
        return !IsReady() && !IsWindingUp() && timer <= castTime;
    }

    public bool IsOnCooldown()
    {
        return !IsReady() && !IsWindingUp() && !IsCasting() && timer <= cooldown;
    }

    public void SetAbilityToReady()
    {
        timer = -1;
    }

    public override void _Process(double delta)
    {
        float dt = (float) delta;

        if (!IsReady())
        {
            timer += dt;
            if (timer > cooldown)
            {
                SetAbilityToReady();
            }
        }

        if (Input.IsActionPressed(inputAction) && IsReady())
        {
            UseAbility();
        }

    }

}
