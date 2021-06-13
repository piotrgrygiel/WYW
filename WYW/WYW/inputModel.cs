using System.ComponentModel.DataAnnotations;
namespace WYW
{
    public class inputModel
    {
        [Required]
        [RegularExpression(@"^[A-Z]{3}\d{3,4}$", ErrorMessage = "Wrong Flight No.")]
        public string Name { get; set; }
        private int iataLength = 2;

        public string getNumber()
        {
            string number = Name.Substring(iataLength);
            for(int i=0; i < number.Length; i++)
            {
                if(System.Char.IsDigit(number[i]))
                {
                    number = number.Substring(i);
                    break;
                }
            }

            return number;
        }
    }
}