using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[NodeIcon("text_interface_unload.png")]
public class TextUnload : TextInterfaceNode {
    [JsonConstructor]
    private TextUnload() : base() { }
    public TextUnload(DocumentData workspace) : base(workspace) {}

    [JsonIgnore, XmlIgnore]
    protected override string nodeName => "Unload";

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "Unload";

    public override object Clone() {
        var n = new TextUnload(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
