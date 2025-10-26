using Godot;
using System;

public partial class Gravity : Node2D
{
	[Export]
	public float GravityAcceleration { get; set; } = 9.8f;

	private CharacterBody2D Parent;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Parent = GetParent<CharacterBody2D>();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Parent is null) return;
		Parent.Velocity += new Vector2(0, GravityAcceleration * (float)delta);
	}
}
