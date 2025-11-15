using Godot;
using System;

public partial class MeleeAttack : AttackSource
{
	private double disposingTime = 1;
	
	private double thisTime = 0;
	
	public override void _Ready()
	{
	}
	
	public override void _Process(double delta)
	{
		thisTime += delta;
		if (thisTime >= disposingTime) QueueFree();
	}
}
