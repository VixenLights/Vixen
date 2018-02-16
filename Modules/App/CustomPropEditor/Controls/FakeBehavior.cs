using System;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace VixenModules.App.CustomPropEditor.Controls
{
    public class FakeBehavior: Behavior<TreeView>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            Console.Out.WriteLine("Fake behavior attached!");
        }

        
    }
}
