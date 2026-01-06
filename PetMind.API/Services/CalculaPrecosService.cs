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
            _servicosBase = ServicosBaseData.GetServicosBase();
            _adicionais = ServicosAdicionaisData.GetAdicionais();
        }

        // Métodos auxiliares para comparação de strings
        private bool StringsIguais(string? a, string? b)
        {
            if (a == null || b == null) return false;
            return a.Trim().Equals(b.Trim(), StringComparison.OrdinalIgnoreCase);
        }

        public decimal GetPrecoBasePorCachorro(Cachorro pet, List<string> servicosBase)
        {
            var item = _servicosBase.Find(s => 
                StringsIguais(s.Porte, pet.Porte) && 
                StringsIguais(s.Raca, pet.Raca));
            
            return servicosBase.Sum(nome => 
                item?.Servicos.GetValueOrDefault(nome) ?? 0);
        }

        public decimal GetPrecoAdicionalPorCachorro(Cachorro pet, List<string> adicionais)
        {
            var porteKey = (pet.Porte?.Trim() ?? "").ToUpper() switch
            {
                "PEQUENO" => "P",
                "MÉDIO" => "M",
                "MEDIO" => "M",
                "GRANDE" => "G",
                _ => "M"
            };

            return adicionais.Sum(nome => 
                _adicionais.GetValueOrDefault(nome)?.GetValueOrDefault(porteKey) ?? 0);
        }

        public decimal GetPrecoTotalHorario(Horario horario)
        {
            if (horario.Cachorro == null) return 0;
    
            return GetPrecoBasePorCachorro(horario.Cachorro, 
                       new List<string> { horario.ServicoBaseSelecionado }) +
                   GetPrecoAdicionalPorCachorro(horario.Cachorro, horario.Adicionais);
        }

        public List<string> GetRacasPorPorte(string? porte)
        {
            if (string.IsNullOrWhiteSpace(porte))
                return new List<string>();
                
            return _servicosBase
                .Where(item => StringsIguais(item.Porte, porte))
                .Select(item => item.Raca)
                .Distinct()
                .ToList();
        }

        public List<string> GetServicosPorRacaPorte(string? raca, string? porte)
        {
            if (string.IsNullOrWhiteSpace(raca) || string.IsNullOrWhiteSpace(porte))
                return new List<string>();
            
            var item = _servicosBase.FirstOrDefault(s => 
                StringsIguais(s.Porte, porte) && 
                StringsIguais(s.Raca, raca));
            
            return item?.Servicos.Keys.ToList() ?? new List<string>();
        }

        public List<string> GetServicosAdicionais()
        {
            return _adicionais.Keys.ToList();
        }

        // MÉTODO ADICIONAL PARA DEBUG
        public Dictionary<string, object> DebugBuscaServicos(string raca, string porte)
        {
            var resultado = new Dictionary<string, object>();
            
            resultado["RacaInformada"] = raca;
            resultado["PorteInformado"] = porte;
            resultado["RacaTrimmed"] = raca?.Trim();
            resultado["PorteTrimmed"] = porte?.Trim();
            
            // Todas as combinações disponíveis
            var todasCombinacoes = _servicosBase
                .Select(s => new 
                { 
                    PorteOriginal = s.Porte, 
                    PorteTrimmed = s.Porte?.Trim(),
                    RacaOriginal = s.Raca, 
                    RacaTrimmed = s.Raca?.Trim(),
                    Servicos = s.Servicos.Keys.ToList()
                })
                .ToList();
            
            resultado["TodasCombinacoes"] = todasCombinacoes;
            
            // Busca específica
            var itemEncontrado = _servicosBase.FirstOrDefault(s => 
                StringsIguais(s.Porte, porte) && 
                StringsIguais(s.Raca, raca));
            
            resultado["ItemEncontrado"] = itemEncontrado != null;
            resultado["ServicosDisponiveis"] = itemEncontrado?.Servicos.Keys.ToList() ?? new List<string>();
            
            return resultado;
        }
    }
}