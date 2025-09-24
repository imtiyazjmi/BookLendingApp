using Amazon.SimpleSystemsManagement;
using Amazon.SimpleSystemsManagement.Model;

namespace BookLendingApp.Services;

public class ParameterStoreService
{
    private readonly IAmazonSimpleSystemsManagement _ssmClient;

    public ParameterStoreService(IAmazonSimpleSystemsManagement ssmClient)
    {
        _ssmClient = ssmClient;
    }

    public async Task<string> GetParameterAsync(string parameterName)
    {
        try
        {
            var request = new GetParameterRequest
            {
                Name = parameterName,
                WithDecryption = true
            };

            var response = await _ssmClient.GetParameterAsync(request);
            return response.Parameter.Value;
        }
        catch (ParameterNotFoundException)
        {
            throw new InvalidOperationException($"Parameter {parameterName} not found in SSM");
        }
    }
}