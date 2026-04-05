namespace VMAPP.Common
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class GlobalConstant
    {
        public const string SystemName = "VMAPP";
        public const string AdministratorRoleName = "ProgramAdministrator";
        public const string InsuranceCompanyRoleName = "InsuranceCompany";
        public const string VehicleServiceRoleName = "VehicleService";
        public const string VINRegex = @"^[A-HJ-NPR-Z0-9]{17}$";

        public const int VehicleServiceName = 150;
        public const int VehicleServiceDescription = 1000;
        public const int ServiceRecordDescription = 1500;
        public const int CarBrandLength = 100;
        public const int CarModelLength = 100;

        public const int CityMaxLength = 100;
        public const int CityMinLength = 3;

        public const int AddressMaxLength = 1024;
        public const int AddressMinLength = 3;

        public const int InsuranceCompanyName = 150;
        public const int InsuranceCompanyDescription = 1000;
        public const int InsurancePolicyNumber = 50;
        public const int InsuranceClaimDescription = 1500;
    }
}
