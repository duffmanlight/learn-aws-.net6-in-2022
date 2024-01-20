using Amazon.Lambda.Core;
using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2.DocumentModel;
using EmployeesCRUDV2.Models;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EmployeesCRUDV2
{
    public class EmployeesLambdaV2
    {
        public async Task<EmployeeResponseDto>  GetEmployeeByIdHandlerAsync(string employeeId, ILambdaContext context)
        {
            EmployeeResponseDto employeeResponseDto = new();

            if (string.IsNullOrEmpty(employeeId))
            {
                context.Logger.LogDebug($"Did NOT receive the valid request for Employee.");

                employeeResponseDto.Message = "Received Invalid Employee Id";

                return employeeResponseDto;
            }

            context.Logger.LogDebug($"Received the request with Employee Id {employeeId}.");

            var _dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
            EmployeeDto employeeDto = await _dynamoDbContext.LoadAsync<EmployeeDto>(employeeId);

            context.Logger.LogDebug($"Sending the response for Employee Id {employeeId}.");

            employeeResponseDto = GenerateEmployeeResponseDto(employeeDto);

            return employeeResponseDto;
        }
        public async Task<IEnumerable<EmployeeDto>> GetAllEmployeesHandlerAsync(ILambdaContext context)
        {
            context.Logger.LogDebug($"Received the request to return all employees.");

            var _dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());
            var table = _dynamoDbContext.GetTargetTable<EmployeeDto>();
            var scanOps = new ScanOperationConfig();
            
            // returns the set of Document objects for the supplied ScanOption
            var scanResults = table.Scan(scanOps);
            List<Document> documentList = await scanResults.GetNextSetAsync();
            IEnumerable<EmployeeDto> allEmployees = _dynamoDbContext.FromDocuments<EmployeeDto>(documentList);
            return allEmployees;
        }
        public async Task<EmployeeResponseDto> PostEmployeeHandlerAsync(EmployeeDto newEmployeeRequestDto, ILambdaContext context)
        {
            context.Logger.LogDebug($"Received the request with Employee Id {newEmployeeRequestDto.employeeId} to create new.");

            var _dynamoDbContext = new DynamoDBContext(new AmazonDynamoDBClient());

            await _dynamoDbContext.SaveAsync(newEmployeeRequestDto);

            context.Logger.LogDebug($"Sending the response for New Employee Id {newEmployeeRequestDto.employeeId}.");

            EmployeeResponseDto employeeResponseDto = GenerateEmployeeResponseDto(newEmployeeRequestDto, "New Employee Created");

            return employeeResponseDto;
        }

        private static EmployeeResponseDto GenerateEmployeeResponseDto(EmployeeDto employeeDto, string message = "Record Found")
        {
            EmployeeResponseDto employeeResponseDto = new();
            if (employeeDto != null)
            {
                employeeResponseDto.Success = true;
                employeeResponseDto.Message = message;
                employeeResponseDto.Employee = employeeDto;
            }

            return employeeResponseDto;
        }
    }
}
