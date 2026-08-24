using Microsoft.Win32;
using ReGraphik.Models;
using ReGraphik.Services;
using ReGraphik.Services.Interface;
using ReGraphik.Views;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ReGraphik.ViewModels
{
    /// <summary>
    /// ViewModel para a tela de conta do usuário, responsável por gerenciar os dados do perfil, estatísticas e interações com a API.
    /// </summary>
    public class ContaViewModel : BaseViewModel
    {
        private readonly Usuario _usuarioAtual;
        private readonly IAutorizarService _autorizarService;
        private readonly IResiduoService _residuoService;
        private string _emailReal = string.Empty;

        /// <summary>
        /// Guarda temporariamente o caminho da nova foto selecionada antes de salvar na API
        /// </summary>
        private string _caminhoNovaFotoSelecionada = string.Empty;

        public string UltimoAcesso => $"Hoje às {DateTime.Now:HH:mm}";

        private string? _fotoPerfilCaminho;
        public string? FotoPerfilCaminho
        {
            get => _fotoPerfilCaminho;
            set
            {
                _fotoPerfilCaminho = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(SemFoto));
            }
        }

        public bool SemFoto => string.IsNullOrWhiteSpace(FotoPerfilCaminho);

        public string Iniciais
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Nome)) return "?";
                var partes = Nome.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                if (partes.Length == 1) return partes[0][0].ToString().ToUpper();
                return (partes[0][0].ToString() + partes[^1][0].ToString()).ToUpper();
            }
        }

        public string LoginExibicao => string.IsNullOrWhiteSpace(Login) ? string.Empty : $"@{Login}";
        public string EmailResumido => MascararEmail(_emailReal);

        private string? _nome;
        public string Nome
        {
            get => _nome;
            set
            {
                _nome = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(Iniciais));
            }
        }

        private string? _cpf;
        public string CPF
        {
            get => _cpf;
            set { _cpf = value; OnPropertyChanged(); }
        }

        private string? _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string? _login;
        public string Login
        {
            get => _login;
            set { _login = value; OnPropertyChanged(); OnPropertyChanged(nameof(LoginExibicao)); }
        }

        private string? _perfil;
        public string Perfil
        {
            get => _perfil;
            set { _perfil = value; OnPropertyChanged(); }
        }

        private string? _cargo;
        public string Cargo
        {
            get => _cargo;
            set { _cargo = value; OnPropertyChanged(); }
        }

        private string? _departamento;
        public string Departamento
        {
            get => _departamento;
            set { _departamento = value; OnPropertyChanged(); }
        }

        private string? _telefone;
        public string Telefone
        {
            get => _telefone;
            set { _telefone = value; OnPropertyChanged(); }
        }

        private bool _ocupado;
        public bool Ocupado
        {
            get => _ocupado;
            set { _ocupado = value; OnPropertyChanged(); }
        }

        private int _totalResiduos;
        public int TotalResiduos
        {
            get => _totalResiduos;
            set { _totalResiduos = value; OnPropertyChanged(); }
        }

        private int _totalReaproveitados;
        public int TotalReaproveitados
        {
            get => _totalReaproveitados;
            set { _totalReaproveitados = value; OnPropertyChanged(); }
        }

        private string _valorEconomico = "R$ 0,00";
        public string ValorEconomico
        {
            get => _valorEconomico;
            set { _valorEconomico = value; OnPropertyChanged(); }
        }

        private ObservableCollection<Residuo> _ultimosResiduos = new();
        public ObservableCollection<Residuo> UltimosResiduos
        {
            get => _ultimosResiduos;
            set { _ultimosResiduos = value; OnPropertyChanged(); }
        }

        private bool _carregandoEstatisticas;
        public bool CarregandoEstatisticas
        {
            get => _carregandoEstatisticas;
            set { _carregandoEstatisticas = value; OnPropertyChanged(); }
        }

        private string _mensagemErroEmail = string.Empty;
        public string MensagemErroEmail
        {
            get => _mensagemErroEmail;
            set { _mensagemErroEmail = value; OnPropertyChanged(); }
        }

        private string _mensagemSucesso = string.Empty;
        public string MensagemSucesso
        {
            get => _mensagemSucesso;
            set { _mensagemSucesso = value; OnPropertyChanged(); }
        }

        private string _mensagemErroGeral = string.Empty;
        public string MensagemErroGeral
        {
            get => _mensagemErroGeral;
            set { _mensagemErroGeral = value; OnPropertyChanged(); }
        }

        // Comandos
        public ICommand SalvarCommand { get; }
        public ICommand CancelarCommand { get; }
        public ICommand EmailGotFocusCommand { get; }
        public ICommand EmailLostFocusCommand { get; }
        public ICommand SelecionarFotoCommand { get; }
        public ICommand AtualizarEstatisticasCommand { get; }

        public ContaViewModel(Usuario usuario, IAutorizarService autorizarService)
        {
            _usuarioAtual = usuario;
            _autorizarService = autorizarService;
            _residuoService = new ResiduoService();

            CarregarDadosNaTela();

            SalvarCommand = new RelayCommand(async (param) => await SalvarPerfilAsync(param));
            CancelarCommand = new RelayCommand(_ => CarregarDadosNaTela());
            EmailGotFocusCommand = new RelayCommand(EmailGotFocus);
            EmailLostFocusCommand = new RelayCommand(EmailLostFocus);
            SelecionarFotoCommand = new RelayCommand(_ => MudarFoto());
            AtualizarEstatisticasCommand = new RelayCommand(async _ => await CarregarEstatisticasAsync());

            string? fotoInicial = !string.IsNullOrEmpty(_usuarioAtual.FotoPerfil)
                ? _usuarioAtual.FotoPerfil
                : UsuarioSessaoService.Instancia.FotoCaminho;

            FotoPerfilCaminho = fotoInicial;

            _ = CarregarEstatisticasAsync();
        }

        /// <summary>
        /// Carrega os dados do usuário atual na tela, preenchendo as propriedades correspondentes.
        /// </summary>
        private void CarregarDadosNaTela()
        {
            Nome = _usuarioAtual.Nome ?? string.Empty;
            Login = _usuarioAtual.Login ?? string.Empty;
            Perfil = _usuarioAtual.Perfil ?? "Usuário";
            Cargo = _usuarioAtual.Cargo ?? "Analista Operacional";
            Departamento = _usuarioAtual.Departamento ?? "Gestão Ambiental";
            Telefone = _usuarioAtual.Telefone ?? string.Empty;

            CPF = MascararCpf(_usuarioAtual.CPF);

            _emailReal = _usuarioAtual.Email ?? string.Empty;
            Email = MascararEmail(_emailReal);

            OnPropertyChanged(nameof(EmailResumido));
        }

        /// <summary>
        /// Carrega as estatísticas relacionadas aos resíduos do usuário, incluindo total de resíduos, total reaproveitados e valor econômico estimado.
        /// </summary>
        /// <returns></returns>
        private async Task CarregarEstatisticasAsync()
        {
            try
            {
                CarregandoEstatisticas = true;

                var todos = await _residuoService.ObterTodosResiduosAsync();
                var meus = todos.ToList();

                TotalResiduos = meus.Count;
                TotalReaproveitados = meus.Count(r => r.Status == "Reaproveitado");

                double somaValores = meus.Sum(r => r.Quantidade * 5.50);
                ValorEconomico = somaValores.ToString("C2");

                var todos_ordenados = meus
                    .Where(r => r.DataCadastro != null)
                    .OrderByDescending(r => r.DataCadastro)
                    .ToList();

                Application.Current.Dispatcher.Invoke(() =>
                    UltimosResiduos = new ObservableCollection<Residuo>(todos_ordenados));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Erro ao carregar estatísticas: {ex.Message}");

                Application.Current.Dispatcher.Invoke(() =>
                {
                    MensagemWindow.Exibir("Erro", "Não foi possível carregar as estatísticas!", MensagemWindow.TipoMensagem.Erro);
                });

                TotalResiduos = 0;
                TotalReaproveitados = 0;
                ValorEconomico = "R$ 0,00";
            }
            finally
            {
                CarregandoEstatisticas = false;
            }
        }

        /// <summary>
        /// Método chamado quando o campo de e-mail recebe foco. Ele limpa a mensagem de erro e exibe o e-mail real do usuário para edição.
        /// </summary>
        public void EmailGotFocus()
        {
            MensagemErroEmail = string.Empty;
            Email = _emailReal;
        }

        public void EmailLostFocus()
        {
            if (!string.IsNullOrWhiteSpace(Email) && !Email.Contains('@'))
            {
                MensagemErroEmail = "E-mail inválido. Verifique o endereço informado.";
            }
            else
            {
                MensagemErroEmail = string.Empty;
                _emailReal = Email ?? string.Empty;
                _usuarioAtual.Email = _emailReal;
                OnPropertyChanged(nameof(EmailResumido));
            }

            Email = MascararEmail(_emailReal);
        }

        /// <summary>
        /// Método responsável por abrir um diálogo para o usuário selecionar uma nova foto de perfil. 
        /// Se uma foto for selecionada, o caminho da foto é armazenado e a propriedade FotoPerfilCaminho é atualizada.
        /// </summary>
        private void MudarFoto()
        {
            try
            {
                var openFileDialog = new OpenFileDialog
                {
                    Title = "Selecionar Foto de Perfil",
                    Filter = "Imagens (*.jpg;*.jpeg;*.png)|*.jpg;*.jpeg;*.png"
                };

                if (openFileDialog.ShowDialog() != true) return;

                _caminhoNovaFotoSelecionada = openFileDialog.FileName;
                FotoPerfilCaminho = _caminhoNovaFotoSelecionada;
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MensagemWindow.Exibir("Erro", "Erro ao carregar a foto de perfil!", MensagemWindow.TipoMensagem.Erro);
                });
            }
        }

        /// <summary>
        /// Método responsável por mascarar o CPF do usuário, exibindo apenas os três primeiros dígitos e substituindo os demais por asteriscos.
        /// </summary>
        /// <param name="cpf"></param>
        /// <returns></returns>
        private static string MascararCpf(string? cpf)
        {
            if (string.IsNullOrWhiteSpace(cpf)) return string.Empty;
            var d = Regex.Replace(cpf, @"\D", "");
            return d.Length >= 3 ? d[..3] + ".***.***-**" : cpf;
        }

        /// <summary>
        /// Método responsável por mascarar o e-mail do usuário, exibindo apenas os dois primeiros caracteres antes do "@" e substituindo os demais por asteriscos.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private static string MascararEmail(string? email)
        {
            if (string.IsNullOrWhiteSpace(email)) return string.Empty;
            var at = email.IndexOf('@');
            if (at <= 2) return email;
            return email[..2] + new string('*', at - 2) + email[at..];
        }

        /// <summary>
        /// Método assíncrono responsável por salvar as alterações no perfil do usuário.
        /// </summary>
        /// <param name="parameter"></param>
        private async Task SalvarPerfilAsync(object? parameter)
        {
            try
            {
                MensagemSucesso = string.Empty;
                MensagemErroGeral = string.Empty;

                if (string.IsNullOrWhiteSpace(Nome) || string.IsNullOrWhiteSpace(Login))
                {
                    MensagemErroGeral = "Nome e Login são obrigatórios.";
                    return;
                }

                if (!string.IsNullOrWhiteSpace(MensagemErroEmail))
                {
                    MensagemErroGeral = "Corrija os erros antes de salvar.";
                    return;
                }

                string novaSenha = string.Empty;
                string confirmacaoSenha = string.Empty;
                PasswordBox? pbSenha = null;
                PasswordBox? pbConfirmacao = null;

                if (parameter is object[] passArray && passArray.Length >= 2)
                {
                    if (passArray[0] is PasswordBox p1) pbSenha = p1;
                    if (passArray[1] is PasswordBox p2) pbConfirmacao = p2;

                    novaSenha = pbSenha?.Password ?? passArray[0]?.ToString() ?? string.Empty;
                    confirmacaoSenha = pbConfirmacao?.Password ?? passArray[1]?.ToString() ?? string.Empty;
                }
                else if (parameter is PasswordBox singlePb)
                {
                    pbSenha = singlePb;
                    novaSenha = singlePb.Password;
                }

                if (!string.IsNullOrEmpty(novaSenha) || !string.IsNullOrEmpty(confirmacaoSenha))
                {
                    if (novaSenha != confirmacaoSenha)
                    {
                        MensagemErroGeral = "As senhas digitadas não coincidem.";
                        return;
                    }
                }

                _usuarioAtual.Nome = Nome;
                _usuarioAtual.Login = Login;
                _usuarioAtual.Email = _emailReal;
                _usuarioAtual.Telefone = Telefone;
                _usuarioAtual.Cargo = Cargo;
                _usuarioAtual.Departamento = Departamento;

                if (!string.IsNullOrWhiteSpace(novaSenha))
                    _usuarioAtual.Senha = novaSenha;

                Ocupado = true;
                bool sucesso = false;

                if (!string.IsNullOrEmpty(_caminhoNovaFotoSelecionada) && File.Exists(_caminhoNovaFotoSelecionada))
                {
                    string? novaUrlFoto = await _autorizarService.AtualizarComFotoAsync(_usuarioAtual.Id, _usuarioAtual, _caminhoNovaFotoSelecionada);
                    sucesso = novaUrlFoto != null;

                    if (sucesso)
                    {
                        _usuarioAtual.FotoPerfil = novaUrlFoto;
                        UsuarioSessaoService.Instancia.FotoCaminho = novaUrlFoto;
                        ConfiguracaoLocalService.SalvarFoto(novaUrlFoto);
                        FotoPerfilCaminho = novaUrlFoto;
                        _caminhoNovaFotoSelecionada = string.Empty;
                    }
                }
                else
                {
                    sucesso = await _autorizarService.AtualizarAsync(_usuarioAtual.Id, _usuarioAtual);
                }

                if (sucesso)
                {
                    Email = MascararEmail(_emailReal);

                    if (pbSenha != null) pbSenha.Password = string.Empty;
                    if (pbConfirmacao != null) pbConfirmacao.Password = string.Empty;

                    OnPropertyChanged(nameof(EmailResumido));
                    OnPropertyChanged(nameof(LoginExibicao));

                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MensagemWindow.Exibir("Sucesso!", "Os dados do perfil foram atualizados com sucesso.", MensagemWindow.TipoMensagem.Sucesso);
                    });
                }
                else
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MensagemWindow.Exibir("Erro", "Erro ao atualizar os dados. Tente novamente mais tarde.", MensagemWindow.TipoMensagem.Erro);
                    });
                }
            }
            catch (Exception)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    MensagemWindow.Exibir("Erro", "Erro de conexão!", MensagemWindow.TipoMensagem.Erro);
                });
            }
            finally
            {
                Ocupado = false;
            }
        }
    }
}