using ClientWPF.Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace ClientWPF.Common
{
    public static class Client
    {
        public async static Task<HttpResponseMessage> GetAllEmployees() 
        {
            string url = "https://localhost:7230/api/Employee/GetAllEmployees";

            using  HttpClient client = new HttpClient();

            return await client.GetAsync(url);
        }

        public async static Task<HttpResponseMessage> UpdateRangeEmployees(List<EmployeeDto> employees)
        {
            string url = "https://localhost:7230/api/Employee/UpdateEmployeeRange";

            using HttpClient client = new HttpClient();

            return await client.PutAsJsonAsync(url, employees);
        }

    }
}
