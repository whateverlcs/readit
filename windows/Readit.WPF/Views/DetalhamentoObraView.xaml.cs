using Readit.Core.Domain;
using Readit.WPF.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using WpfAnimatedGif;

namespace Readit.WPF.Views
{
    /// <summary>
    /// Interaction logic for DetalhamentoObraView.xaml
    /// </summary>
    public partial class DetalhamentoObraView : UserControl
    {
        public DetalhamentoObraView()
        {
            InitializeComponent();

            ImageBehavior.SetAnimatedSource(GifImage, GifImage.Source);
        }

        private void Star_MouseEnter(object sender, MouseEventArgs e)
        {
            if (sender is TextBlock star && star.DataContext is StarRating starRating)
            {
                if (DataContext is DetalhamentoObraViewModel viewModel)
                {
                    viewModel.StarHovered(starRating.StarIndex);
                }
            }
        }

        private void Star_MouseLeave(object sender, MouseEventArgs e)
        {
            if (DataContext is DetalhamentoObraViewModel viewModel)
            {
                viewModel.StarHoverExit();
            }
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