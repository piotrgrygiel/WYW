using System.ComponentModel.DataAnnotations;

namespace WYW.Pages
{
    public class InputModel
    {
        [Required]
        [RegularExpression(@".*\d{3,4}$", ErrorMessage = "Wrong Flight No.")]
        public string Name { get; set; }
        private int iataLength = 2;

        public string GetNumber()
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