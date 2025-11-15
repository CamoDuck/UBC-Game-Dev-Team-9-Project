using Godot;
using System;
using System.Linq;
using Team9Game.Scripts.Combat;

public partial class Attack : Node2D
{
	[Export]
	public bool IsActive { get; set; } = true;
	
	private Health Parent => GetParent<Health>();
	
	[Export]
	public string HitboxName { get; set; } = "Hitbox";
	
	public override void _Ready()
	{
		Console.WriteLine("Hello");
		Parent.ChildEnteredTree += ParentOnChildEnteredTree;
		var hitbox = Parent.GetNode<Area2D>(HitboxName);
		hitbox.AreaEntered += OnDamageAreaBodyEntered;
	}

	//Automatically connect to any Area2D added to the parent
	private void ParentOnChildEnteredTree(Node node)
	{
		Console.WriteLine("ParentOnChildEnteredTree");
		if (node.Name == HitboxName && node is Area2D area)
		{
			area.AreaEntered += OnDamageAreaBodyEntered;
		}
			
		
	}


	private void OnDamageAreaBodyEntered(Node2D body)
	{
		Console.WriteLine("Hit " + body.Name);
		if (!IsActive || body is not AttackSource attackSource) return;
		if (!attackSource.SelfDamage && attackSource.Parent == Parent.Parent) return; // Prevent self-damage
		Parent.TakeDamage(CalculateDamage(attackSource), attackSource);
	}
	
	private double CalculateDamage(AttackSource attackRegion)
	{
		var baseDamage = attackRegion.AttackPower;
		var defense = Parent.Defense;
		var damage = baseDamage - defense;
		Console.WriteLine("Damage: " + damage);
		return Math.Max(damage, 0);
	}
	
	
	public override void _Process(double delta){}
}

public enum AttackType
{
	Melee,
	Ranged,
	Magic,
	Explosive
}
