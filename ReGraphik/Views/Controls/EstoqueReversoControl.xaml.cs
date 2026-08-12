using ReGraphik.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ReGraphik.Models;           // Permite reconhecer a classe Residuo
using ReGraphik.ViewModels;       // OBRIGATÓRIO: Permite reconhecer a EstoqueReversoViewModel
using ReGraphik.Views;            // Permite abrir a SugestaoResiduoWindow

namespace ReGraphik.Views.Controls
{
    /// <summary>
    /// Define a interação lógica para EstoqueReversoControl.xaml
    /// </summary>
    public partial class EstoqueReversoControl : UserControl
    {
        public EstoqueReversoControl()
        {
            InitializeComponent();
            DataContext = new EstoqueReversoViewModel();

            this.IsVisibleChanged += EstoqueReversoControl_IsVisibleChanged;
        }

        /// <summary>
        /// Evento de clique do botão "Sugestões" do DataGrid
        /// </summary>
        private void BtnSugestoes_Click(object sender, RoutedEventArgs e)
        {
            // Obtém o botão que foi clicado
            var botao = sender as Button;

            // Pega o objeto Residuo vinculado à linha atual da tabela
            if (botao?.DataContext is Residuo residuoSelecionado)
            {
                // Cria a janela de sugestões passando o resíduo da linha por parâmetro!
                SugestaoResiduoWindow window = new SugestaoResiduoWindow(residuoSelecionado);

                // Define a janela principal do app como "mãe" desta para centralizar na tela
                window.Owner = Application.Current.MainWindow;

                // Abre em modo modal (bloqueia a tela de trás até fechar)
                window.ShowDialog();
            }
        }

        /// <summary>
        /// Abre o menu de alteração de status no clique esquerdo do selo do card.
        /// O ContextMenu do WPF só abre sozinho no clique direito, por isso é acionado aqui.
        /// </summary>
        private void BtnStatus_Click(object sender, RoutedEventArgs e)
        {
            if (sender is not Button botao || botao.ContextMenu is null) return;

            // Necessário para que os MenuItens encontrem os comandos publicados no Tag do botão
            botao.ContextMenu.PlacementTarget = botao;
            botao.ContextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            botao.ContextMenu.IsOpen = true;
        }

        private async void EstoqueReversoControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            // Verifica se a tela ficou visível para o usuário
            if ((bool)e.NewValue == true)
            {
                if (DataContext is EstoqueReversoViewModel viewModel)
                {
                    // Recarrega os dados do banco atualizando o Grid em tempo real
                    await viewModel.CarregarEstoqueDoBancoAsync();
                }
            }
        }
    }
}