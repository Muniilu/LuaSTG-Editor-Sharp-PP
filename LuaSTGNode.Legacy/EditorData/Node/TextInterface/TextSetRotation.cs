using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[NodeIcon("text_interface_set_rotation.png")]
public class TextSetRotation : TextInterfaceNode {
    [JsonConstructor]
    private TextSetRotation() : base() { }
    public TextSetRotation(DocumentData workspace) : base(workspace) {
        attributes.Add(new AttrItem("Rotation", this));
    }

    [JsonIgnore, XmlIgnore]
    protected override string nodeName => "Set Rotation";

    [JsonIgnore, XmlIgnore]
    protected override string nodeAttrs => DefaultedValue(0, "0");

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "SetRotation";

    [JsonIgnore, XmlIgnore]
    protected override bool isSetter => true;

    public override object Clone() {
        var n = new TextSetRotation(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
