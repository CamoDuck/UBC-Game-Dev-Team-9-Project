using Godot;

public partial class DialogSpeakerShower : MarginContainer
{
	[Export]
	public TextureRect SpeakerImg;

	[Export]
	public Texture2D SpeakerTexture
	{
		get => SpeakerImg.Texture;
		set => SpeakerImg.Texture = value;
	}
}
