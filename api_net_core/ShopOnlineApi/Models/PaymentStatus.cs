namespace ShopOnline.Api.Models
{
    public enum PaymentStatus
    {
        Created = 0,
        Pending = 1,
        Approved = 2,
        Completed = 3,
        Failed = 4,
        Cancelled = 5
    }
}
