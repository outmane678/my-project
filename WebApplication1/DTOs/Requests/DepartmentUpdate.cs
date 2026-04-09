namespace WebApplication1.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
public class DepartmentUpdate
{
     [Required, StringLength(100), Column("DepartmentName")]
    public string Name { get; set; } = null!;
}