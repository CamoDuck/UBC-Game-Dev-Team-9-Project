using Godot;
using System;

public partial class NotificationItem : PanelContainer
{
	[Export] public Label MessageLabel { get; set; }
	[Export] public TextureRect IconRect { get; set; }
	[Export] public float DisplayTime { get; set; } = 3.0f;

	public override void _Ready()
	{
		// Fade in
		Modulate = new Color(1, 1, 1, 0);
		var tween = CreateTween();
		tween.TweenProperty(this, "modulate:a", 1.0f, 0.3f);
		
		// Wait and then fade out
		tween.TweenInterval(DisplayTime);
		tween.TweenProperty(this, "modulate:a", 0.0f, 0.5f);
		tween.Finished += () => QueueFree();
	}

	public void Setup(string text, Texture2D icon)
	{
		if (MessageLabel != null) MessageLabel.Text = "Unlocked: " + text;
		if (IconRect != null) IconRect.Texture = icon;
	}
}
