using Microsoft.AspNetCore.Identity;

using VMAPP.Data.Models.Base;

namespace VMAPP.Data.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using VMAPP.Common;

    public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
    {
        public ApplicationUser()
        {
            this.Id = Guid.NewGuid().ToString();
            this.Roles = new HashSet<IdentityUserRole<string>>();
            this.Claims = new HashSet<IdentityUserClaim<string>>();
            this.Logins = new HashSet<IdentityUserLogin<string>>();
        }

        public string City { get; set; } = null!;

        [Required]
        [StringLength(GlobalConstant.AddressMaxLength, MinimumLength = GlobalConstant.AddressMinLength)]
        public string Address { get; set; } = null!;

        public DateTime CreatedOn { get; set; }
        public string? CreatedById { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public string? ModifiedById { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime? DeletedOn { get; set; }

        public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }
        public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
        public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

        public ICollection<InsurancePolicy> CreatedInsurancePolicies { get; set; } = new List<InsurancePolicy>();
        public ICollection<InsuranceCompany> CreatedInsuranceCompanies { get; set; } = new List<InsuranceCompany>();
        public ICollection<ServiceRecord> CreatedServiceRecords { get; set; } = new List<ServiceRecord>();
    }
}
