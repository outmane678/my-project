namespace WebApplication1.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class DepartmentCreate
{
     [Required, StringLength(100)]
    public string Name { get; set; } = null!;
}