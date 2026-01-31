using Godot;
using System;

public partial class AttackSource : Area2D
{
	[Export]
	public double AttackPower { get; set; } = 10.0;
	
	[Export]
	public AttackType Type { get; set; } = AttackType.Melee;
	
	[Export]
	public bool SelfDamage { get; set; } = false;
	
	public Node2D Parent => GetParent<Node2D>();
	
	public override void _Ready() { }

	public override void _Process(double delta)
	{
		
	}
}
