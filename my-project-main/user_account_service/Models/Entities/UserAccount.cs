using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace user_account_service.Models.Entities;
[Table("UserAccounts")]
public class UserAccount
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    [Column("id")]
    public Guid Id { get; set; }
    [Required]
    [Column("username")]
    [StringLength(50)]
    public string Username { get; set; }
    [Required]
    [Column("password")]
    [MaxLength(100)]
    public string Password { get; set; }
    [Required]
    [MaxLength(20)]
    public string Role { get; set; } = "USER";
    [Required]
    public Guid EmployeeId { get; set; }
}