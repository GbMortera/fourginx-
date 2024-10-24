using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.IO;
using System;
using EnviaDados.Models;

namespace EnviaDados.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index()
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "cadastro.html"), "text/html");
        }

        [HttpPost("/configura")]
        public IActionResult Configura([FromBody] ProxyConfig config)
        {
            // Caminho do script existente que será chamado
            string scriptPath = "/home/fouradmin/webserverteste/add_server.sh";

            try
            {
                // Criar o comando que será executado com as variáveis
                string comando = $"/bin/bash {scriptPath} {config.Porta} {config.Alias} {config.ProxyPass} {config.nome}";

                // Configurar o processo para executar o bash com o script
                ProcessStartInfo procStartInfo = new ProcessStartInfo
                {
                    FileName = "/bin/bash",
                    Arguments = $"-c \"{comando}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process proc = new Process())
                {
                    proc.StartInfo = procStartInfo;
                    proc.Start();

                    // Captura a saída e erros do script
                    string output = proc.StandardOutput.ReadToEnd();
                    string error = proc.StandardError.ReadToEnd();
                    proc.WaitForExit();

                    // Verificar se houve algum erro
                    if (!string.IsNullOrEmpty(error))
                    {
                        return StatusCode(500, new { message = $"Erro ao executar o script: {error}" });
                    }

                    // Retornar a saída do script, se necessário
                    return Ok(new { message = "Configurações gravadas com sucesso!", output });
                }
            }
            catch (Exception ex)
            {
                // Capturar e retornar o erro
                return StatusCode(500, new { message = $"Erro ao executar o script: {ex.Message}" });
            }
        }
    }
}
