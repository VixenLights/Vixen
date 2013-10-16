using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VersionControl
{
   public static class Extensions
    {
       public static bool  CanRemove(this TreeNode node) {

           return (node.Nodes.Count == 0 && node.Tag == null);

       }
       public static void ClearEmptyChildren(this TreeNode node) {
           List<TreeNode> removeKeys = new List<TreeNode>();
           foreach (TreeNode item in node.Nodes)
           {
               ClearEmptyChildren(item);
               if (item.CanRemove())
                   removeKeys.Add(item );

           }
           removeKeys.ForEach(key => node.Nodes.Remove(key));
       }
    }
}
