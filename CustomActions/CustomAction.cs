using System;
using System.Diagnostics;
using System.Windows.Forms;
using Microsoft.Deployment.WindowsInstaller;

namespace CustomActions
{
    /// <summary>
    /// Classe que contém ações personalizadas para o instalador do ReGraphik.
    /// </summary>
    public class CustomActions
    {
        /// <summary>
        /// Verifica se o aplicativo ReGraphik está em execução e solicita ao usuário que o feche antes de prosseguir com a instalação.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult CheckAndCloseApp(Session session)
        {
            Process[] processes = Process.GetProcessesByName("ReGraphik");

            if (processes.Length > 0)
            {
                string warningMsg = session.GetTranslation("AppRunningWarning") ??
                    "O ReGraphik está em execução. Deseja fechar a aplicação para prosseguir?";

                DialogResult result = MessageBox.Show(warningMsg, "ReGraphik Setup",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (result == DialogResult.Yes)
                {
                    foreach (var process in processes)
                    {
                        process.Kill();
                        process.WaitForExit();
                    }
                }
                else
                {
                    return ActionResult.UserExit;
                }
            }
            return ActionResult.Success;
        }

        /// <summary>
        /// Verifica a versão instalada do ReGraphik e exibe uma mensagem informando se a versão atual é mais recente ou igual à versão instalada.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        [CustomAction]
        public static ActionResult CheckVersionAction(Session session)
        {
            string installedVersionStr = session["INSTALLEDFOLDERSVERSION"]; // Obtida do registro ou AppSearch
            string currentVersionStr = session["ProductVersion"];

            if (!string.IsNullOrEmpty(installedVersionStr))
            {
                Version installedVer = new Version(installedVersionStr);
                Version currentVer = new Version(currentVersionStr);

                if (installedVer >= currentVer)
                {
                    string alreadyInstalledMsg = string.Format(
                        session.GetTranslation("AlreadyInstalled") ?? "A versão {0} já está instalada.",
                        installedVersionStr);

                    MessageBox.Show(alreadyInstalledMsg, "ReGraphik Setup", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }

            return ActionResult.Success;
        }
    }

    /// <summary>
    /// Extensão para a classe Session do Windows Installer, permitindo a obtenção de traduções de strings localizadas.
    /// </summary>
    public static class SessionExtensions
    {
        public static string GetTranslation(this Session session, string key)
        {
            try { return session.GetProductProperty("!(loc." + key + ")"); }
            catch { return null; }
        }
    }
}