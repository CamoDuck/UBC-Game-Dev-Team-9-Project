using Godot;
using System;

public partial class Dash : Ability
{
    GameManager game;
    PlayerController player;
    float duration;
    float strength;

    public override void _Ready()
    {
        game = GameManager.Get();
        player = game.playerController;

        windupTime = 0;
        castTime = 0;
        duration = 0.2f;
        cooldown = duration + 0.05f;
        strength = 2000;
        inputAction = "movement_ability";
    }

    private float interpFunction(float normalizedTimeLeft)
    {
        return normalizedTimeLeft;
    }

    protected override void AbilityImplementation()
    {
        player.forceManager.ApplyForce(player.lastMoveDirection, strength, duration, interpFunction);
    }


}
