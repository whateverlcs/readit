using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfAnimatedGif;

namespace readit.Views
{
    /// <summary>
    /// Interaction logic for PaginaInicialView.xaml
    /// </summary>
    public partial class PaginaInicialView : UserControl
    {
        public PaginaInicialView()
        {
            InitializeComponent();

            ImageBehavior.SetAnimatedSource(GifImage, GifImage.Source);
        }

        private void OnGridLoaded(object sender, RoutedEventArgs e)
        {
            if (sender is Grid grid)
            {
                ApplyRoundedClip(grid);

                // Escuta mudanças de visibilidade
                grid.IsVisibleChanged += (s, args) =>
                {
                    if (grid.Visibility == Visibility.Visible)
                    {
                        // Aguarda o layout ser recalculado antes de reaplicar o Clip
                        Dispatcher.InvokeAsync(() => ApplyRoundedClip(grid));
                    }
                };
            }
        }

        private void ApplyRoundedClip(Grid grid)
        {
            if (grid == null) return;

            var clipGeometry = new PathGeometry();
            UpdateClipGeometry(grid, clipGeometry);

            grid.Clip = clipGeometry;

            // Atualiza o clip dinamicamente quando o tamanho do grid mudar
            grid.SizeChanged += (s, args) =>
            {
                UpdateClipGeometry(grid, clipGeometry);
            };
        }

        private void UpdateClipGeometry(Grid grid, PathGeometry clipGeometry)
        {
            double width = grid.ActualWidth;
            double height = grid.ActualHeight;
            double radius = 15; // Raio dos cantos inferiores

            var figure = new PathFigure
            {
                StartPoint = new Point(0, 0) // Começa no canto superior esquerdo
            };

            // Linha superior
            figure.Segments.Add(new LineSegment(new Point(width, 0), true));

            // Linha lateral direita
            figure.Segments.Add(new LineSegment(new Point(width, height - radius), true));

            // Arco inferior direito
            figure.Segments.Add(new ArcSegment
            {
                Point = new Point(width - radius, height),
                Size = new Size(radius, radius),
                SweepDirection = SweepDirection.Clockwise
            });

            // Linha inferior
            figure.Segments.Add(new LineSegment(new Point(radius, height), true));

            // Arco inferior esquerdo
            figure.Segments.Add(new ArcSegment
            {
                Point = new Point(0, height - radius),
                Size = new Size(radius, radius),
                SweepDirection = SweepDirection.Clockwise
            });

            // Linha lateral esquerda
            figure.Segments.Add(new LineSegment(new Point(0, 0), true));

            // Atualiza o PathGeometry
            clipGeometry.Figures.Clear();
            clipGeometry.Figures.Add(figure);
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

        private void ProgressBar_Loaded(object sender, RoutedEventArgs e)
        {
            if (sender is ProgressBar progressBar)
            {
                var clipGeometry = new RectangleGeometry
                {
                    Rect = new Rect(0, 0, progressBar.ActualWidth, progressBar.ActualHeight),
                    RadiusX = 15,
                    RadiusY = 15
                };

                progressBar.Clip = clipGeometry;

                // Atualiza o Clip dinamicamente quando o tamanho mudar
                progressBar.SizeChanged += (s, args) =>
                {
                    clipGeometry.Rect = new Rect(0, 0, progressBar.ActualWidth, progressBar.ActualHeight);
                };
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainScrollViewer.ScrollToTop();
        }
    }
}