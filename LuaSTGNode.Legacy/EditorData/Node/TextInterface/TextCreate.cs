using LuaSTGEditorSharp.EditorData.Message;
using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[NodeIcon("text_interface_create.png")]
public class TextCreate : TextSetText {
    [JsonConstructor]
    private TextCreate() : base() { }

    public TextCreate(DocumentData workspace) : base(workspace) {
        var fontType = new DependencyAttrItem("Font Type", FontType.Direct, this, "text_interface_font_type");

        attributes.Add(fontType);
        attributes.Add(new AttrItem("Font Size", this));
        attributes.Add(new AttrItem("Pre-Load", this, "bool"));
        RefreshAttrs(fontType);
    }

    protected static class FontType {
        public const string Direct = "Direct";
        public const string FontDesc = "Custom Desc";
    }

    public override string ToString() {
        if (Parent is null) return GetType().Name; // ValidateChild only

        string container = attributes[0].AttrInput;
        string textName = attributes[1].AttrInput;
        if (textName == "") textName = "(unnamed)";
        if (container != "") textName += $" (in {container})";
        string outStr = $"Create Text object {textName} w/ ";

        switch (ArgVal(0)) {
            case ContentType.Direct:
                string value = ArgVal(1);
                outStr += $"value {(value == "" ? "\"\"" : value)}, ";
                break;

            case ContentType.LabelPath:
                outStr += $"path {ArgVal(1)}, ";
                break;

            case ContentType.Template:
                if (Arg(3).AttrCap == "Arg Count")
                    outStr += $"template {ArgVal(2)}, ";
                else
                    outStr += "template, ";
                break;
        }

        int fontTypeID = FindArgID("Font Type");
        switch (attributes[fontTypeID].AttrInput) {
            case FontType.Direct:
                outStr += $"font(s) {attributes[fontTypeID + 1].AttrInput}, ";
                break;

            case FontType.FontDesc:
                outStr += "custom font desc, ";
                break;

            default:
                break;
        }

        return outStr + $"font size {attributes[FindArgID("Font Size")].AttrInput}";
    }

    protected override void RefreshAttrs(AttrItem updatedAttr, int delUpTo = 99) {
        int fontTypeID = FindArgID("Font Type");

        if (fontTypeID == -1) {
            base.RefreshAttrs(updatedAttr);

        } else if (updatedAttr != attributes[fontTypeID]) {
            base.RefreshAttrs(updatedAttr, fontTypeID - 1);

        } else {
            int fontSizeID = FindArgID("Font Size");

            for (int i = fontSizeID - 1; i > fontTypeID; i--)
                attributes.RemoveAt(i);

            switch (attributes[fontTypeID].AttrInput) {
                case FontType.Direct:
                    attributes.Insert(fontTypeID + 1, new AttrItem("Value", this));
                    break;

                case FontType.FontDesc:
                    attributes.Insert(fontTypeID + 1, new AttrItem("Default Font", this));
                    attributes.Insert(fontTypeID + 2, new AttrItem("JP Font", this));
                    attributes.Insert(fontTypeID + 3, new AttrItem("CN Font", this));
                    attributes.Insert(fontTypeID + 4, new AttrItem("JP Font Scale", "1", this));
                    attributes.Insert(fontTypeID + 5, new AttrItem("CN Font Scale", "1", this));
                    break;

                default:
                    break;
            }
        }
    }

    public override IEnumerable<string> ToLua(int spacing) {
        string indent = Indent(spacing);
        string container = attributes[0].AttrInput;
        string textName = attributes[1].AttrInput;

        int fontTypeID = FindArgID("Font Type");

        if (attributes[fontTypeID].AttrInput == FontType.FontDesc) {
            string defFont  = attributes[fontTypeID + 1].AttrInput;
            string jpFont   = attributes[fontTypeID + 2].AttrInput;
            string cnFont   = attributes[fontTypeID + 3].AttrInput;
            string jpScale  = attributes[fontTypeID + 4].AttrInput;
            string cnScale  = attributes[fontTypeID + 5].AttrInput;
            if (jpScale == "") jpScale = "nil";
            if (cnScale == "") cnScale = "nil";

            yield return indent + $"local _lastFontDesc = FontDesc(fonts.{defFont}, fonts.{jpFont}, fonts.{cnFont}, {jpScale}, {cnScale})\n\n";
        }

        string lhs = $"{(container == "" ? textName : container + "." + textName)} = ";
        yield return indent + lhs + $"Text({LuaArgList()})\n";

        int childPadding = lhs.Length;
        foreach (var childLua in GetChildLua(spacing))
            yield return "".PadLeft(childPadding) + childLua;
    }

    protected override string LuaArgList() {
        string argList = base.LuaArgList();
        int fontTypeID = FindArgID("Font Type");
        string fontSize = attributes[FindArgID("Font Size")].AttrInput;
        string preLoad = attributes[FindArgID("Pre-Load")].AttrInput;

        if (attributes[fontTypeID].AttrInput == FontType.Direct)
            argList += $", {attributes[fontTypeID + 1].AttrInput}";
        else
            argList += ", _lastFontDesc";

        argList += $", {fontSize}";
        if (preLoad != "") argList += $", {preLoad}";

        return argList;
    }

    protected override void CheckErrors(List<MessageBase> errors) {
        base.CheckErrors(errors);
        string fontType = attributes[FindArgID("Font Type")].AttrInput;

        if (fontType != FontType.Direct && fontType != FontType.FontDesc)
            errors.Add(new CustomMessage("Font Type must be \"Direct\" or \"Custom Desc\".", 0, this));
    }

    // extending SetText may have been a janky idea...
    private int FindArgID(string name) {
        for (int i = attributes.Count - 1; i >= 0; i--)
            if (attributes[i].Name == name)
                return i;

        return -1;
    }

    public override object Clone() {
        var n = new TextCreate(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
