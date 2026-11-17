using LuaSTGEditorSharp.EditorData.Message;
using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using LuaSTGEditorSharp.EditorData.Node.Object;
using LuaSTGEditorSharp.EditorData.Node.Render;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Xml.Serialization;

namespace LuaSTGEditorSharp.EditorData.Node.TextInterface;

[IgnoreAttributesParityCheck]
public abstract class TextInterfaceNode : TreeNode {
    [JsonConstructor]
    public TextInterfaceNode() : this(null) { }
    public TextInterfaceNode(DocumentData workspace) : base(workspace) {
        PropertyChanged += OnPropertyChanged;
    }

    [JsonIgnore, XmlIgnore]
    protected virtual string nodeName => "Unspecified Text Interface Node";

    [JsonIgnore, XmlIgnore]
    protected virtual string nodeAttrs => "";

    [JsonIgnore, XmlIgnore]
    protected virtual string funcName => "Error";

    [JsonIgnore, XmlIgnore]
    protected virtual bool allowChaining => true;

    [JsonIgnore, XmlIgnore]
    protected virtual bool isSetter => false;

    [JsonIgnore, XmlIgnore]
    protected virtual bool allArgsOptional => false;

    protected void Log(string s) {
        (Application.Current.MainWindow as IMainWindow).Log($"[{GetType().Name}] {s}");
    }

    private void OnPropertyChanged(object sender, PropertyChangedEventArgs e) {
        RefreshAttributes();
    }

    protected void RefreshAttributes() {
        if (Parent is TextInterfaceNode) { // in chain
            if (attributes.Count > 0 && attributes[0].Name == "Container") {
                attributes.RemoveAt(1);
                attributes.RemoveAt(0);
            }

        } else { // not in chain
            if (attributes.Count == 0 || attributes[0].Name != "Container") {
                attributes.Insert(0, new AttrItem("Container", this));
                attributes.Insert(1, new AttrItem("Text Name", this));
            }
        }
    }

    public override bool CustomValidateChild(TreeNode child, int index, bool isNewParent) {
        int chainTermNodeID = Children.ToList().FindIndex(c => !((TextInterfaceNode)c).allowChaining);

        return child is TextInterfaceNode and not TextCreate // only TextInterfaceNodes
            && (isNewParent ? child.Children.Count == 0 : Parent is not TextInterfaceNode) // no grandchildren
            && (((TextInterfaceNode)child).allowChaining || index == Children.Count) // nodes that can't be chained must come last
            && !(chainTermNodeID != -1 && index > chainTermNodeID); // can't insert a node after a chain-stopping node
    }

    public override string ToString() {
        if (Parent is null) return GetType().Name; // ValidateChild only

        // Certain TreeNode logs happen at really inopportune times & I kinda give up
        // fixing them all in a sensible way :/
        RefreshAttributes();

        if (Parent is TextInterfaceNode) 
            return $"... and {nodeName}{GetNodeAttrs()}";

        else if (attributes.Count > 0)
            return $"{nodeName} {(isSetter ? "of " : "")}{GetTextObjName()}{GetNodeAttrs()}";

        else
            return $"[ERROR] Uninitialized {GetType().Name}";
    }

    protected virtual string GetNodeAttrs() {
        return " " + (isSetter ? $"to {nodeAttrs}" : nodeAttrs);
    }

    private string GetTextObjName() {
        if (Parent is TextInterfaceNode) {
            return "(specified in parent)";

        } else {
            string container = attributes[0].AttrInput;
            string textName = attributes[1].AttrInput;
            if (textName == "") textName = "unspecified Text object";

            if (container == "")
                return textName;
            else
                return $"{textName} (from {container})";
        }
    }

    protected virtual string LuaArgList() {
        var attrList = (Parent is TextInterfaceNode ? attributes : attributes.Skip(2))
            .Select(a => a.AttrInput).ToList();

        if (allArgsOptional) {
            var args = new List<string>(attrList.Count);

            for (int i = attrList.Count - 1; i >= 0; i--) {
                string arg = attrList[i];

                if (!NilOrEmpty(arg)) args.Add(arg);
                else if (args.Count > 0) args.Add("nil");
            }

            args.Reverse();
            return string.Join(", ", args);
        }

        return string.Join(", ", attrList);
    }

    public override IEnumerable<string> ToLua(int spacing) {
        string funcCall = $":{funcName}({LuaArgList()})\n";

        if (Parent is TextInterfaceNode) {
            yield return Indent(spacing) + funcCall;

        } else {
            string container = attributes[0].AttrInput;
            string textName = attributes[1].AttrInput;
            string textObjRef = (container == "") ? textName : container + "." + textName;
            yield return Indent(spacing) + textObjRef + funcCall;

            int childPadding = textObjRef.Length;
            foreach (var childLua in GetChildLua(spacing))
                yield return "".PadLeft(childPadding) + childLua;
        }
    }

    protected IEnumerable<string> GetChildLua(int spacing) {
        return base.ToLua(spacing);
    }

    public AttrItem Arg(int index) {
        return attributes[Parent is TextInterfaceNode ? index : index + 2];
    }

    public string ArgVal(int index) {
        return Arg(index).AttrInput;
    }

    public int ArgCount() {
        return Parent is TextInterfaceNode ? attributes.Count : attributes.Count - 2;
    }

    public bool NilOrEmpty(string val) {
        return val == "" || val == "nil";
    }

    public string DefaultedValue(int index, string def) {
        string val = ArgVal(index);
        return NilOrEmpty(val) ? def : val;
    }

    private static readonly Type[] onFrameTypes = [
        typeof(CallBackFunc), typeof(BGOnFrame), // frame
        typeof(OnRender), typeof(OnRenderCustom), typeof(BGOnRender), // render
    ];

    protected bool HasOnFrameAncestor() {
        TreeNode n = this;

        while ((n = n.Parent) != null)
            if (onFrameTypes.Contains(n.GetType()))
                return true;

        return false;
    }

    [JsonIgnore, XmlIgnore]
    protected override bool EnableParityCheck => false;

    public override IEnumerable<Tuple<int, TreeNode>> GetLines() {
        yield return new Tuple<int, TreeNode>(1, this);
    }

    protected virtual void CheckErrors(List<MessageBase> errors) { }

    public override List<MessageBase> GetMessage() {
        List<MessageBase> errors = [];
        RefreshAttributes();

        if (Parent is not TextInterfaceNode && string.IsNullOrEmpty(attributes[1].AttrInput))
            errors.Add(new ArgNotNullMessage("Text Name", 0, this));

        CheckErrors(errors);
        return errors;
    }
}
