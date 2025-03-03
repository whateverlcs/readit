using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace readit.Views
{
    /// <summary>
    /// Interaction logic for LeituraCapituloView.xaml
    /// </summary>
    public partial class LeituraCapituloView : UserControl
    {
        public LeituraCapituloView()
        {
            InitializeComponent();
        }

        private void VoltarTopo_Click(object sender, RoutedEventArgs e)
        {
            MainScrollViewer.ScrollToTop();
        }

        private void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is Grid grid)
            {
                var clipGeometry = new RectangleGeometry
                {
                    RadiusX = 15,
                    RadiusY = 15,
                    Rect = new Rect(0, 0, grid.ActualWidth, grid.ActualHeight)
                };

                grid.Clip = clipGeometry;

                // Atualiza o clip dinamicamente quando o tamanho do grid mudar
                grid.SizeChanged += (s, args) =>
                {
                    clipGeometry.Rect = new Rect(0, 0, grid.ActualWidth, grid.ActualHeight);
                };
            }
        }
    }
}