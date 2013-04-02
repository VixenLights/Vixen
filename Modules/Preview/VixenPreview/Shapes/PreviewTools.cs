using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Vixen.Data.Flow;
using Vixen.Module;
using Vixen.Module.OutputFilter;
using Vixen.Services;
using Vixen.Sys;
using Vixen.Sys.Output;
namespace VixenModules.Preview.VixenPreview.Shapes
{
    public class ComboBoxItem
    {
        public string Text { get; set; }
        public object Value { get; set; }

        public ComboBoxItem(string text, object value)
        {
            Text = text;
            Value = value;
        }

        public override string ToString()
        {
            return Text;
        }
    }

    class PreviewTools
    {

        static double Perimeter(PreviewPoint p1, PreviewPoint p2) 
        {
            double p;
            double a;
            double b;
            a = Math.Abs(p2.Y - p1.Y);
            b = Math.Abs(p2.X - p1.X);
            p = Math.PI * ((3 * (a + b)) - Math.Sqrt((3 * a + b) * (a + 3 * b)));
            return p;
        }

        // 
        // Add the root nodes to the Display Element tree
        //
        static public void PopulateElementTree(TreeView tree)
        {
            foreach (ElementNode channel in VixenSystem.Nodes.GetRootNodes())
            {
                AddNodeToElementTree(tree.Nodes, channel);
            }
        }

        // 
        // Add each child Display Element or Display Element Group to the tree
        // 
        static private void AddNodeToElementTree(TreeNodeCollection collection, ElementNode channelNode)
        {
            TreeNode addedNode = new TreeNode();
            addedNode.Name = channelNode.Id.ToString();
            addedNode.Text = channelNode.Name;
            addedNode.Tag = channelNode;

            collection.Add(addedNode);

            foreach (ElementNode childNode in channelNode.Children)
            {
                AddNodeToElementTree(addedNode.Nodes, childNode);
            }
        }

        static public List<Point> GetArcPoints(double Width, double Height, double NumPoints)
        {
            List<Point> points = new List<Point>();

            double C_x = Width / 2;
            double C_y = Height;
            double radianIncrement = Math.PI / (NumPoints - 1);
            for (double t = Math.PI; t <= 2 * Math.PI; t += radianIncrement)
            {
                double X = C_x + (Width / 2) * Math.Cos(t);
                double Y = C_y + (Height) * Math.Sin(t);
                points.Add(new Point((int)X, (int)Y));
            }
            return points;
        }

        static public List<Point> GetEllipsePoints(
            double leftOffset,
            double topOffset,
            double Width, 
            double Height, 
            double totalPoints, 
            double degrees)
        {

            //const double C_x = 10, C_y = 20, w = 40, h = 50;
            //for (double t = 0; t <= 2 * pi; t += 0.01)
            //{
            //    double X = C_x + (w / 2) * cos(t);
            //    double Y = C_y + (h / 2) * sin(t);
            //    // Do what you want with X & Y here 
            //}
            List<Point> points = new List<Point>();


            //double degrees = 360;
            double totalRadians = (degrees * Math.PI) / 180;
            double numPoints = (totalPoints / 2);
            double C_x = Width / 2;
            double C_y = Height / 2;
            double radianIncrement;
            if (degrees <= 180)
            {
                radianIncrement = Math.PI / (numPoints - 1);
                for (double t = 0; t <= totalRadians; t += radianIncrement)
                {
                    double X = C_x + (Width / 2) * Math.Cos(t) + leftOffset;
                    double Y = C_y + (Height / 2) * Math.Sin(t) + topOffset;
                    points.Add(new Point((int)X, (int)Y));
                }
                return points;
            }
            else
            {
                radianIncrement = (Math.PI * 2) / totalPoints;
                for (double t = 0; t < totalRadians; t += radianIncrement)
                {
                    double X = (C_x + (Width / 2) * Math.Cos(t)) + leftOffset;
                    double Y = (C_y + (Height / 2) * Math.Sin(t)) + topOffset;
                    points.Add(new Point((int)X, (int)Y));
                }
                return points;
            }
        }

    }
}
