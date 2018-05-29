using System;
using System.Linq;
using System.Windows;
using ICSharpCode.TreeView;
using Mono.Cecil;

namespace ICSharpCode.ILSpy.TreeNodes.Analyzer
{
	[ExportContextMenuEntryAttribute(Header = "クリップボードにコピー", Icon = "images/Search.png")]
	internal sealed class AnalyzeContextMenuEntryCopyToClipboard : IContextMenuEntry
	{
		public bool IsVisible(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return context.Reference != null && context.Reference.Reference is MemberReference;
			return context.SelectedTreeNodes.All(n => n is IMemberTreeNode);
		}

		public bool IsEnabled(TextViewContext context)
		{
			if (context.SelectedTreeNodes == null)
				return context.Reference != null && context.Reference.Reference is MemberReference;
			foreach (IMemberTreeNode node in context.SelectedTreeNodes) {
				if (!(node.Member is TypeDefinition
				      || node.Member is FieldDefinition
				      || node.Member is MethodDefinition
				      || AnalyzedPropertyTreeNode.CanShow(node.Member)
				      || AnalyzedEventTreeNode.CanShow(node.Member)))
					return false;
			}

			return true;
		}

		public void Execute(TextViewContext context)
		{
			if (context.SelectedTreeNodes != null) {
				foreach (IMemberTreeNode node in context.SelectedTreeNodes) {
					CopyInfo(node.Member);
				}
			} else if (context.Reference != null && context.Reference.Reference is MemberReference) {
				if (context.Reference.Reference is MemberReference)
					CopyInfo((MemberReference)context.Reference.Reference);
				// TODO: implement support for other references: ParameterReference, etc.
			}
		}

        /// <summary>
        /// Analyzeツリーに表示されているノードを再帰的に呼び出し
        /// 各ノードの表示文字列を取得します。
        /// </summary>
        /// <param name="pre"></param>
        /// <param name="node"></param>
        /// <returns></returns>
        private static string getNodes(String pre, SharpTreeNode node)
        {
            String t = String.Empty;

            if (node.Children.Count == 0) return "";
            pre += " ";
            foreach (var item in node.Children)
            {
                t += pre + item.Text + "\r\n";
                t += getNodes(pre, item);
            }
            return t;
        }
        
		public static void CopyInfo(MemberReference member)
		{
            String copyText = String.Empty;

            // Analyzeツリーに表示されている文字列を取得
            var v = getNodes(String.Empty, AnalyzerTreeView.Instance.Root);

            // 上記文字列をクリップボードにコピー
            Clipboard.SetText(v);
            
		}
	}
}
