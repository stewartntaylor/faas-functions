using System;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Function
{
    public class FunctionHandler
    {
        public Task<string> Handle(string input)
        {
            //return Task.FromResult($"Hello! Your input was {input}");

            string machine = System.Environment.MachineName;
            return Task.FromResult(JsonConvert.SerializeObject(new { inputText = input, machineName = machine }));
        }
    }
}
