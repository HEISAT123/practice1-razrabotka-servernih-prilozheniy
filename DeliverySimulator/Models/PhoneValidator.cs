using System.Text.RegularExpressions;

namespace DeliverySimulator.Models;

/// <summary>
/// Проверка российских номеров телефона и приведение их к виду +7XXXXXXXXXX.
/// </summary>
public static class PhoneValidator
{
    private static readonly Regex PhonePattern = new Regex(@"^(\+7|8)\d{10}$");

    public static bool IsValid(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone))
        {
            return false;
        }

        return PhonePattern.IsMatch(Clean(phone));
    }

    /// <summary>
    /// Приводит номер к виду +7XXXXXXXXXX. Бросает исключение, если номер некорректен.
    /// </summary>
    public static string Normalize(string phone)
    {
        if (!IsValid(phone))
        {
            throw new ArgumentException(
                "Телефон должен быть в формате +7XXXXXXXXXX или 8XXXXXXXXXX.", nameof(phone));
        }

        string cleaned = Clean(phone);

        if (cleaned.StartsWith('8'))
        {
            cleaned = "+7" + cleaned.Substring(1);
        }

        return cleaned;
    }

    private static string Clean(string phone)
    {
        return Regex.Replace(phone, @"[\s\-()]", "");
    }
}
