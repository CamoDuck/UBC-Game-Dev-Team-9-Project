using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
    [Export] public ForceManager forceManager;
    [Export]
    public float walkSpeed { get; set; } = 400;

    /// <summary>
    /// The change in velocity when jumping
    /// </summary>
    [Export]
    public float jumpImpulse { get; set; } = 1000;

    public Vector2 lastMoveDirection = Vector2.Right;


    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        KeyMovementProcess(delta);
        MoveAndSlide();
    }

    ///<summary>
    /// This function handles key movement
    ///</summary>
    private void KeyMovementProcess(double delta)
    {
        UpdateMoveVelocity();
        UpdateLastMoveDirection();
    }

    private void UpdateMoveVelocity()
    {
        float X = 0, Y = Velocity.Y;
        if (Input.IsActionPressed("move_left"))
        {
            X -= walkSpeed;
        }
        if (Input.IsActionPressed("move_right"))
        {
            X += walkSpeed;
        }
        if (Input.IsActionPressed("jump") && CanJump())
        {
            Y -= jumpImpulse;
        }

        Velocity = new Vector2(X, Y);
    }

    private void UpdateLastMoveDirection()
    {
        if (Math.Abs(Velocity.X) > 0)
        {
            lastMoveDirection = new Vector2(Velocity.X, 0);
        }
    }

    /// <summary>
    /// This function checks whether the player can jump.
    /// </summary>
    /// <returns></returns>
    private bool CanJump()
    {
        return IsOnFloor();
    }
}
