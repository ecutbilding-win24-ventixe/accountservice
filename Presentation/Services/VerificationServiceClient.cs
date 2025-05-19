using VerificationServiceGrpcServer;

namespace Presentation.Services;

public class VerificationServiceClient(VerificationServiceProto.VerificationServiceProtoClient client)
{
    private readonly VerificationServiceProto.VerificationServiceProtoClient _client = client;

    public async Task<bool> ConfirmUserEmailAsync(string email)
    {
        var request = new ConfirmUserEmailRequest { Email = email };
        var response = await _client.ConfirmUserEmailAsync(request);
        return response.Success;
    }
}
