using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[NodeIcon("richtextupdate.png")]
public class TextLoad : TextInterfaceNode {
    [JsonConstructor]
    private TextLoad() : base() { }
    public TextLoad(DocumentData workspace) : base(workspace) {}

    [JsonIgnore, XmlIgnore]
    protected override string nodeName => "Load";

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "Load";

    public override object Clone() {
        var n = new TextLoad(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
