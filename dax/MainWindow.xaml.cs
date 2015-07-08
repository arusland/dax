using dax.Core.Document;
using System.Windows;

namespace dax
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DaxDocument doc = DaxDocument.Load(@"d:\WORK\MyProjects\dax\template.xml");
        }
    }
}
