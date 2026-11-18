using LuaSTGEditorSharp.EditorData.Message;
using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[NodeIcon("text_interface_set_fill.png")]
public class TextSetColor : TextInterfaceNode {
    [JsonConstructor]
    private TextSetColor() : base() { }
    public TextSetColor(DocumentData workspace) : base(workspace) {
        var mode = new DependencyAttrItem("Input Mode", ColorInput.Direct, this, "text_interface_color_input");
        attributes.Add(mode);
        RefreshAttrs(mode);
    }

    private static class ColorInput {
        public const string Direct = "Direct";
        public const string Gradient = "Gradient";
    }

    public override void ReflectAttr(DependencyAttrItem relatedAttrItem, DependencyAttributeChangedEventArgs args) {
        RefreshAttrs(relatedAttrItem);
    }

    private void RefreshAttrs(AttrItem mode) {
        int modeIndex = attributes.IndexOf(mode);
        for (int i = attributes.Count - 1; i > modeIndex; i--)
            attributes.RemoveAt(i);

        switch (mode.AttrInput) {
            default:
            case ColorInput.Direct:
                attributes.Add(new AttrItem("Color(s)", this));
                break;

            case ColorInput.Gradient:
                attributes.Add(new AttrItem("Top Color", this));
                attributes.Add(new AttrItem("Bottom Color", this));
                break;
        }
    }

    [JsonIgnore, XmlIgnore]
    protected override string nodeName => "Set Fill Color";

    [JsonIgnore, XmlIgnore]
    protected override string nodeAttrs {
        get {
            switch (ArgVal(0)) {
                default:
                case ColorInput.Direct:
                    return DefaultedValue(1, "white");

                case ColorInput.Gradient:
                    return $"gradient from {ArgVal(1)} to {ArgVal(2)}";
            }
        }
    }

    protected override string LuaArgList() {
        switch (ArgVal(0)) {
            default:
            case ColorInput.Direct:
                return ArgVal(1);

            case ColorInput.Gradient:
                return $"{{{ArgVal(1)}, {ArgVal(2)}}}";
        }
    }

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "SetColor";

    [JsonIgnore, XmlIgnore]
    protected override bool isSetter => true;

    protected override void CheckErrors(List<MessageBase> errors) {
        if (ArgVal(0) is not ColorInput.Direct and not ColorInput.Gradient)
            errors.Add(new CustomMessage("Input Mode must be \"Direct\" or \"Gradient\".", 0, this));

        if (HasOnFrameAncestor())
            errors.Add(new CustomMessage($"SetColor should not be used in frame & render contexts; see SetColorFilter & SetAlpha.", 3, this));
    }

    public override object Clone() {
        var n = new TextSetColor(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
