using PetMind.API.Models;

namespace PetMind.API.Services.Data;

public class ServicosBaseData
{
    public static List<ServicoBase> GetServicosBase()
    {
        return new List<ServicoBase>
        {
            // PORTE PEQUENO
            new ServicoBase
            {
                Porte = "Pequeno",
                Raca = "Vira lata",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 60.00m,
                    ["Banho e Tosa Higiênica"] = 70.00m,
                    ["Banho e Tosa Verão"] = 100.00m,
                    ["Banho e Tosa na Máquina"] = 110.00m,
                    ["Banho e Tosa na Tesoura"] = 120.00m,
                    ["Banho e Triming"] = 170.00m,
                    ["Banho Carding"] = 170.00m
                }
            },
            new ServicoBase
            {
                Porte = "Pequeno",
                Raca = "York",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 50.00m,
                    ["Banho e Tosa Higiênica"] = 60.00m,
                    ["Banho e Tosa Verão"] = 90.00m,
                    ["Banho e Tosa na Máquina"] = 100.00m,
                    ["Banho e Tosa na Tesoura"] = 120.00m,
                    ["Banho e Triming"] = 170.00m
                }
            },
            new ServicoBase
            {
                Porte = "Pequeno",
                Raca = "Shitzu",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 50.00m,
                    ["Banho e Tosa Higiênica"] = 60.00m,
                    ["Banho e Tosa Verão"] = 90.00m,
                    ["Banho e Tosa na Máquina"] = 100.00m,
                    ["Banho e Tosa na Tesoura"] = 120.00m,
                    ["Banho e Triming"] = 170.00m
                }
            },
            new ServicoBase
            {
                Porte = "Pequeno",
                Raca = "Maltês",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 50.00m,
                    ["Banho e Tosa Higiênica"] = 60.00m,
                    ["Banho e Tosa Verão"] = 90.00m,
                    ["Banho e Tosa na Máquina"] = 100.00m,
                    ["Banho e Tosa na Tesoura"] = 120.00m,
                    ["Banho e Triming"] = 170.00m
                }
            },
            new ServicoBase
            {
                Porte = "Pequeno",
                Raca = "Dachshund",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 60.00m,
                    ["Banho Carding"] = 90.00m
                }
            },
            new ServicoBase
            {
                Porte = "Pequeno",
                Raca = "Pinscher",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 40.00m,
                    ["Banho Carding"] = 70.00m
                }
            },
            new ServicoBase
            {
                Porte = "Pequeno",
                Raca = "Pug",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 50.00m,
                    ["Banho e Tosa Verão"] = 90.00m,
                    ["Banho Carding"] = 90.00m
                }
            },
            new ServicoBase
            {
                Porte = "Pequeno",
                Raca = "Spitz",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 60.00m,
                    ["Banho e Tosa Higiênica"] = 70.00m,
                    ["Banho e Tosa Verão"] = 90.00m,
                    ["Banho e Tosa na Máquina"] = 100.00m,
                    ["Banho e Tosa na Tesoura"] = 120.00m
                }
            },
            new ServicoBase
            {
                Porte = "Pequeno",
                Raca = "Lhasa",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 50.00m,
                    ["Banho e Tosa Higiênica"] = 60.00m,
                    ["Banho e Tosa Verão"] = 90.00m,
                    ["Banho e Tosa na Máquina"] = 100.00m,
                    ["Banho e Tosa na Tesoura"] = 120.00m,
                    ["Banho e Triming"] = 160.00m
                }
            },
            new ServicoBase
            {
                Porte = "Pequeno",
                Raca = "Poodle",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 50.00m,
                    ["Banho e Tosa Higiênica"] = 60.00m,
                    ["Banho e Tosa Verão"] = 90.00m,
                    ["Banho e Tosa na Máquina"] = 100.00m,
                    ["Banho e Tosa na Tesoura"] = 120.00m,
                    ["Banho e Triming"] = 160.00m
                }
            },

            // PORTE MÉDIO
            new ServicoBase
            {
                Porte = "Médio",
                Raca = "Vira lata",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 60.00m,
                    ["Banho e Tosa Higiênica"] = 70.00m,
                    ["Banho e Tosa Verão"] = 100.00m,
                    ["Banho e Tosa na Máquina"] = 110.00m,
                    ["Banho e Tosa na Tesoura"] = 120.00m,
                    ["Banho e Triming"] = 150.00m,
                    ["Banho Carding"] = 150.00m
                }
            },
            new ServicoBase
            {
                Porte = "Médio",
                Raca = "Poodle",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 60.00m,
                    ["Banho e Tosa Higiênica"] = 70.00m,
                    ["Banho e Tosa Verão"] = 100.00m,
                    ["Banho e Tosa na Máquina"] = 110.00m,
                    ["Banho e Tosa na Tesoura"] = 120.00m
                }
            },
            new ServicoBase
            {
                Porte = "Médio",
                Raca = "Buldog",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 60.00m,
                    ["Banho Carding"] = 90.00m
                }
            },
            new ServicoBase
            {
                Porte = "Médio",
                Raca = "Beagle",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 60.00m,
                    ["Banho e Tosa Verão"] = 100.00m,
                    ["Banho Carding"] = 90.00m
                }
            },
            new ServicoBase
            {
                Porte = "Médio",
                Raca = "Chow Chow",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 90.00m,
                    ["Banho e Tosa Higiênica"] = 100.00m,
                    ["Banho e Tosa Verão"] = 120.00m,
                    ["Banho e Tosa na Máquina"] = 120.00m,
                    ["Banho e Tosa na Tesoura"] = 150.00m
                }
            },

            // PORTE GRANDE
            new ServicoBase
            {
                Porte = "Grande",
                Raca = "Vira lata",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 90.00m,
                    ["Banho e Tosa Higiênica"] = 120.00m,
                    ["Banho e Tosa Verão"] = 140.00m,
                    ["Banho e Tosa na Máquina"] = 150.00m,
                    ["Banho e Tosa na Tesoura"] = 170.00m,
                    ["Banho e Triming"] = 160.00m,
                    ["Banho Carding"] = 120.00m
                }
            },
            new ServicoBase
            {
                Porte = "Grande",
                Raca = "Border Collie",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 80.00m,
                    ["Banho e Tosa Higiênica"] = 95.00m,
                    ["Banho e Triming"] = 140.00m
                }
            },
            new ServicoBase
            {
                Porte = "Grande",
                Raca = "Pastor Alemão",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 90.00m,
                    ["Banho e Tosa Higiênica"] = 105.00m,
                    ["Banho e Triming"] = 150.00m
                }
            },
            new ServicoBase
            {
                Porte = "Grande",
                Raca = "Husky",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 100.00m,
                    ["Banho e Tosa Higiênica"] = 120.00m,
                    ["Banho e Triming"] = 160.00m
                }
            },
            new ServicoBase
            {
                Porte = "Grande",
                Raca = "Dálmata",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 80.00m,
                    ["Banho e Tosa Verão"] = 120.00m,
                    ["Banho Carding"] = 140.00m
                }
            },
            new ServicoBase
            {
                Porte = "Grande",
                Raca = "Golden",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 100.00m,
                    ["Banho e Tosa Higiênica"] = 120.00m,
                    ["Banho e Triming"] = 160.00m
                }
            },
            new ServicoBase
            {
                Porte = "Grande",
                Raca = "São Bernardo",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 160.00m,
                    ["Banho e Tosa Higiênica"] = 180.00m,
                    ["Banho e Triming"] = 200.00m
                }
            },
            new ServicoBase
            {
                Porte = "Grande",
                Raca = "Rottweiler",
                Servicos = new Dictionary<string, decimal>
                {
                    ["Banho"] = 90.00m,
                    ["Banho Carding"] = 120.00m
                }
            }
        };
    }
}