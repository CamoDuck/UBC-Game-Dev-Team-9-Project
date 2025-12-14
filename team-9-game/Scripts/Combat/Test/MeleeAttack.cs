using Godot;
using System;

public partial class MeleeAttack : AttackSource
{
	[Export]
	public double disposingTime { get; set; } = 1;
	
	private double thisTime = 0;

	[Export]
	public bool LeftSide
	{
		get => _leftSide;
		set
		{
			Rotation = value ? 0 : float.Pi;
			_leftSide = value;
		}
	}

	private bool _leftSide = false;
	
	
	
	public override void _Ready()
	{
	}
	
	public override void _Process(double delta)
	{
		thisTime += delta;
		if (thisTime >= disposingTime) QueueFree();
	}
}
