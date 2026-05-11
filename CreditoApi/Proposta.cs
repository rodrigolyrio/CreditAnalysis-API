public class Proposta {
    public int Id { get; set; }
    public string NomeCliente { get; set; } = string.Empty;
    public string CPF { get; set; } = string.Empty; // CPF
    public decimal ValorSolicitado { get; set; }
    public decimal RendaMensal { get; set; }
    public string StatusProposta { get; set; } = "Em Analise";
    public decimal ValorParcela { get; set; } 
}