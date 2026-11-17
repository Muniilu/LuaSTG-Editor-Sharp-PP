using LuaSTGEditorSharp.EditorData.Message;
using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[NodeIcon("text_interface_set_alignment.png")]
public class TextSetAlignment : TextInterfaceNode {
    [JsonConstructor]
    private TextSetAlignment() : base() { }
    public TextSetAlignment(DocumentData workspace) : base(workspace) {
        attributes.Add(new AttrItem("Horizontal", Unchanged, this, "text_interface_hAlign"));
        attributes.Add(new AttrItem("Vertical", Unchanged, this, "text_interface_vAlign"));
    }

    public const string Unchanged = "Unchanged";
    private static readonly List<string> HAlign = ["Left", "Center", "Right"];
    private static readonly List<string> VAlign = ["Top", "Middle", "Right"];

    [JsonIgnore, XmlIgnore]
    protected override string nodeName {
        get {
            string hAlign = ArgVal(0);
            string vAlign = ArgVal(1);

            if (hAlign == Unchanged && vAlign != Unchanged)
                return "Set Vertical Alignment";
            else if (vAlign == Unchanged && hAlign != Unchanged)
                return "Set Horizontal Alignment";

            return "Set Alignment";
        }
    }

    [JsonIgnore, XmlIgnore]
    protected override string nodeAttrs {
        get {
            string hAlign = ArgVal(0);
            string vAlign = ArgVal(1);

            if (hAlign == Unchanged && vAlign != Unchanged)
                return vAlign;
            else if (vAlign == Unchanged && hAlign != Unchanged)
                return hAlign;

            return $"{hAlign}, {vAlign}";
        }
    }

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "SetAlignment";

    [JsonIgnore, XmlIgnore]
    protected override bool isSetter => true;

    protected override string LuaArgList() {
        static string FormatAlign(string value, List<string> valid) =>
            valid.Contains(value) ? $"aln.{value.ToUpper()}" : "nil";

        return $"{FormatAlign(ArgVal(0), HAlign)}, {FormatAlign(ArgVal(1), VAlign)}";
    }

    protected override void CheckErrors(List<MessageBase> errors) {
        string hAlign = ArgVal(0);
        string vAlign = ArgVal(1);

        if (!HAlign.Contains(hAlign) && hAlign != Unchanged)
            errors.Add(new CustomMessage("Horizontal alignment must be \"Left\", \"Center\", \"Right\" or \"Unchanged\".", 0, this));

        if (!VAlign.Contains(vAlign) && vAlign != Unchanged)
            errors.Add(new CustomMessage("Vertical alignment must be \"Top\", \"Middle\", \"Bottom\" or \"Unchanged\".", 0, this));
    }

    public override object Clone() {
        var n = new TextSetAlignment(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
