using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[LeafNode]
[NodeIcon("richtextrender.png")]
public class TextRender : TextInterfaceNode {
    [JsonConstructor]
    private TextRender() : base() { }
    public TextRender(DocumentData workspace) : base(workspace) {
        attributes.Add(new AttrItem("X", this));
        attributes.Add(new AttrItem("Y", this));
    }

    [JsonIgnore, XmlIgnore]
    protected override string nodeName => "Render";

    [JsonIgnore, XmlIgnore]
    protected override string nodeAttrs => $"at ({ArgVal(0)}, {ArgVal(1)})";

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "Render";

    [JsonIgnore, XmlIgnore]
    protected override bool allowChaining { get => false; }

    public override object Clone() {
        var n = new TextRender(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
