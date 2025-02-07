using Microsoft.WindowsAPICodePack.Dialogs;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace readit.Views
{
    /// <summary>
    /// Interaction logic for CadastroCapituloView.xaml
    /// </summary>
    public partial class CadastroCapituloView : UserControl
    {
        private readonly string[] _supportedExtensions = { ".zip", ".rar", ".7z", ".tar", ".gz" };

        private bool _isDialogOpen = false;

        public CadastroCapituloView()
        {
            InitializeComponent();
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

        private void DropCapitulos_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (_isDialogOpen) return;

            Global.ListaCapitulosSelecionados.Clear();

            _isDialogOpen = true;

            var dialog = new CommonOpenFileDialog
            {
                Multiselect = true,
                Filters = { new CommonFileDialogFilter("Arquivos Compactados", "*.zip;*.rar;*.7z;*.tar;*.gz") }
            };

            if (dialog.ShowDialog() == CommonFileDialogResult.Ok)
            {
                foreach (var file in dialog.FileNames)
                {
                    Global.ListaCapitulosSelecionados.Add(file);
                }

                TxtDropCapitulos.Text = Global.ListaCapitulosSelecionados.Count > 0 ? $"{Global.ListaCapitulosSelecionados.Count} Capítulo(s) Identificado(s)" : "Nenhum capítulo inserido";
            }

            _isDialogOpen = false;
        }

        private void DropCapitulos_Drop(object sender, System.Windows.DragEventArgs e)
        {
            Global.ListaCapitulosSelecionados.Clear();

            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (var file in files)
                {
                    if (_supportedExtensions.Any(ext => file.EndsWith(ext, System.StringComparison.OrdinalIgnoreCase)))
                    {
                        Global.ListaCapitulosSelecionados.Add(file);
                    }
                }

                TxtDropCapitulos.Text = Global.ListaCapitulosSelecionados.Count > 0 ? $"{Global.ListaCapitulosSelecionados.Count} Capítulo(s) Identificado(s)" : "Nenhum capítulo inserido";
            }
        }

        private void DropCapitulos_PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }
    }
}