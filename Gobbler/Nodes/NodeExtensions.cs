using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gobbler
{
    public static class NodeExtensions
    {
        public static bool IsParentOf(this INode root, INode node)
        {
            if (root.Nodes != null)
                return root.Nodes.Contains(node);
            else
                return false;
        }

        public static INode ParentOf(this INode current, INode node)
        {
            if (current.Nodes == null)
                return null;

            if (current.Nodes.Count == 0)
                return null;

            if (current.IsParentOf(node))
                return current;

            foreach (INode n in current.Nodes)
            {

                INode result = n.ParentOf(node);
                if (result != null)
                    return result;


            }
            return null;
        }

        public static bool IsUserNode(this INode node)
        {
            NodeKind[] kinds = { NodeKind.Folder, NodeKind.Feed };
            return kinds.Contains(node.Kind);
    
        }

        

        public static void Delete(this INode root, INode node)
        {

            INode parent = root.ParentOf(node);
            if (parent != null)
                parent.Nodes.Remove(node);
        }


    }
}
