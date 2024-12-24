namespace DiscountGenerationService.Data.Entities
{
    public class UserToShardMapping
    {
        public required Guid UserId { get; set; }
        public required char ShardLabel { get; set; }
    }
}