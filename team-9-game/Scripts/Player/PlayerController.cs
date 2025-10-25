using Godot;
using System;

public partial class PlayerController : CharacterBody2D
{
	[Export]
	public float Speed { get; set; } = 400;

	#region Jump
	/// <summary>
	/// The maximum speed the player can reach when jumping.
	/// </summary>
	[Export]
	public float MaxJumpSpeed { get; set; } = 150;

	/// <summary>
	/// The minimum speed the player can reach when jumping.
	/// <br/>
	/// This usually applies when the jump button is tapped quickly.
	/// </summary>
	[Export]
	public float MinJumpSpeed { get; set; } = 100;

	/// <summary>
	/// The maximum time the player can hold the jump button to reach max jump speed.
	/// <br/>
	/// This usually applies when the jump button is held down.
	/// </summary>
	[Export]
	public double MaxJumpTime { get; set; } = 0.5;

	/// <summary>
	/// While users hold the space button less than this value,
	/// the initial jumping speed will be MinJumpSpeed
	/// </summary>
	[Export]
	public double MinJumpTime { get; set; } = 0.2;

	private double JumpTimer = 0;

	private bool IsJumpTiming = false;
	#endregion



	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		KeyMovementProcess(delta);
		MoveAndSlide();
	}

	///<summary>
	/// This function handles key movement
	///</summary>
	private void KeyMovementProcess(double delta)
	{
		float X = 0, Y = Velocity.Y;
		// Don't want to overwrite the vertical velocity. Especially while falling.
		if (Input.IsActionPressed("move_left"))
			X -= Speed;
		
		if (Input.IsActionPressed("move_right"))
			X += Speed ;

		if (Input.IsActionJustPressed("jump") && IsOnFloor())
		{
			IsJumpTiming = true;//Enable the timer
		}
		else if (Input.IsActionPressed("jump"))
		{
			JumpTimer = IsJumpTiming ? JumpTimer + delta : JumpTimer;//Record the timer
		}
		
		if (CanJump())// While "jump" is released or timer reaches the maximum time
		{
			IsJumpTiming = false;
			Y -= CalculateJumpSpeed();// do jump
			JumpTimer = 0;// initialize the timer
		}

		Velocity = new Vector2(X, Y);
	}

	/// <summary>
	/// This function checks whether the player can jump.
	/// </summary>
	/// <returns></returns>
	private bool CanJump()
	{
		return IsOnFloor() && IsJumpTiming &&
			(Input.IsActionJustReleased("jump") || JumpTimer >= MaxJumpTime);
	}

	///<summary>
	/// Linear interpolate the initial speed of jumping
	///</summary>
	///<returns> The intial speed </returns>
	private float CalculateJumpSpeed()
	{
		if (JumpTimer < MinJumpTime)
			return MinJumpSpeed;
		if (JumpTimer >= MaxJumpTime)
			return MaxJumpSpeed;
		var timer_duration = MaxJumpTime - MinJumpTime;
		var speed_range = MaxJumpSpeed - MinJumpSpeed;
		var res = MinJumpSpeed + speed_range * (JumpTimer - MinJumpTime) / timer_duration;
		GD.Print($"Calculated Result: {res}");
		return (float)res;
	}
}
