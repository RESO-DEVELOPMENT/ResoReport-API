using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ResoReportDataService.Commons
{

    public enum AccountStatus
    {
        Active,
        Deactivate
    }

    public enum BrandStatus
    {
        Active
    }

    public enum CategoryStatus
    {
        Active
    }

    public enum CategoryType
    {
        Normal,
        Extra
    }

    public enum CollectionStatus
    {
        Active,
        Deactivate
    }

    public enum DateFilters
    {
        Sunday = 1,
        Monday = 2,
        Tuesday = 4,
        Wednesday = 8,
        Thursday = 16,
        Friday = 32,
        Saturday = 64,
    }

    public enum GroupCombinationMode
    {
        FIXED,
        DYNAMIC
    }

    public enum GroupProductStatus
    {
        Active,
        Deactivate
    }

    public enum MenuProductStatus
    {
        Active,
        Deactivate
    }

    public enum MenuStatus
    {
        Active,
        Deactivate
    }

    public enum OrderStatus
    {
        PENDING,
        PAID,
        CANCELED
    }

    public enum OrderType
    {
        EAT_IN,
        TAKE_AWAY,
        DELIVERY
    }

    public enum PaymentTypeEnum
    {
        CASH,
        MOMO,
        BANKING,
        VISA,
        W3W
    }

    public enum ProductInGroupStatus
    {
        Active,
        Deactivate
    }

    public enum ProductSize
    {
        S,
        M,
        L
    }

    public enum ProductStatus
    {
        Active,
        Deactive
    }

    public enum ProductType
    {
        SINGLE,
        PARENT,
        EXTRA,
        CHILD,
        COMBO
    }

    public enum PromotionEnum
    {
        Amount,
        Percent,
        Product,
        AutoApply
    }

    public enum PromotionStatus
    {
        Active,
        Deactive
    }

    public enum RoleEnum
    {
        SysAdmin,
        BrandManager,
        BrandAdmin,
        StoreManager,
        Staff
    }

    public enum StoreStatus
    {
        Active
    }
}