using Godot;

public partial class DialogChoiceUI : TextureButton
{
	[Export]
	public int ChoiceIndex { get; set; }

	[Export]
	public Label ChoiceLabel;
	
	[Export]
	public string ChoiceText
	{
		get => ChoiceLabel.Text;
		set => ChoiceLabel.Text = value;
	}
}
