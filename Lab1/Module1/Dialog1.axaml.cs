using Avalonia.Controls;
using Avalonia.Interactivity;

namespace Module1
{
    public partial class Dialog1 : Window
    {
        public Dialog1()
        {
            InitializeComponent();
        }

        public int SelectedValue => (int)this.FindControl<Slider>("NumberSlider")!.Value;

        private void OkButton_Click(object? sender, RoutedEventArgs e)
        {
            Close(true);
        }

        private void CancelButton_Click(object? sender, RoutedEventArgs e)
        {
            Close(false);
        }
    }
}
