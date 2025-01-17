﻿using Amazon.Lambda.Core;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace EmployeesCRUDV3
{
    public class EmployeesV3
    {
        public string? GetEmployeeByIdHandle(string input, ILambdaContext context)
        {
            return input?.ToUpper();
        }
    }
}
