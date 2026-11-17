using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[NodeIcon("richtextsetshadow.png")]
public class TextSetShadow : TextInterfaceNode {
    [JsonConstructor]
    private TextSetShadow() : base() { }
    public TextSetShadow(DocumentData workspace) : base(workspace) {
        attributes.Add(new AttrItem("Blur", this));
        attributes.Add(new AttrItem("Scale", "1", this));
        attributes.Add(new AttrItem("Color", "cols.black", this));
        attributes.Add(new AttrItem("Offset X", this));
        attributes.Add(new AttrItem("Offset Y", this));
    }

    [JsonIgnore, XmlIgnore]
    protected override string nodeName => $"{(NilOrEmpty(ArgVal(0)) ? "Disable" : "Set")} RichText Shadow";

    [JsonIgnore, XmlIgnore]
    protected override string nodeAttrs {
        get {
            string attrs = $"blur {ArgVal(0)}"
                + ", scale " + DefaultedValue(1, "1")
                + ", color " + DefaultedValue(2, "cols.black");

            if (ArgVal(3) != "" || ArgVal(4) != "")
                attrs += $" @ offset ({DefaultedValue(3, "0")}, {DefaultedValue(4, "0")})";

            return attrs;
        }
    }

    protected override string GetNodeAttrs() {
        return NilOrEmpty(ArgVal(0)) ? "" : $" to {nodeAttrs}";
    }

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "SetShadow";

    [JsonIgnore, XmlIgnore]
    protected override bool isSetter => true;

    [JsonIgnore, XmlIgnore]
    protected override bool allArgsOptional => true;

    public override object Clone() {
        var n = new TextSetShadow(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
