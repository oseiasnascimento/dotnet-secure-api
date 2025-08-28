namespace Guardian.Application.Validations
{
    public class CPFValidator
    {
        public static bool Validate(string cpf)
        {
            if (cpf.Length != 11)
                return false;

            bool allDigitsEqual = true;
            for (int i = 1; i < cpf.Length; i++)
            {
                if (cpf[i] != cpf[i - 1])
                {
                    allDigitsEqual = false;
                    break;
                }
            }

            if (allDigitsEqual)
                return false;

            int[] digits = new int[11];
            for (int i = 0; i < 11; i++)
            {
                digits[i] = int.Parse(cpf[i].ToString());
            }

            int sum1 = 0;
            for (int i = 0; i < 9; i++)
            {
                sum1 += digits[i] * (10 - i);
            }

            int remainder1 = sum1 % 11;
            int checkDigit1 = (remainder1 < 2) ? 0 : (11 - remainder1);

            if (checkDigit1 != digits[9])
                return false;

            int sum2 = 0;
            for (int i = 0; i < 10; i++)
            {
                sum2 += digits[i] * (11 - i);
            }

            int remainder2 = sum2 % 11;
            int checkDigit2 = (remainder2 < 2) ? 0 : (11 - remainder2);

            return checkDigit2 == digits[10];
        }
    }
}