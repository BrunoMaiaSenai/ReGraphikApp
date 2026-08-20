using ReGraphik.ViewModels;
using System.Windows.Controls;

namespace ReGraphik.Views.Controls
{
    public partial class MapaControl : UserControl
    {
        public MapaControl()
        {
            InitializeComponent();
            DataContext = new MapaViewModel();
        }
    }
}