
var builder = WebApplication.CreateBuilder(args);

// Definindo a porta com a variável $1
var port = Environment.GetEnvironmentVariable("PORTA") ?? "5009";
builder.WebHost.UseUrls($"http://*:{port}");

var app = builder.Build();

app.MapGet("/configura/{porta}/{alias}/{proxy_pass}", async context =>
{
    // Recebendo as variáveis pela URL e evitando valores nulos
    var porta = context.Request.RouteValues["porta"] as string ?? "defaultPort";
    var alias = context.Request.RouteValues["alias"] as string ?? "defaultAlias";
    var proxyPass = context.Request.RouteValues["proxy_pass"] as string ?? "defaultProxy";

    // Criando o processo para executar o script
    System.Diagnostics.ProcessStartInfo process = new System.Diagnostics.ProcessStartInfo();
    process.UseShellExecute = false;
    process.WorkingDirectory = "/bin";
    process.FileName = "sh";

    // Passando as variáveis para o script add_server.sh
    process.Arguments = $"/home/fouradmin/add_server.sh {porta} {alias} {proxyPass}";
    process.RedirectStandardOutput = true;

    // Iniciando o processo
    System.Diagnostics.Process cmd = System.Diagnostics.Process.Start(process);
    
    // Aguardando o término
    cmd.WaitForExit();

    // Construindo a resposta em HTML
    var htmlResponse = $@"
    <html>
        <head>
            <title>Cadastro do Host</title>
            <style>
                body {{
                    font-family: Arial, sans-serif;
                    background-color: #f4f4f9;
                    text-align: center;
                    padding: 50px;
                }}
                h1 {{
                    color: #4CAF50;
                }}
                p {{
                    font-size: 18px;
                }}
                .confirmation {{
                    margin-top: 20px;
                    padding: 15px;
                    border: 2px solid #4CAF50;
                    display: inline-block;
                    background-color: #e8f5e9;
                    color: #2e7d32;
                    font-weight: bold;
                }}
            </style>
        </head>
        <body>
            <h1>Host cadastrado com sucesso!</h1>
            <p>O host <strong>{alias}</strong> foi cadastrado na porta <strong>{porta}</strong>.</p>
            <p>Proxy configurado para: <strong>{proxyPass}</strong></p>
            <div class='confirmation'>
                Confirmação de que o host foi registrado e o Nginx foi recarregado.
            </div>
        </body>
    </html>";

    // Respondendo ao cliente com HTML
    context.Response.ContentType = "text/html";
    await context.Response.WriteAsync(htmlResponse);
});

app.Run();


