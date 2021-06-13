using System.ComponentModel.DataAnnotations;
namespace WYW
{
    public class inputModel
    {
        [Required]
        [RegularExpression(@"^[A-Z]{2}\d{3,4}$", ErrorMessage = "Wrong Flight No.")]
        public string Name { get; set; }
        private int iataLength = 2;

        public string getNumber()
        {
            return Name.Substring(iataLength);
        }
    }
}