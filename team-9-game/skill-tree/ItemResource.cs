using Godot;
using System;

[Tool]
[GlobalClass]
public partial class ItemResource : Resource
{
	[Export]
	public string ItemName { get; set; } = "New Item";

	[Export(PropertyHint.MultilineText)]
	public string Description { get; set; } = "";

	[Export]
	public Texture2D Icon { get; set; }

	[Export]
	public int MaxStackSize { get; set; } = 64;

	[Export]
	public bool IsConsumable { get; set; } = false;
}
