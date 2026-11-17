using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[NodeIcon("text_interface_set_template_arg.png")]
public class TextSetTemplateArg : TextInterfaceNode {
    [JsonConstructor]
    private TextSetTemplateArg() : base() { }
    public TextSetTemplateArg(DocumentData workspace) : base(workspace) {
        attributes.Add(new AttrItem("Index", this));
        attributes.Add(new AttrItem("Value", this));
    }

    [JsonIgnore, XmlIgnore]
    protected override string nodeName {
        get {
            string indexVal = ArgVal(0);
            return "Set Template Arg" + (indexVal == "" ? "" : $" #{indexVal}");
        }
    }

    [JsonIgnore, XmlIgnore]
    protected override string nodeAttrs => ArgVal(1);

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "SetTemplateArg";

    [JsonIgnore, XmlIgnore]
    protected override bool isSetter => true;

    public override object Clone() {
        var n = new TextSetTemplateArg(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
