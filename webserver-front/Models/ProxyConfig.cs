namespace EnviaDados.Models
{
    public class ProxyConfig
    {
        public string Alias { get; set; }
        public int Porta { get; set; }
        public string ProxyPass { get; set; }
        public string nome { get; set; } // Adicionado para capturar o Host
    }
}
