using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[NodeIcon("text_interface_reload.png")]
public class TextReload : TextInterfaceNode {
    [JsonConstructor]
    private TextReload() : base() { }
    public TextReload(DocumentData workspace) : base(workspace) {}

    [JsonIgnore, XmlIgnore]
    protected override string nodeName => "Reload";

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "Reload";

    public override object Clone() {
        var n = new TextReload(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
