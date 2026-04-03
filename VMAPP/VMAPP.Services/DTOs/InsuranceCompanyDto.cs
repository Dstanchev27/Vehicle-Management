namespace VMAPP.Services.DTOs
{
    public class InsuranceCompanyDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public DateTime CreatedOn { get; set; }
        public string? CreatedById { get; set; }
        public string? ModifiedById { get; set; }
    }
}
