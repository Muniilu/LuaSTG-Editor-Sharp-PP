using LuaSTGEditorSharp.EditorData.Message;
using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[NodeIcon("text_interface_set_size.png")]
public class TextSetSize : TextInterfaceNode {
    [JsonConstructor]
    private TextSetSize() : base() { }
    public TextSetSize(DocumentData workspace) : base(workspace) {
        attributes.Add(new AttrItem("Font Size", this));
    }

    [JsonIgnore, XmlIgnore]
    protected override string nodeName => "Set Font Size";

    [JsonIgnore, XmlIgnore]
    protected override string nodeAttrs => ArgVal(0);

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "SetSize";

    [JsonIgnore, XmlIgnore]
    protected override bool isSetter => true;

    protected override void CheckErrors(List<MessageBase> errors) {
        if (NilOrEmpty(ArgVal(0)))
            errors.Add(new CustomMessage($"Font Size cannot be nil.", 0, this));

        if (HasOnFrameAncestor())
            errors.Add(new CustomMessage($"SetSize should not be used in frame & render contexts; see SetScale.", 3, this));
    }

    public override object Clone() {
        var n = new TextSetSize(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
