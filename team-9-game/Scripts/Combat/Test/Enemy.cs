using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	private double time = 0;
	
	public override void _Ready()
	{}
	
	public override void _Process(double delta)
	{
		time += delta;
		if (time >= 2)
		{
			time = 0;
			Console.WriteLine("Enemy Attack!");
			Attack();
		}
		MoveAndSlide();
	}

	private void Attack()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Combat/Test/melee_attack.tscn");
		MeleeAttack attack = scene.Instantiate<MeleeAttack>();
		this.AddChild(attack);
	}
}
