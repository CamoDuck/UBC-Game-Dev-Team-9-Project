using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Team9Game.Scripts.Dialog;

public class DialogRunner(DialogContext context)
{
    public Texture2D CurrentImage { get; private set; }
        = ResourceLoader.Load<Texture2D>(context.InitialBackGroundUri);
    
    private DialogNode _currentNode = new();

    public Dictionary<string, Texture2D> Characters { get; private set; }
        = GetPortrait(context.Characters);

    public string[] Choices => _currentNode.Options.Select(a => a.option).ToArray();

    public string[] Speakers => _currentNode.Speaker;
    
    public string Text => _currentNode.Text;
    
    private readonly DialogContext _context = context;

    public void Start()
    {
        _currentNode = _context.Nodes[_context.StartID];
        UpdateBackground();
    }

    public void StartFrom(string id)
    {
        _currentNode = _context.Nodes[id];
        UpdateBackground();
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
        UpdateBackground();
        return false;
    }

    private void UpdateBackground()
    {
        var uri = _currentNode.BackgroundUri;
        if (uri == "") return;
        try
        {
            CurrentImage = ResourceLoader.Load<Texture2D>(uri);
        }
        catch
        {
            // ignored
        }
    }

    private static Dictionary<string, Texture2D> GetPortrait(Dictionary<string, string> characters)
    {
        Dictionary<string, Texture2D> ret = [];
        foreach (var chr in characters)
        {
            var uri = chr.Value;
            var texture = ResourceLoader.Load<Texture2D>(uri);
            ret[chr.Key] = texture;
        }
        return ret;
    }
}