using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Team9Game.Scripts.Dialog;

public class DialogRunner(DialogContext context)
{
    public Texture2D CurrentImage { get; private set; }
        = ResourceLoader.Load<Texture2D>(context.InitialBackGroundUri);
    
    private DialogNode _currentNode = new();

    public Dictionary<string, Texture2D> SpeakerPortraits { get; private set; }
        = GetPortrait(context.Characters);
    
    public string Title { get; private set; }

    public string[] Choices => _currentNode.Options.Select(a => a.option).ToArray();

    public string[] Speakers => _currentNode.Speaker;
    
    public string Text => _currentNode.Text;
    
    private readonly DialogContext _context = context;

    public void Start()
    {
        _currentNode = _context.Nodes[_context.StartID];
        Title = _context.Title;
        Update();
    }

    public void StartFrom(string id)
    {
        _currentNode = _context.Nodes[id];
        Title = _context.Title;
        Update();
    }

    /// <summary>
    /// If returns true, it means the dialog comes to an end and no
    /// more dialog.
    /// </summary>
    /// <returns></returns>
    public bool Next(int choiceIndex)
    {
        var id = _currentNode.Options[choiceIndex].value;
        if (id == "end") return true;
        _currentNode = _context.Nodes[id];
        Update();
        return false;
    }

    private void Update()
    {
        UpdateBackground();
        UpdateTitle();
        UpdatePortrait();
    }

    private void UpdateBackground()
    {
        var uri = _currentNode.BackgroundChange;
        if (uri is null) return;
        try
        {
            CurrentImage = GD.Load<Texture2D>(uri);
        }
        catch
        {
            // ignored
        }
    }

    private void UpdateTitle()
    {
        var txt = _currentNode.TitleChange;
        if (txt is null) return;
        Title = txt;
    }

    private void UpdatePortrait()
    {
        var speakers =  _currentNode.SpeakerChanges;
        if (speakers is null) return;
        foreach (var (key, value) in speakers)
        {
            try
            {
                var texture = GD.Load<Texture2D>(value);
                SpeakerPortraits[key] = texture;
            }
            catch
            {
                //ignore
            }
            
        }
    }

    private static Dictionary<string, Texture2D> GetPortrait(Dictionary<string, string> characters)
    {
        Dictionary<string, Texture2D> ret = [];
        // Wow, new syntax!
        foreach (var (key, uri) in characters)
        {
            var texture = GD.Load<Texture2D>(uri);
            ret[key] = texture;
        }
        return ret;
    }
}