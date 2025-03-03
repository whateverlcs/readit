using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace readit.Helpers
{
    public static class SmoothScrollBehavior
    {
        public static bool GetEnableSmoothScrolling(DependencyObject obj)
        {
            return (bool)obj.GetValue(EnableSmoothScrollingProperty);
        }

        public static void SetEnableSmoothScrolling(DependencyObject obj, bool value)
        {
            obj.SetValue(EnableSmoothScrollingProperty, value);
        }

        public static readonly DependencyProperty EnableSmoothScrollingProperty =
            DependencyProperty.RegisterAttached(
                "EnableSmoothScrolling",
                typeof(bool),
                typeof(SmoothScrollBehavior),
                new PropertyMetadata(false, OnEnableSmoothScrollingChanged));

        private static void OnEnableSmoothScrollingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (d is ScrollViewer scrollViewer)
            {
                if ((bool)e.NewValue)
                {
                    scrollViewer.PreviewMouseWheel += ScrollViewer_PreviewMouseWheel;
                }
                else
                {
                    scrollViewer.PreviewMouseWheel -= ScrollViewer_PreviewMouseWheel;
                }
            }
        }

        private static void ScrollViewer_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (sender is ScrollViewer scrollViewer)
            {
                e.Handled = true;

                double currentOffset = scrollViewer.VerticalOffset;
                double targetOffset = currentOffset - (e.Delta * 1.0); // Aumenta a sensibilidade

                // Aplica um movimento suave diretamente
                scrollViewer.ScrollToVerticalOffset(targetOffset);
            }
        }
    }
}