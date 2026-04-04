using Godot;
using System.Collections.Generic;

[Tool]
[GlobalClass]
public partial class TalentResource : Resource
{
	[Export]
	public Texture2D TalentIcon { get; set; }

	[Export]
	public bool IsUnlocked { get; set; } = false;
	/// When true, the icon and tooltip info are hidden until the resource is unlocked.
	/// Set this to true on weapon resources, leave false on talent resources.
	
	[Export]
	public bool HideUntilUnlocked { get; set; } = false;

	[Export] public string AbilityName { get; set; } 
	
	[Export] public string WeaponDescription { get; set; }

	[Export]
	public Godot.Collections.Array<TalentResource> UnlockTalents { get; set; }
		= new Godot.Collections.Array<TalentResource>();
}
