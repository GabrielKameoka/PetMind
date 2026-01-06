using PetMind.API.Services.Data;

namespace PetMind.API.Services
{
    public interface IValidaRacaService
    {
        bool RacaValida(string raca);
        bool PorteValido(string porte);
        List<string> GetRacasValidas();
        List<string> GetRacasPorPorte(string porte);
        bool RacaCompativelComPorte(string raca, string porte);
        List<string> GetServicosPorRacaPorte(string raca, string porte);
    }

    public class ValidaRacaService : IValidaRacaService
    {
        public bool RacaValida(string raca)
        {
            var racas = ServicosBaseData.GetServicosBase()
                .Select(s => s.Raca)
                .Distinct()
                .ToList();
            
            return racas.Any(r => 
                r.Trim().Equals(raca.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public bool PorteValido(string porte)
        {
            var portes = new[] { "Pequeno", "Médio", "Grande" };
            return portes.Any(p => 
                p.Trim().Equals(porte.Trim(), StringComparison.OrdinalIgnoreCase));
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
                .Where(s => s.Porte.Trim().Equals(porte.Trim(), StringComparison.OrdinalIgnoreCase))
                .Select(s => s.Raca)
                .Distinct()
                .OrderBy(r => r)
                .ToList();
        }

        public bool RacaCompativelComPorte(string raca, string porte)
        {
            return ServicosBaseData.GetServicosBase()
                .Any(s => s.Raca.Trim().Equals(raca.Trim(), StringComparison.OrdinalIgnoreCase) &&
                          s.Porte.Trim().Equals(porte.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        public List<string> GetServicosPorRacaPorte(string raca, string porte)
        {
            var servicosBase = ServicosBaseData.GetServicosBase()
                .FirstOrDefault(s => 
                    s.Raca.Trim().Equals(raca.Trim(), StringComparison.OrdinalIgnoreCase) &&
                    s.Porte.Trim().Equals(porte.Trim(), StringComparison.OrdinalIgnoreCase));
            
            return servicosBase?.Servicos?.Keys.ToList() ?? new List<string>();
        }
    }
}