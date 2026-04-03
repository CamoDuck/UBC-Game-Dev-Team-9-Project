using Godot;

public partial class WeaponTooltip : Control
{
	private TextureRect _closeUpRect;
	private Label _descriptionLabel;
	private Label _titleLabel;

	public override void _Ready()
	{
		_closeUpRect = GetNode<TextureRect>("MarginContainer/VBoxContainer/CloseUpRect");
		_descriptionLabel = GetNode<Label>("MarginContainer/VBoxContainer/Description");
		_titleLabel = GetNodeOrNull<Label>("MarginContainer/VBoxContainer/TitleLabel");
	}

	public void Setup(string title, Texture2D image, string description)
	{
		if (_titleLabel != null) _titleLabel.Text = title ?? "Unknown Weapon";
		if (_closeUpRect != null) _closeUpRect.Texture = image;
		if (_descriptionLabel != null) _descriptionLabel.Text = description ?? "No description available.";
	}
}
