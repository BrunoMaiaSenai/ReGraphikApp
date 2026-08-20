using Microsoft.Win32;
using ReGraphik.ViewModels;
using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ReGraphik.Views.Behaviors
{
    public static class MapaBrowserBehavior
    {
        #region Ativar

        public static readonly DependencyProperty AtivarProperty =
            DependencyProperty.RegisterAttached(
                "Ativar",
                typeof(bool),
                typeof(MapaBrowserBehavior),
                new PropertyMetadata(false, OnAtivarChanged));

        public static void SetAtivar(DependencyObject element, bool value)
        {
            element.SetValue(AtivarProperty, value);
        }

        public static bool GetAtivar(DependencyObject element)
        {
            return (bool)element.GetValue(AtivarProperty);
        }

        private static void OnAtivarChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = d as WebBrowser;

            if (browser == null)
                return;

            if ((bool)e.NewValue)
            {
                DefinirEmulacaoNavegador();

                browser.ObjectForScripting =
                    new PonteScriptMapa(browser);

                browser.Navigated += Browser_Navigated;

                string html = GetHtml(browser);

                if (!string.IsNullOrWhiteSpace(html))
                {
                    RenderizarHtml(browser, html);
                }
            }
            else
            {
                browser.Navigated -= Browser_Navigated;
            }
        }

        #endregion


        #region Html

        public static readonly DependencyProperty HtmlProperty =
            DependencyProperty.RegisterAttached(
                "Html",
                typeof(string),
                typeof(MapaBrowserBehavior),
                new PropertyMetadata(
                    string.Empty,
                    OnHtmlChanged));

        public static void SetHtml(
            DependencyObject element,
            string value)
        {
            element.SetValue(HtmlProperty, value);
        }

        public static string GetHtml(
            DependencyObject element)
        {
            return (string)element.GetValue(HtmlProperty);
        }

        private static void OnHtmlChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = d as WebBrowser;

            if (browser == null)
                return;

            if (!GetAtivar(browser))
                return;

            string html = e.NewValue as string;

            if (!string.IsNullOrWhiteSpace(html))
            {
                RenderizarHtml(browser, html);
            }
        }

        #endregion


        #region IndiceFoco

        public static readonly DependencyProperty IndiceFocoProperty =
            DependencyProperty.RegisterAttached(
                "IndiceFoco",
                typeof(int?),
                typeof(MapaBrowserBehavior),
                new PropertyMetadata(
                    null,
                    OnIndiceFocoChanged));

        public static void SetIndiceFoco(
            DependencyObject element,
            int? value)
        {
            element.SetValue(
                IndiceFocoProperty,
                value);
        }

        public static int? GetIndiceFoco(
            DependencyObject element)
        {
            return (int?)element.GetValue(
                IndiceFocoProperty);
        }

        private static void OnIndiceFocoChanged(
            DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            WebBrowser browser = d as WebBrowser;

            if (browser == null)
                return;

            if (!GetAtivar(browser))
                return;

            if (e.NewValue == null)
                return;

            int indice = (int)e.NewValue;

            browser.Dispatcher.Invoke(() =>
            {
                try
                {
                    browser.InvokeScript(
                        "centralizarPonto",
                        indice);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "Erro ao focar no marcador: " +
                        ex.Message);
                }
            });
        }

        #endregion


        #region Placeholder

        public static readonly DependencyProperty PlaceholderProperty =
            DependencyProperty.RegisterAttached(
                "Placeholder",
                typeof(FrameworkElement),
                typeof(MapaBrowserBehavior),
                new PropertyMetadata(null));

        public static void SetPlaceholder(
            DependencyObject element,
            FrameworkElement value)
        {
            element.SetValue(
                PlaceholderProperty,
                value);
        }

        public static FrameworkElement GetPlaceholder(
            DependencyObject element)
        {
            return (FrameworkElement)
                element.GetValue(
                    PlaceholderProperty);
        }

        #endregion


        private static void RenderizarHtml(
            WebBrowser browser,
            string conteudoHtml)
        {
            browser.Dispatcher.Invoke(() =>
            {
                try
                {
                    dynamic activeX =
                        browser.GetType().InvokeMember(
                            "ActiveXInstance",
                            System.Reflection.BindingFlags.GetProperty |
                            System.Reflection.BindingFlags.Instance |
                            System.Reflection.BindingFlags.NonPublic,
                            null,
                            browser,
                            null);

                    if (activeX != null)
                    {
                        activeX.Silent = true;
                    }

                    browser.NavigateToString(
                        conteudoHtml);
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine(
                        "Erro ao renderizar HTML: " +
                        ex.Message);
                }
            });
        }


        private static void Browser_Navigated(
            object sender,
            System.Windows.Navigation.NavigationEventArgs e)
        {
            WebBrowser browser =
                sender as WebBrowser;

            if (browser == null)
                return;

            MapaViewModel viewModel =
                browser.DataContext as MapaViewModel;

            if (viewModel != null)
            {
                viewModel.NotificarMapaCarregado();
            }

            FrameworkElement placeholder =
                GetPlaceholder(browser);

            if (placeholder != null)
            {
                placeholder.Visibility =
                    Visibility.Collapsed;
            }
        }


        private static void DefinirEmulacaoNavegador()
        {
            try
            {
                string nomeProcesso =
                    System.IO.Path.GetFileName(
                        Environment.GetCommandLineArgs()[0]);

                if (nomeProcesso.EndsWith(".vshost.exe"))
                {
                    nomeProcesso =
                        nomeProcesso.Replace(
                            ".vshost.exe",
                            ".exe");
                }

                using (RegistryKey chave =
                    Registry.CurrentUser.OpenSubKey(
                        @"Software\Microsoft\Internet Explorer\Main\FeatureControl\FEATURE_BROWSER_EMULATION",
                        true))
                {
                    if (chave != null)
                    {
                        chave.SetValue(
                            nomeProcesso,
                            11001,
                            RegistryValueKind.DWord);
                    }
                }
            }
            catch
            {
            }
        }


        /// <summary>
        /// Ponte de comunicação entre JavaScript e C#.
        /// Permanece na camada visual para que o WebBrowser
        /// não fique acoplado diretamente à ViewModel.
        /// </summary>
        [ComVisible(true)]
        public class PonteScriptMapa
        {
            private readonly WebBrowser _browser;

            public PonteScriptMapa(
                WebBrowser browser)
            {
                _browser = browser;
            }

            public void NotificarMovimentoMapa(
                double swLat,
                double swLng,
                double neLat,
                double neLng)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        MapaViewModel viewModel = null;

                        _browser.Dispatcher.Invoke(() =>
                        {
                            viewModel =
                                _browser.DataContext
                                as MapaViewModel;
                        });

                        if (viewModel == null)
                            return;

                        await viewModel
                            .BuscarPorCoordenadasAsync(
                                swLat,
                                swLng,
                                neLat,
                                neLng);

                        string novoJson = "";

                        _browser.Dispatcher.Invoke(() =>
                        {
                            novoJson =
                                viewModel.GerarJsonMarcadores(
                                    viewModel
                                        .PontosAtuais
                                        .ToList());
                        });

                        _browser.Dispatcher.Invoke(() =>
                        {
                            try
                            {
                                _browser.InvokeScript(
                                    "renderizarPontos",
                                    new object[]
                                    {
                                        novoJson
                                    });
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(
                                    "Erro ao atualizar marcadores: " +
                                    ex.Message);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(
                            "Erro ao atualizar mapa: " +
                            ex.Message);
                    }
                });
            }
        }
    }
}