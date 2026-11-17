using LuaSTGEditorSharp.EditorData.Message;
using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[NodeIcon("text_interface_set_blend.png")]
public class TextSetBlendMode : TextInterfaceNode {
    [JsonConstructor]
    private TextSetBlendMode() : base() { }
    public TextSetBlendMode(DocumentData workspace) : base(workspace) {
        attributes.Add(new AttrItem("Blend Mode", this, "blend"));
    }

    [JsonIgnore, XmlIgnore]
    protected override string nodeName => "Set Blend Mode";

    [JsonIgnore, XmlIgnore]
    protected override string nodeAttrs => DefaultedValue(0, "\"\"");

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "SetBlendMode";

    [JsonIgnore, XmlIgnore]
    protected override bool isSetter => true;

    protected override void CheckErrors(List<MessageBase> errors) {
        string raw = ArgVal(0);
        if (raw.Length < 2 || raw[0] != '"' || raw[raw.Length - 1] != '"') return; // string literals only

        string blendMode = raw.Substring(1, raw.Length - 2);
        string[] parts = blendMode.Split('+');

        if (blendMode == "one" || (parts.Length == 2
          && parts[0] is "add" or "mul"
          && parts[1] is "sub" or "mul" or "min" or "max"))
            errors.Add(new CustomMessage($"Blend mode \"{blendMode}\" looks different across text engines.", 5, this));
    }

    public override object Clone() {
        var n = new TextSetBlendMode(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
