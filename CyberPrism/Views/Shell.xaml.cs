using System.Windows;

namespace CyberPrism.Views
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class Shell : Window
	{
		public Shell()
		{
			InitializeComponent();
		}

        private void Window_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2) return; // Prevent interference with DoubleClick but typically handled separately
            this.DragMove();
        }

        private void Window_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
             if (this.WindowState == WindowState.Maximized)
                this.WindowState = WindowState.Normal;
            else
                this.WindowState = WindowState.Maximized;
        }

        private void Window_MouseRightButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to exit the application?", 
                                       "Confirm Exit", 
                                       MessageBoxButton.YesNo, 
                                       MessageBoxImage.Question);
                                       
            if (result == MessageBoxResult.Yes)
            {
                this.Close();
            }
		}
	}
}
