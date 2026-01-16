namespace CyberPrism.Modules.Settings.Services
{
    public interface ISuccessDialogService
    {
        void ShowSuccessDialog();
    }

    public class SuccessDialogService : ISuccessDialogService
    {
        public void ShowSuccessDialog()
        {
            var dialog = new Views.SuccessDialog();
            if (System.Windows.Application.Current != null && System.Windows.Application.Current.MainWindow != null)
            {
                dialog.Owner = System.Windows.Application.Current.MainWindow;
            }
            dialog.ShowDialog();
        }
    }
}

