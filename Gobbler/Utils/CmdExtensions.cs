using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Input;

namespace Gobbler
{
    public static class CmdExtensions
    {
        public static CommandBinding Command(this UserControl control, string name)
        {
            ICommand command = new RoutedCommand(name, typeof(UserControl));
            CommandBinding binding = new CommandBinding(command);
            control.CommandBindings.Add(binding);
            return binding;
        }
        
    }
}
