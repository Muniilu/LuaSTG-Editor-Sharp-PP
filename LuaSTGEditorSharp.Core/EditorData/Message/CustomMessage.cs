using System.Windows;
using LuaSTGEditorSharp.EditorData.Interfaces;

namespace LuaSTGEditorSharp.EditorData.Message {
    public class CustomMessage : MessageBase {
        public string MessageContent { get; set; }
        public override DocumentData SourceDoc { get => (Source as TreeNode).parentWorkSpace; }

        public CustomMessage(string content, int level, IMessageThrowable source) : base(level, source) {
            MessageContent = content;
        }

        public override string ToString() {
            return MessageContent;
        }

        public override object Clone() {
            return new CustomMessage(MessageContent, WarningLevel, Source);
        }

        public override void Invoke() {
            (Application.Current.MainWindow as IMainWindow).Reveal(Source as TreeNode);
        }
    }
}
