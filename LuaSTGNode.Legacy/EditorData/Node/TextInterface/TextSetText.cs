using LuaSTGEditorSharp.EditorData.Message;
using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

// Known bug: some operations may crash when SharpX is closing with an unsaved doc
//            and it for some reason tries to undo unsaved changes (??)

[NodeIcon("richtextsettext.png")]
public class TextSetText : TextInterfaceNode {
    [JsonConstructor]
    protected TextSetText() : base() { }
    public TextSetText(DocumentData workspace) : base(workspace) {
        AttrItem ct = new DependencyAttrItem("Content Type", ContentType.Direct, this, "text_interface_content_type");
        attributes.Add(ct);
        RefreshAttrs(ct);
    }

    protected static class ContentType {
        public const string Direct = "Direct";
        public const string LabelPath = "Label Path";
        public const string Template = "Template";
    }

    [JsonIgnore, XmlIgnore]
    protected override string nodeName => "Set Text";

    [JsonIgnore, XmlIgnore]
    protected override string funcName => "SetText";

    [JsonIgnore, XmlIgnore]
    protected override bool isSetter => true;

    [JsonIgnore, XmlIgnore]
    protected override string nodeAttrs {
        get {
            switch (ArgVal(0)) {
                case ContentType.Direct:
                    string value = ArgVal(1);
                    return value == "" ? "\"\"" : value;

                case ContentType.LabelPath:
                    return $"path {ArgVal(1)}";

                case ContentType.Template:
                    if (ArgCount() < 4 || !int.TryParse(ArgVal(3), out _))
                        return "template";
                    string argList = GetTemplateArgList();

                    if (argList == "")
                        return $"template {ArgVal(2)}";
                    else
                        return $"template {ArgVal(2)} with arguments {argList}";

                default:
                    return "";
            }
        }
    }

    protected override string LuaArgList() {
        switch (ArgVal(0)) {
            default:
            case ContentType.Direct:
                string value = ArgVal(1);
                return value == "" ? "\"\"" : value;

            case ContentType.LabelPath:
                return ToLabelPath(ArgVal(1));

            case ContentType.Template:
                string template = ArgVal(2);
                if (ArgVal(1) == ContentType.LabelPath)
                    template = ToLabelPath(template);

                string argList = GetTemplateArgList();
                if (argList == "") return $"LabelTemplate({template})";
                else return $"LabelTemplate({template}, {argList})";
        }
    }

    private string GetTemplateArgList() {
        if (ArgCount() < 4) return "";

        AttrItem argCount = Arg(3);
        var args = attributes.Skip(attributes.IndexOf(argCount) + 1).Take(int.Parse(argCount.AttrInput));
        return string.Join(", ", args.Select(a => a.AttrInput == "" ? "\"\"" : a.AttrInput));
    }

    private int CountFormatSpecifiers(string text) {
        return Regex.Matches(text, @"%(?!%)[-+ #0]*\d*(?:\.\d*)?[cdiouxXaAeEfgGqs]").Count;
    }

    public override void ReflectAttr(DependencyAttrItem relatedAttrItem, DependencyAttributeChangedEventArgs args) {
        RefreshAttrs(relatedAttrItem);
    }

    protected virtual void RefreshAttrs(AttrItem updatedAttr, int delUpTo = 99) {
        string attrName = updatedAttr.AttrCap;
        string attrVal = updatedAttr.AttrInput;

        if (attrName == "Format String") {
            int fmtSpecifiers = CountFormatSpecifiers(attrVal);
            if (fmtSpecifiers > 0) Arg(3).AttrInput = fmtSpecifiers.ToString();
            return;
        }

        int index = attributes.IndexOf(updatedAttr);
        int indexMax = Math.Min(delUpTo, attributes.Count - 1);

        for (int i = indexMax; i > index; i--)
            attributes.RemoveAt(i);

        if (attrName == "Arg Count") {
            if (!int.TryParse(attrVal, out int nArgs)) nArgs = 0;
            for (int i = 1; i <= Math.Min(nArgs, 20); i++)
                attributes.Insert(index + i, new AttrItem($"Arg #{i}", this));

            int fmtSpecifiers = CountFormatSpecifiers(ArgVal(2));
            if (fmtSpecifiers > 0 && fmtSpecifiers != nArgs)
                updatedAttr.AttrInput = fmtSpecifiers.ToString();

        } else {
            switch (attrVal) {
                case ContentType.Direct:
                    if (attrName == "Template Type")
                        attributes.Insert(index + 1, new AttrItem("Format String", this));
                    else
                        attributes.Insert(index + 1, new AttrItem("Value", this));

                    AddTemplateArgCount(index + 2, attrName);
                    break;

                case ContentType.LabelPath:
                    attributes.Insert(index + 1, new AttrItem("Label Path", this));
                    AddTemplateArgCount(index + 2, attrName);
                    break;

                case ContentType.Template:
                    if (attrName == "Content Type") {
                        attributes.Insert(index + 1, new DependencyAttrItem("Template Type", ContentType.Direct, this, "text_interface_template_type"));
                        attributes.Insert(index + 2, new DependencyAttrItem("Format String", this));
                        attributes.Insert(index + 3, new DependencyAttrItem("Arg Count", this));
                    }
                    break;

                default:
                    break;
            }
        }
    }

    private void AddTemplateArgCount(int index, string attrName) {
        if (attrName == "Template Type")
            attributes.Insert(index, new DependencyAttrItem("Arg Count", this));
    }

    private static readonly HashSet<string> GlobalPathParts = [
        "gameplay", "stageFormations", "menu", "bg_text", "main", "manual",
        "music_text", "warning", "rpc", "story", "vs"
    ];

    private string ToLabelPath(string path) {
        string[] parts = [.. path.Split('.').Select(p => GlobalPathParts.Contains(p) ? p : $"\"{p}\"")];
        return $"LabelPath({string.Join(", ", parts)})";
    }

    private void ValidateLabelPath(string path, List<MessageBase> errors) {
        string[] parts = path.Split('.');

        if (parts.Length < 3) {
            errors.Add(new CustomMessage("Label Path must have 3 or more period-separated parts.", 0, this));

        } else {
            foreach(string part in parts) {
                if (part.Length == 0) {
                    errors.Add(new CustomMessage("Label Path part cannot be empty.", 0, this));
                    return;
                }
            }
        }
    }

    private bool ValidateContentType(string type, int valIndex, string defaultError, List<MessageBase> errors) {
        switch (type) {
            case ContentType.Direct:
                return true;

            case ContentType.LabelPath:
                ValidateLabelPath(ArgVal(valIndex), errors);
                return true;

            default:
                errors.Add(new CustomMessage(defaultError, 0, this));
                return false;
        }
    }

    protected override void CheckErrors(List<MessageBase> errors) {
        var type = ArgVal(0);

        if (type == ContentType.Template) {
            bool validType = ValidateContentType(ArgVal(1), 2, "Template Type must be \"Direct\" or \"Label Path\".", errors);

            if (validType && (!int.TryParse(ArgVal(3), out int cnt) || cnt < 0 || cnt > 20))
                errors.Add(new CustomMessage("Arg Count must be an integer between 0 and 20.", 0, this));

        } else {
            ValidateContentType(type, 1, "Content Type must be \"Direct\", \"Label Path\" or \"Template\".", errors);
        }
    }

    public override object Clone() {
        var n = new TextSetText(parentWorkSpace);
        n.DeepCopyFrom(this);
        return n;
    }
}
