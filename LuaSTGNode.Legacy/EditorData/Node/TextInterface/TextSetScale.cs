using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[NodeIcon("text_interface_set_scale.png")]
public class TextSetScale : TextInterfaceNode {
    [JsonConstructor]
    private TextSetScale() : base() { }
    public TextSetScale(DocumentData workspace) : base(workspace) {
        attributes.Add(new AttrItem("Scale (X)", "1", this));
        attributes.Add(new AttrItem("Scale Y", this));
    }

    [JsonIgnore, XmlIgnore]
    protected override string nodeName => "Set Scale";

    [JsonIgnore, XmlIgnore]
    protected override string nodeAttrs {
        get {
            string scaleX = DefaultedValue(0, "1");
            string scaleY = ArgVal(1);

            if (NilOrEmpty(scaleY))
                return scaleX;

            else
                return $"({scaleX}, {scaleY})";
        }
    }

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "SetScale";

    [JsonIgnore, XmlIgnore]
    protected override bool isSetter => true;

    [JsonIgnore, XmlIgnore]
    protected override bool allArgsOptional => true;

    public override object Clone() {
        var n = new TextSetScale(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
