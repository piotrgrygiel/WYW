using System.ComponentModel.DataAnnotations;
namespace WYW
{
    public class inputModel
    {
    [Required]
    [StringLength(10, ErrorMessage = "Name is too long.")]
    public string Name { get; set; }
    }
}