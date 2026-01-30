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
        public const string AdministratorRoleName = "Administrator";
        public const string VINRegex = @"^[A-HJ-NPR-Z0-9]{17}$";
        public const int VehicleServiceName = 150;
        public const int VehicleServiceDescription = 1000;
        public const int ServiceRecordDescription = 1500;
        public const int CarBrandLength = 100;
        public const int CarModelLength = 100;
    }
}
