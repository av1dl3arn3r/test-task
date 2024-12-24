using Discount;
using DiscountGenerationService.Services;
using Grpc.Core;

namespace DiscountGenerationService.GrpcServices
{
    public class DiscountService : DiscountServiceGRPC.DiscountServiceGRPCBase
    {
        private readonly CodeRetrievalService _code_retrieval;
        private readonly CodeUtilisationlService _code_utilisation;

        public DiscountService(CodeRetrievalService code_retrieval, CodeUtilisationlService code_utilisation)
        {
            _code_retrieval = code_retrieval;
            _code_utilisation = code_utilisation;
        }

        async public override Task<GetDiscountReply> GetDiscountCodes(GetDiscountRequest request, ServerCallContext context)
        {
            var user_id = Guid.Parse(request.UserId);
            var codes = await _code_retrieval.GetDiscountCodesAsync(user_id, request.Count, (ushort)request.Length);
            var reply = new GetDiscountReply();
            reply.Codes.AddRange(codes);
            return reply;
        }

        async public override Task<UseDiscountReply> UseDiscountCode(UseDiscountRequest request, ServerCallContext context)
        {
            var status = await _code_utilisation.UseDiscountCodeAsync(request.Code);
            var reply = new UseDiscountReply();
            reply.Status = status;
            return reply;
        }
    }
}
