using PetMind.API.Models;
using PetMind.API.Models.Entities;
using PetMind.API.Services.Data;

namespace PetMind.API.Services
{
    public class CalculaPrecosService
    {
        private readonly List<ServicoBase> _servicosBase;
        private readonly Dictionary<string, Dictionary<string, decimal>> _adicionais;

        public CalculaPrecosService()
        {
            // Dados vêm de arquivos separados
            _servicosBase = ServicosBaseData.GetServicosBase();
            _adicionais = ServicosAdicionaisData.GetAdicionais();
        }

        // Métodos de cálculo PURA (igual seu Angular)
        public decimal GetPrecoBasePorCachorro(Cachorro pet, List<string> servicosBase)
        {
            var item = _servicosBase.Find(s => s.Porte == pet.Porte && s.Raca == pet.Raca);
            return servicosBase.Sum(nome => item?.Servicos.GetValueOrDefault(nome) ?? 0);
        }

        public decimal GetPrecoAdicionalPorCachorro(Cachorro pet, List<string> adicionais)
        {
            var porteKey = pet.Porte switch
            {
                "Pequeno" => "P",
                "Médio" => "M", 
                "Grande" => "G",
                _ => "M"
            };

            return adicionais.Sum(nome => _adicionais.GetValueOrDefault(nome)?.GetValueOrDefault(porteKey) ?? 0);
        }

        public decimal GetPrecoTotalHorario(Horario horario)
        {
            return horario.Cachorros.Sum(pet =>
                GetPrecoBasePorCachorro(pet, new List<string> { horario.ServicoBaseSelecionado }) +
                GetPrecoAdicionalPorCachorro(pet, horario.Adicionais)
            );
        }

        public List<string> GetRacasPorPorte(string porte)
        {
            return _servicosBase
                .Where(item => item.Porte == porte)
                .Select(item => item.Raca)
                .Distinct()
                .ToList();
        }

        public List<string> GetServicosPorRacaPorte(string raca, string porte)
        {
            var item = _servicosBase.FirstOrDefault(s => s.Porte == porte && s.Raca == raca);
            return item?.Servicos.Keys.ToList() ?? new List<string>();
        }

        public List<string> GetServicosAdicionais()
        {
            return _adicionais.Keys.ToList();
        }
    }
}