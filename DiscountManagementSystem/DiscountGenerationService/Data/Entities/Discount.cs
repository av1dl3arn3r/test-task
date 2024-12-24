namespace DiscountGenerationService.Data.Entities
{
    public class Discount
    {
        public required DateTime CreationTime { get; set; }
        public required string Code { get; set; }
    }

    public class AvailableDiscount7 : Discount { }
    public class AvailableDiscount8 : Discount { }
    public class EmittedDiscount7 : Discount { }
    public class EmittedDiscount8 : Discount { }

    public class UtilisedDiscount7 : Discount { }
    public class UtilisedDiscount8 : Discount { }
}
