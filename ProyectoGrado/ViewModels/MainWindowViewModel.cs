using ProyectoGrado.Views;

namespace ProyectoGrado.ViewModels
{
    public class MainWindowViewModel
    {
        private bool _isAdminitracion;

        public bool IsAdminitracion
        {
            get { return _isAdminitracion; }
            set 
            { 
                _isAdminitracion = value;
                AdminitracionView();
            }
        }

        private void AdminitracionView()
        {
            Administracion administracion = new Administracion();
        }
    }
}
