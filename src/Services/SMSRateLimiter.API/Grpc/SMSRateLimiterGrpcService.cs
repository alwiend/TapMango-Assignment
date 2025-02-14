using Grpc.Core;
using SMSRateLimiterAPI;

namespace SMSRateLimiter.API.Grpc;

public class SMSRateLimiterGrpcService(SMSRateLimiterServices services) : SMSRateLimiterGrpc.SMSRateLimiterGrpcBase {

    /// <summary>
    ///     A gRPC method to determine if a SMS message can be sent for a given phone number and account
    ///     Better performance over the HTTP implementation.
    /// </summary>
    /// <param name="request"></param>
    /// <param name="context"></param>
    /// <returns></returns>
    public override async Task<CanSendResponse> CanSendSMSMessage(CanSendSMSRequest request, ServerCallContext context) {
        try {
            if (request == null || string.IsNullOrEmpty(request.PhoneNumber) || string.IsNullOrEmpty(request.AccountId)) {
                return new CanSendResponse() { CanSend = false, Reason = "Invalid Request" };
            }

            var canSendResponse = await services.CanSendSMSMessageAsync(request.PhoneNumber, request.AccountId);

            return new CanSendResponse() { CanSend = canSendResponse.Item1, Reason = canSendResponse.Item2 };
        } catch (Exception ex) {
            services.Logger.LogError(ex, $"An error occured: {ex.Message}");
            return new CanSendResponse() {
                CanSend = false,
                Reason = ex.Message
            };
        }
    }
}
