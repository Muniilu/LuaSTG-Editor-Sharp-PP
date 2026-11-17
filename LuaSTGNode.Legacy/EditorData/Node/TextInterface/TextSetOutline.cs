using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[NodeIcon("richtextsetoutline.png")]
public class TextSetOutline : TextInterfaceNode {
    [JsonConstructor]
    private TextSetOutline() : base() { }
    public TextSetOutline(DocumentData workspace) : base(workspace) {
        attributes.Add(new AttrItem("Width", this));
        attributes.Add(new AttrItem("Intensity", "16", this));
        attributes.Add(new AttrItem("Color", "cols.black", this));
        attributes.Add(new AttrItem("Offset X", this));
        attributes.Add(new AttrItem("Offset Y", this));
    }

    [JsonIgnore, XmlIgnore]
    protected override string nodeName => $"{(IsDisable() ? "Disable" : "Set")} Outline";

    [JsonIgnore, XmlIgnore]
    protected override string nodeAttrs {
        get {
            string attrs = $"width {ArgVal(0)}"
                + ", intensity " + DefaultedValue(1, "16")
                + ", color " + DefaultedValue(2, "cols.black");

            if (ArgVal(3) != "" || ArgVal(4) != "")
                attrs += $" @ offset ({DefaultedValue(3, "0")}, {DefaultedValue(4, "0")})";

            return attrs;
        }
    }

    protected override string GetNodeAttrs() {
        return IsDisable() ? "" : $" to {nodeAttrs}";
    }

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "SetOutline";

    [JsonIgnore, XmlIgnore]
    protected override bool isSetter => true;

    private bool IsDisable() {
        string width = ArgVal(0);
        if (NilOrEmpty(width)) return true;

        bool literal = float.TryParse(width, out float widthVal);
        return literal && widthVal <= 0.001;
    }

    [JsonIgnore, XmlIgnore]
    protected override bool allArgsOptional => true;

    public override object Clone() {
        var n = new TextSetOutline(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
