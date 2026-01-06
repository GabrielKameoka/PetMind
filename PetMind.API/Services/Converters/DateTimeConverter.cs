// Services/DateTimeConverter.cs

using System.Globalization;

namespace PetMind.API.Services.Converters
{
    public static class DateTimeConverter
    {
        public static DateTime ConverterParaDateTime(string dataString)
        {
            // Remove espaços extras
            dataString = dataString?.Trim() ?? "";
            
            // Tenta formato brasileiro primeiro: "dd/MM/yyyy HH:mm:ss"
            if (DateTime.TryParseExact(dataString, "dd/MM/yyyy HH:mm:ss", 
                    CultureInfo.GetCultureInfo("pt-BR"), DateTimeStyles.None, out DateTime result))
            {
                return result;
            }
            
            // Tenta formato brasileiro sem segundos: "dd/MM/yyyy HH:mm"
            if (DateTime.TryParseExact(dataString, "dd/MM/yyyy HH:mm", 
                    CultureInfo.GetCultureInfo("pt-BR"), DateTimeStyles.None, out result))
            {
                return result;
            }
            
            // Tenta só data: "dd/MM/yyyy"
            if (DateTime.TryParseExact(dataString, "dd/MM/yyyy", 
                    CultureInfo.GetCultureInfo("pt-BR"), DateTimeStyles.None, out result))
            {
                return result.AddHours(DateTime.Now.Hour).AddMinutes(DateTime.Now.Minute);
            }
            
            // Se não for formato brasileiro, tenta parse normal (aceita ISO também)
            if (DateTime.TryParse(dataString, CultureInfo.InvariantCulture, 
                    DateTimeStyles.None, out result))
            {
                return result;
            }
            
            throw new ArgumentException(
                $"Data inválida: '{dataString}'. " +
                $"Use formato: 'dd/MM/yyyy HH:mm:ss' (ex: 25/12/2024 14:30:00)");
        }
    }
}