namespace PetMind.API.Services.Data
{
    public static class ServicosAdicionaisData
    {
        public static Dictionary<string, Dictionary<string, decimal>> GetAdicionais()
        {
            return new Dictionary<string, Dictionary<string, decimal>>
            {
                ["Escovação de Dente"] = new Dictionary<string, decimal> { ["P"] = 15, ["M"] = 15, ["G"] = 15 },
                ["Desembolo"] = new Dictionary<string, decimal> { ["P"] = 15, ["M"] = 25, ["G"] = 60 },
                ["Hidratação"] = new Dictionary<string, decimal> { ["P"] = 20, ["M"] = 25, ["G"] = 30 },
                ["TaxiDog"] = new Dictionary<string, decimal> { ["P"] = 15, ["M"] = 15, ["G"] = 15 },
                ["Cortar Unha"] = new Dictionary<string, decimal> { ["P"] = 5, ["M"] = 10, ["G"] = 10 }
            };
        }
    }
}