namespace StockMaster.ViewModels.Admin
{
    public class UserListViewModel
    {
        public string Id { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? NomeCompleto { get; set; }
        public string? Reparto { get; set; }
        public bool IsAttivo { get; set; }
        public DateTime? UltimoAccesso { get; set; }
        public List<string> Roles { get; set; } = new();
    }
}