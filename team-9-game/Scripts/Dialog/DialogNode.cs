#nullable enable
using System.Collections.Generic;
using Godot.Collections;

namespace Team9Game.Scripts.Dialog;

public class DialogNode
{
    /// <summary>
    /// This is the ID for this dialog node.
    /// It should be unique within the dialog tree.
    /// </summary>
    public string ID { get; private set; }
        
    public string Text { get; private set; }

    /// <summary>
    /// If there is only one response,
    /// it means there is no choice to be made,
    /// and the dialog will proceed automatically.
    /// </summary>
    public List<(string option, string value)> Options = [];
        
    public string[] Speaker { get; private set; }

    /// <summary>
    /// If it is an empty string or invalid, the background image will
    /// not change.
    /// </summary>
    public string? BackgroundChange  = null;

    public System.Collections.Generic.Dictionary<string, string>? SpeakerChanges = null;
    
    public string? TitleChange = null;

    public DialogNode(){}
    
    public DialogNode(Dictionary data)
    {
        ID = data["id"].AsString();
        Text = data["text"].AsString();
        Speaker = data["speaker"].AsStringArray();
        BackgroundChange = data.GetValueOrDefault("background").AsString();
        InitializeOptions(data["options"].AsGodotArray());
    }

    private void InitializeOptions(Array data)
    {
        foreach (var item in data)
        {
            var opts = item.AsStringArray();
            Options.Add((opts[0], opts[1]));
        }
    }

    private void InitializeChanges(Dictionary data)
    {
        foreach (var item in data)
        {
            var key = item.Key.AsString();
            var value = item.Value;
            switch (key)
            {
                case "background":
                    BackgroundChange = value.AsString();
                    break;
                case "speakers":
                    InitializeSpeakerChanges(value.AsGodotDictionary());
                    break;
                case "title":
                    TitleChange = value.AsString();
                    break;
            }
        }
    }
    
    private void InitializeSpeakerChanges(Dictionary data)
    {
        SpeakerChanges = [];
        foreach (var item in data)
        {
            SpeakerChanges[item.Key.AsString()] = item.Value.AsString();
        }
    }
}