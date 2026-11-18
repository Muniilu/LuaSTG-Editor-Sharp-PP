using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.Linq;
using LuaSTGEditorSharp.EditorData.Node.NodeAttributes;
using Newtonsoft.Json;

namespace LuaSTGEditorSharp.EditorData.Node.General
{
    [Serializable, NodeIcon("if.png")]
    [RCInvoke(0)]
    public class IfNode : TreeNode
    {
        [JsonConstructor]
        private IfNode() : base() { }

        public IfNode(DocumentData workSpaceData)
            : this(workSpaceData, "") { }

        public IfNode(DocumentData workSpaceData, string code)
            : base(workSpaceData)
        {
            //attributes.Add(new AttrItem("Condition", this) { AttrInput = code });
            Condition = code;
        }

        [JsonIgnore, NodeAttribute, XmlAttribute("Condition")]
        public string Condition
        {
            get => DoubleCheckAttr(0).attrInput;
            set => DoubleCheckAttr(0).attrInput = value;
        }

        public override bool CustomValidateChild(TreeNode child, int index, bool isNewParent) {
            if (index > 0) { // Then node must be 1st
                if (child is IfElse)
                    return index == Children.Count;

                if (child is IfElseIf) {
                    int elseID = Children.ToList().FindIndex(c => c is IfElse);
                    return elseID == -1 || index <= elseID;
                }
            }

            return false;
        }

        public IEnumerable<string> BaseToLua(int spacing, IEnumerable<TreeNode> children)
        {
            return base.ToLua(spacing, children);
        }

        public override IEnumerable<string> ToLua(int spacing)
        {
            string sp = Indent(spacing);
            var i = GetLogicalChildren().OrderBy((s) => (s as IIfChild)?.Priority ?? 0);
            List<TreeNode> t = new(i);

            yield return sp + "if " + Macrolize(0);
            foreach (var a in BaseToLua(spacing, i))
            {
                yield return a;
            }
            yield return sp + "end\n";
        }

        public override IEnumerable<Tuple<int,TreeNode>> GetLines()
        {
            yield return new Tuple<int, TreeNode>(1, this);
            foreach(Tuple<int,TreeNode> t in GetChildLines())
            {
                yield return t;
            }
            yield return new Tuple<int, TreeNode>(1, this);
        }

        public override string ToString()
        {
            return $"If ({NonMacrolize(0)})";
        }

        public override object Clone()
        {
            var n = new IfNode(parentWorkSpace);
            n.DeepCopyFrom(this);
            return n;
        }
    }
}
