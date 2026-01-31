using Godot;
using System;

public partial class Gravity : Node2D
{
    [Export]
    public Vector2 gravity { get; set; } = new Vector2(0, 2000);

    private CharacterBody2D parent;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        parent = GetParent<CharacterBody2D>();
        if (parent == null)
        {
            GD.PrintErr("Missing parent 'CharacterBody2D' for 'Gravity' node");
            SetProcess(false); // Stop script if there is no parent
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        float dt = (float)delta;
        parent.Velocity += gravity * dt;
    }
}
