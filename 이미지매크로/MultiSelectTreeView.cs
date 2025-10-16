using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace 이미지매크로
{
    public class MultiSelectTreeView : TreeView
    {
        public List<TreeNode> SelectedNodes { get; private set; } = new List<TreeNode>();

        protected override void OnAfterSelect(TreeViewEventArgs e)
        {
            base.OnAfterSelect(e);
            if ((ModifierKeys & Keys.Control) == Keys.Control)
            {
                if (!SelectedNodes.Contains(e.Node))
                    SelectedNodes.Add(e.Node);
            }
            else if ((ModifierKeys & Keys.Shift) == Keys.Shift)
            {
                if (SelectedNodes.Count > 0)
                {
                    TreeNode first = SelectedNodes[0];
                    SelectedNodes.Clear();
                    bool selecting = false;
                    // 단순히 루트 노드들의 순서로 처리 (복잡한 계층 구조는 추가 구현 필요)
                    foreach (TreeNode node in this.Nodes)
                    {
                        if (node == first || node == e.Node)
                        {
                            selecting = !selecting;
                            SelectedNodes.Add(node);
                        }
                        else if (selecting)
                        {
                            SelectedNodes.Add(node);
                        }
                    }
                }
                else
                {
                    SelectedNodes.Add(e.Node);
                }
            }
            else
            {
                SelectedNodes.Clear();
                SelectedNodes.Add(e.Node);
            }
            Invalidate();
        }

        protected override void OnDrawNode(DrawTreeNodeEventArgs e)
        {
            if (SelectedNodes.Contains(e.Node))
            {
                e.Graphics.FillRectangle(Brushes.LightBlue, e.Bounds);
            }
            e.DrawDefault = true;
            base.OnDrawNode(e);
        }

        public MultiSelectTreeView()
        {
            this.DrawMode = TreeViewDrawMode.OwnerDrawText;
        }
    }
}
