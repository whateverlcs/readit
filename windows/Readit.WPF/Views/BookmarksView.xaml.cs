﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using WpfAnimatedGif;

namespace Readit.WPF.Views
{
    /// <summary>
    /// Interaction logic for BookmarksView.xaml
    /// </summary>
    public partial class BookmarksView : UserControl
    {
        public BookmarksView()
        {
            InitializeComponent();

            ImageBehavior.SetAnimatedSource(GifImage, GifImage.Source);
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

        /// <summary>
        /// Realiza o scroll até o topo da tela.
        /// </summary>
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MainScrollViewer.ScrollToTop();
        }
    }
}