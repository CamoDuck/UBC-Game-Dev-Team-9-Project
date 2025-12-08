using Godot;
using System;
using System.Diagnostics;
using Team9Game.Scripts.Dialog;

public partial class DialogOptionUi : Control
{
	[Export]
	public Json DialogueData { get; set; }
	
	[Export]
	public string StartFrom { get; set; } = "";
	
	[Signal]
	public delegate void DialogFinishedEventHandler();

	private DialogContext context;
	
	private DialogRunner runner;

	#region UIInitialize
	[Export]
	public VBoxContainer ChoicesStack { get; set; }
	
	[Export]
	public Label TextLabel { get; set; }
	
	[Export]
	public Label SpeakerLabel { get; set; }
	
	[Export]
	public TextureRect BackgroundImage { get; set; }
	#endregion
	
	public override void _Ready()
	{
		
		//Here, initialize everything
		
		context = new DialogContext();
		runner = new DialogRunner(context);
		if (string.IsNullOrEmpty(StartFrom)) 
			runner.Start();
		else runner.StartFrom(StartFrom);
		//var file = FileAccess.Open(DialogueJsonPath, FileAccess.ModeFlags.Read);
		//var jsonText = file.GetAsText();

		//var json = Json.ParseString(jsonText).AsGodotDictionary();
		Update();
	}
	
	public override void _Process(double delta)
	{
	}

	private void Update()
	{
		var choices = runner.Choices;
		TextLabel.Text = runner.Text;
		var speakers = runner.Speakers;
		SpeakerLabel.Text = string.Join('&', speakers);
		ClearChoices();
		var ind = 0;
		foreach (var choice in choices)
		{
			var choiceBtn = NewChoiceUI(choice, ind++);
			choiceBtn.Pressed += () => OnChoiceSelected(choiceBtn);
			ChoicesStack.AddChild(choiceBtn);
		}

		BackgroundImage.Texture = runner.CurrentImage;
	}

	private void ClearChoices()
	{
		var choices = ChoicesStack.GetChildren();
		foreach (var choice in choices)
			choice.QueueFree();
	}

	private DialogChoiceUI NewChoiceUI(string choice, int index)
	{
		var scene = ResourceLoader.Load<PackedScene>("res://Scenes/Dialog/DialogChoiceUI.tscn");
		var ret = scene.Instantiate<DialogChoiceUI>();
		ret.ChoiceIndex = index;
		ret.ChoiceText = choice;
		return ret;
	}

	private void OnChoiceSelected(DialogChoiceUI btn)
	{
		if (runner.Next(btn.ChoiceIndex))
		{
			EmitSignalDialogFinished();
			return;
		}
		Update();
	}
}
