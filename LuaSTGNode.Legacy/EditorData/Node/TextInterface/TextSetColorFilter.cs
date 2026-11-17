using LuaSTGEditorSharp.EditorData.Message;
using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[NodeIcon("text_interface_filter_color.png")]
public class TextSetColorFilter : TextInterfaceNode {
    [JsonConstructor]
    private TextSetColorFilter() : base() { }
    public TextSetColorFilter(DocumentData workspace) : base(workspace) {
        string defInput = ColorInput.String;
        attributes.Add(new DependencyAttrItem("Color Input", defInput, this, "text_interface_color_type"));
        RefreshAttrs(defInput);
    }

    private static class ColorInput {
        public const string String = "ARGB String";
        public const string Split = "Split RGBA";
    }

    public override void ReflectAttr(DependencyAttrItem relatedAttrItem, DependencyAttributeChangedEventArgs args) {
        RefreshAttrs(ArgVal(0));
    }

    private void RefreshAttrs(string inputType) {
        while (attributes.Count > 1)
            attributes.RemoveAt(1);

        switch (inputType) {
            default:
            case ColorInput.String:
                attributes.Add(new AttrItem("ARGB String", "255, 255, 255, 255", this, "ARGB"));
                break;

            case ColorInput.Split:
                attributes.Add(new AttrItem("Red", this));
                attributes.Add(new AttrItem("Green", this));
                attributes.Add(new AttrItem("Blue", this));
                attributes.Add(new AttrItem("Alpha", this));
                break;
        }
    }

    [JsonIgnore, XmlIgnore]
    protected override string nodeName => "Set Color Filter";

    private static readonly string[] RGBA = ["red", "green", "blue", "alpha"];

    private string[] GetRGBAComponents() {
        string colorInput = ArgVal(0);

        if (colorInput == ColorInput.String) {
            string[] argb = ArgVal(1).Split(',');
            if (argb.Length == 4) return [argb[1], argb[2], argb[3], argb[0]];

        } else if (colorInput == ColorInput.Split) {
            return [ArgVal(1), ArgVal(2), ArgVal(3), ArgVal(4)];
        }

        return [];
    }

    [JsonIgnore, XmlIgnore]
    protected override string nodeAttrs {
        get {
            string[] comps = GetRGBAComponents();
            if (comps.Length != 4) return ": invalid";

            List<string> usedComps = [];

            for (int i = 0; i < 4; i++) {
                string trimmedComp = comps[i].Trim();
                comps[i] = trimmedComp;

                if (!NilOrEmpty(trimmedComp))
                    usedComps.Add($"{RGBA[i]} to {trimmedComp}");
            }

            if (usedComps.Count == 4)
                return $" to color ({comps[3]}, {comps[0]}, {comps[1]}, {comps[2]})";
            else if (usedComps.Count == 0)
                return ": unchanged";

            return $": {string.Join(", ", usedComps)}";
        }
    }

    protected override string LuaArgList() {
        string[] comps = GetRGBAComponents();

        for (int i = 0; i < comps.Length; i++) {
            if (comps[i] == "") comps[i] = "nil";
            else comps[i] = comps[i].Trim();
        }

        return string.Join(", ", comps);
    }

    protected override string GetNodeAttrs() => nodeAttrs;

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "SetColorFilter";

    [JsonIgnore, XmlIgnore]
    protected override bool isSetter => true;

    protected override void CheckErrors(List<MessageBase> errors) {
        switch (ArgVal(0)) {
            case ColorInput.String:
                if (ArgVal(1).Split(',').Length != 4)
                    errors.Add(new CustomMessage("ARGB String should have 4 comma-separated components (nil/empty strings/variables allowed).", 0, this));
                break;

            case ColorInput.Split:
                break;

            default:
                errors.Add(new CustomMessage("Color Input type must be \"ARGB String\" or \"Split RGBA\".", 0, this));
                break;
        }
    }

    public override object Clone() {
        var n = new TextSetColorFilter(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
