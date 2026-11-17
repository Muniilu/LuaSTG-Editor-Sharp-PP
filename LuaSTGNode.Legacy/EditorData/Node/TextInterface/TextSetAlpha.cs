using LuaSTGEditorSharp.EditorData.Message;
using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[NodeIcon("text_interface_alpha.png")]
public class TextSetAlpha : TextInterfaceNode {
    [JsonConstructor]
    private TextSetAlpha() : base() { }
    public TextSetAlpha(DocumentData workspace) : base(workspace) {
        attributes.Add(new AttrItem("Alpha", "1", this));
    }

    [JsonIgnore, XmlIgnore]
    protected override string nodeName => "Set Alpha";

    [JsonIgnore, XmlIgnore]
    protected override string nodeAttrs => DefaultedValue(0, "1");

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "SetAlpha";

    [JsonIgnore, XmlIgnore]
    protected override bool isSetter => true;

    [JsonIgnore, XmlIgnore]
    protected override bool allArgsOptional => true;

    protected override void CheckErrors(List<MessageBase> errors) {
        bool literal = float.TryParse(ArgVal(0), out float alphaVal);

        if (literal && (alphaVal < 0 || alphaVal > 1))
            errors.Add(new CustomMessage("Alpha should be between 0 and 1.", 3, this));
    }

    public override object Clone() {
        var n = new TextSetAlpha(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
