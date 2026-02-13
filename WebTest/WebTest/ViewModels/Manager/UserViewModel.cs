using WebTest.Models;

namespace WebTest.ViewModels.Manager;

public class UserViewModel
{
    public int Id { get; set; }
    public string UserName { get; set; } = string.Empty;
    public UserRole Role { get; set; }
}
