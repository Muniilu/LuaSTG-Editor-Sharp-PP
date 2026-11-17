using LuaSTGEditorSharp.EditorData.Message;
using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[NodeIcon("text_interface_set_fill.png")]
public class TextSetColor : TextInterfaceNode {
    [JsonConstructor]
    private TextSetColor() : base() { }
    public TextSetColor(DocumentData workspace) : base(workspace) {
        attributes.Add(new AttrItem("Color(s)", this));
    }

    [JsonIgnore, XmlIgnore]
    protected override string nodeName => "Set Fill Color";

    [JsonIgnore, XmlIgnore]
    protected override string nodeAttrs => IsReset() ? "white" : ArgVal(0);

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "SetColor";

    [JsonIgnore, XmlIgnore]
    protected override bool isSetter => true;

    private bool IsReset() {
        string color = ArgVal(0);
        return NilOrEmpty(color) || color == "{}";
    }

    protected override void CheckErrors(List<MessageBase> errors) {
        if (HasOnFrameAncestor())
            errors.Add(new CustomMessage($"SetColor should not be used in frame & render contexts; see SetColorFilter & SetAlpha.", 3, this));
    }

    public override object Clone() {
        var n = new TextSetColor(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
