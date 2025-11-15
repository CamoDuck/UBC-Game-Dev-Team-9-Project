using Godot;
using System;
using System.Diagnostics;

public partial class Health : Node2D
{
	public Node2D Parent => GetParent<Node2D>();
	
	[Export]
	public double MaxHealth { get; set; } = 100.0;
	
	public double CurrentHealth { get; private set; }

	[Export]
	public double Defense { get; set; } = 0;
	
	[Signal]
	public delegate void OnDamagedEventHandler(
		double damage, AttackSource source);
	
	[Signal]
	public delegate void OnDeathEventHandler();
	
	public void TakeDamage(double damage, AttackSource source)
	{
		CurrentHealth -= damage;
		EmitSignalOnDamaged(damage, source);
		Console.WriteLine("Damage taken: " + damage + ", Current Health: " + CurrentHealth);
		if (CurrentHealth <= 0) EmitSignalOnDeath();
	}

	/// <summary>
	/// This method sets the current health to the specified value,
	/// capped at MaxHealth.
	/// </summary>
	/// <param name="health"></param>
	public void ApplyHealth(double health)
	{
		CurrentHealth = Math.Min(MaxHealth, health);
		if (CurrentHealth <= 0) EmitSignalOnDeath();
	}

	public void TakeHealth(double health)
		=> CurrentHealth = Math.Min(MaxHealth, health + CurrentHealth);


	public override void _Ready()
	{
		CurrentHealth = 100;
		this.OnDeath += () => 
		{
			Console.WriteLine("Entity has died.");
			Process.GetCurrentProcess().Kill();
		};
	}
	
	public override void _Process(double delta) { }
	
}
