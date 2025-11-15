using Godot;
using System;

public partial class PlayerTESTcs: PlayerController
{
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("attack"))
			Attack();
		base._Process(delta);
	}
	
	private void Attack()
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Combat/Test/melee_attack.tscn");
		var attack = scene.Instantiate<MeleeAttack>();
		attack.AttackPower = 100;
		this.AddChild(attack);
	}

	
}
