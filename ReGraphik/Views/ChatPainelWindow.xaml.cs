using System.Windows;
using System.Windows.Input;
using ReGraphik.ViewModels;

namespace ReGraphik.Views
{
    public partial class ChatPainelWindow : Window
    {
        public ChatPainelWindow()
        {
            InitializeComponent();
            DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue is ChatViewModel vmAntiga)
            {
                vmAntiga.SolicitarScrollParaFim -= RolandoParaOFim;
                vmAntiga.SolicitarArrastarJanela -= Arrastar;
                vmAntiga.SolicitarOcultarJanela -= Ocultar;
            }

            if (e.NewValue is ChatViewModel vmNova)
            {
                vmNova.SolicitarScrollParaFim += RolandoParaOFim;
                vmNova.SolicitarArrastarJanela += Arrastar;
                vmNova.SolicitarOcultarJanela += Ocultar;
            }
        }

        private void RolandoParaOFim()
        {
            Dispatcher.BeginInvoke(() =>
            {
                ScrollMensagens.ScrollToBottom();
            }, System.Windows.Threading.DispatcherPriority.Loaded);
        }

        private void Arrastar()
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
                DragMove();
        }

        private void Ocultar() => Hide();
    }
}