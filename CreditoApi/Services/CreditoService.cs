namespace CreditoApi.Services
{
    public class CreditoService
    {
        public string AvaliarProposta(decimal valorSolicitado, decimal rendaMensal)
        {
            decimal valorParcela = valorSolicitado / 12;
            decimal limiteComprometimento = rendaMensal * 0.3m;

            return (valorParcela > limiteComprometimento) ? "Reprovado" : "Aprovado";
        }
    }
}