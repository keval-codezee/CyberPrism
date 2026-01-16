using Prism.Mvvm;

namespace CyberPrism.Modules.FactoryVisual.ViewModels
{
    public class FactoryVisualViewModel : BindableBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public FactoryVisualViewModel()
        {
            
        }
    }
}

