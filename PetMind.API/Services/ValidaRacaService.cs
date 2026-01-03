// Services/ValidacaoService.cs
using PetMind.API.Services.Data;

namespace PetMind.API.Services
{
    public interface IValidacaoService
    {
        bool RacaValida(string raca);
        bool PorteValido(string porte);
        List<string> GetRacasValidas();
        List<string> GetRacasPorPorte(string porte);
        bool RacaCompativelComPorte(string raca, string porte);
    }

    public class ValidacaoService : IValidacaoService
    {
        public bool RacaValida(string raca)
        {
            var racas = ServicosBaseData.GetServicosBase()
                .Select(s => s.Raca)
                .Distinct()
                .ToList();
            
            return racas.Any(r => 
                string.Equals(r, raca, StringComparison.OrdinalIgnoreCase));
        }

        public bool PorteValido(string porte)
        {
            var portes = new[] { "Pequeno", "Médio", "Grande" };
            return portes.Contains(porte, StringComparer.OrdinalIgnoreCase);
        }

        public List<string> GetRacasValidas()
        {
            return ServicosBaseData.GetServicosBase()
                .Select(s => s.Raca)
                .Distinct()
                .OrderBy(r => r)
                .ToList();
        }

        public List<string> GetRacasPorPorte(string porte)
        {
            return ServicosBaseData.GetServicosBase()
                .Where(s => string.Equals(s.Porte, porte, StringComparison.OrdinalIgnoreCase))
                .Select(s => s.Raca)
                .Distinct()
                .OrderBy(r => r)
                .ToList();
        }

        public bool RacaCompativelComPorte(string raca, string porte)
        {
            return ServicosBaseData.GetServicosBase()
                .Any(s => string.Equals(s.Raca, raca, StringComparison.OrdinalIgnoreCase) &&
                          string.Equals(s.Porte, porte, StringComparison.OrdinalIgnoreCase));
        }
    }
}