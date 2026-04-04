using Godot;
using System;
using System.Diagnostics;
using System.Threading;

public partial class PanGameExample : Node2D
{
	[Export] private PanGame PanGame { get; set; }
	[Export] private RigidBody2D Food1 { get; set; }
	[Export] private RigidBody2D Food2 { get; set; }
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		PanGame.CountdownTime = 5; // Five seconds count down
		GD.Print(Food1);
		PanGame.AddFood(Food1);// If you want to handle it yourself, just ignore AddFood
		PanGame.AddFood(Food2);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void HandleGameFinished()
	{
		// Callback Function
		var availableRigid = PanGame.GetAvailableFood();
		PanGame.PauseGame = true;
		OS.Alert($"Game finished. {availableRigid.Length} left!");
	}
}
