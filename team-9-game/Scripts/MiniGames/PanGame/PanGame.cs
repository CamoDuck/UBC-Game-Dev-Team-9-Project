using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class PanGame : Node2D
{
	[Export] private AnimatableBody2D Pan { get; set; }

	[Export] private float RotationCoefficient { get; set; } = 0.02f;

	[Export] private float PanHeightRatio { get; set; } = 0.33f;

	/// <summary>
	/// A countdown timer. If you don't want to listen to OnGameFinishedEvent,
	/// just leave it there.
	/// </summary>
	[Export] public float CountdownTime { get; set; } = float.NaN;
	
	/// <summary>
	/// Event that triggers when fails or times up
	/// </summary>
	[Signal] public delegate void OnGameFinishedEventHandler();
	
	/// <summary>
	/// If you want to pause the minigame, set it to true <br/>
	/// Otherwise, set it to false
	/// </summary>
	public bool PauseGame { get; set; }
	
	private Vector2 MousePosition;
	private float RotationSpeed;
	private bool left;
	private List<RigidBody2D> foods = [];
	private bool initialized = false;
	
	private Vector2 Size => GetViewportRect().Size;

	private Vector2 Center => Size / 2;

	private Vector2 PanExpectedPosition 
		=> new (Center.X, Size.Y * (1 - PanHeightRatio));
	
	private bool Left {
		get => left;
		set
		{
			if (left != value) RotationSpeed = 0;
			left = value;
		}
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		GD.Print("Size: " + Size);
		GD.Print("Center: " + Center);
		GD.Print("Pan Expected Pos: " + PanExpectedPosition);
		
	}
	
	/// <summary>
	/// Add food to the list.
	/// </summary>
	/// <param name="food"></param>
	public void AddFood(RigidBody2D food)
	{
		foods.Add(food);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (PauseGame) return;
		
		// GODOT FIX: INITIAL SIZE IS 64, 64
		if (!initialized && Size == new Vector2(64, 64)) {}
		else initialized = true;
		
		MousePosition = GetGlobalMousePosition();
		Left = MousePosition.X < Center.X;
		RotationSpeed += (Left? -RotationCoefficient : RotationCoefficient) * (float)delta * 60;
		
		CountdownTime -= (float)delta;
		if (CountdownTime <= 0) EmitSignal(SignalName.OnGameFinished);
	}

	public override void _PhysicsProcess(double delta)
	{
		if (PauseGame) return;
		base._PhysicsProcess(delta);
		if (Pan.Position != PanExpectedPosition)
		{
			Pan.SyncToPhysics = false;
			Pan.Position = PanExpectedPosition;
			Pan.SyncToPhysics = true;
		}
			
		switch (Pan.RotationDegrees)
		{
			case >= 90 when RotationSpeed > 0:
				Pan.RotationDegrees = 90;
				break;
			case <= -90 when RotationSpeed < 0:
				Pan.RotationDegrees = -90;
				break;
			default:
				Pan.RotationDegrees += RotationSpeed * (float)delta * 150;
				break;
		}

		if (!CheckNotFail()) EmitSignal(SignalName.OnGameFinished);
	}

	private bool CheckNotFail()
	{
		return foods.Count == 0 || foods.Any(CheckRigidBody);
	}

	private bool CheckRigidBody(RigidBody2D food)
	{ 
		// GODOT FIX
		if (!initialized) return true;
		
		return food.Position is { X: >= 0 } &&
		       food.Position.X <= Size.X && food.Position.Y <= Size.Y;
	}

	/// <summary>
	/// Get foods that are still in the viewport.<br/>
	/// Recommended: call this when OnGameFinished is triggered
	/// </summary>
	public RigidBody2D[] GetAvailableFood()
	{
		return foods.Where(CheckRigidBody).ToArray();
	}
}
