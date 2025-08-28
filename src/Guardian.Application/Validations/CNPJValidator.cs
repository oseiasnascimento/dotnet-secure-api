using System.Globalization;

namespace Guardian.Application.Validations;
public class CNPJValidator
{
    public static bool Validate(string cnpj)
    {
        if (string.IsNullOrEmpty(cnpj))
            return false;

        cnpj = new string(cnpj.Where(char.IsDigit).ToArray());

        if (cnpj.Length != 14)
            return false;

        int[] multiplier1 = { 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };
        int[] multiplier2 = { 6, 5, 4, 3, 2, 9, 8, 7, 6, 5, 4, 3, 2 };

        string tempCnpj = cnpj.Substring(0, 12);
        int sum = 0;

        for (int i = 0; i < 12; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * multiplier1[i];

        int remainder = (sum % 11);
        int digit1 = remainder < 2 ? 0 : 11 - remainder;

        tempCnpj += digit1;
        sum = 0;

        for (int i = 0; i < 13; i++)
            sum += int.Parse(tempCnpj[i].ToString()) * multiplier2[i];

        remainder = (sum % 11);
        int digit2 = remainder < 2 ? 0 : 11 - remainder;

        string validCnpj = tempCnpj + digit2;

        return cnpj == validCnpj;
    }
}
