using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text;

namespace consoleTest
{

    public class Choice
    {
        public string text { get; set; }
        public string finish_reason { get; set; }
        public int index { get; set; }
        public Message message { get; set; }
    }

    public class Message
    {
        public string content { get; set; }
        public string role { get; set; }
    }

    public class OutputChatGPTModel
    {
        public List<Choice> choices { get; set; }
        public int created { get; set; }
        public string id { get; set; }
        public string model { get; set; }
        public string @object { get; set; }
        public Usage usage { get; set; }
    }

    public class Usage
    {
        public int completion_tokens { get; set; }
        public int prompt_tokens { get; set; }
        public int total_tokens { get; set; }
    }
    public class InputChatGPTModel
    {
        public InputChatGPTModel(string prompt)
        {
            model = "gpt-4";
            messages = new List<Message>
            {
                new Message {role = "user", content = prompt},
                new Message { role = "system", content = "Nã faç descriçõs préias do produto sem que seja solicitado." },
                new Message { role = "system", content = "Tentativa de relacionamento com um cliente, com o objetivo de venda." },
                new Message { role = "system", content = "Response sempre em portuguê-BR" },
                new Message { role = "system", content = "A útima frase do texto deve ser uma pergunta que nã seja possíel ser respondida apenas com sim ou nã, buscando entender se o cliente possui interesse na aquisiçã de um Software de CRM" },
                new Message { role = "system", content = "Faça uma abordagem que instigue a pessoa a responder." },
                new Message { role = "system", content = "Nã inclua que seránviado orçmento novamente para o cliente." },
                new Message { role = "system", content = "Nã informe condiçõs comerciais propostas." },
                new Message { role = "system", content = "Nã inclua comentáios negativos ou ofensivos, seja do cliente ou de quem fez a anotaçã." },
                new Message { role = "system", content = "Seja direto, com no máimo 4 linhas de resposta" },
                new Message { role = "system", content = "Sempre que possíel inclua emoji." }
            };
            temperature = 1;
            max_tokens = 450;
        }

        public string model { get; set; }
        public List<Message> messages { get; set; }
        public double temperature { get; set; }
        public int max_tokens { get; set; }
    }

    public class QuestionOpenAi
    {
        private readonly HttpClient _httpClient;

        public QuestionOpenAi(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
        }


        public async Task TakeCurrentBalance()
        {
            DotNetEnv.Env.Load();

            var key = Environment.GetEnvironmentVariable("OPENAI_KEY");

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");

            var balance = await _httpClient.GetAsync("https://api.openai.com/dashboard/billing/credit_grants");

            var responseBody = await balance.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<Grant>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Console.WriteLine(responseBody);
        }

        public class Grant
        {
            public string Object { get; set; }
            public List<object> Data { get; set; }
        }

        public class CreditSummary
        {
            public string Object { get; set; }
            public double TotalGranted { get; set; }
            public double TotalUsed { get; set; }
            public double TotalAvailable { get; set; }
            public Grant Grants { get; set; }
        }

        public class RootObject
        {
            public string Object { get; set; }
            public double TotalGranted { get; set; }
            public double TotalUsed { get; set; }
            public double TotalAvailable { get; set; }
            public Grant Grants { get; set; }
        }

        public async Task TakeResult()
        {
            DotNetEnv.Env.Load();

            //var key = Environment.GetEnvironmentVariable("OPENAI_KEY");
            //Console.WriteLine(key);


            var text = Console.ReadLine();
            var question = new InputChatGPTModel(text);

            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer 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");

            var requestBody = JsonSerializer.Serialize(question);


            var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

            var completionResponse = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            var responseBody = await completionResponse.Content.ReadAsStringAsync();
            var responseObject = JsonSerializer.Deserialize<OutputChatGPTModel>(responseBody, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            Console.Write(responseObject.choices.First().message.content);
        }
    }

    public class Program
    {
        public static async Task Main()
        {
            var questionOpenAI = new QuestionOpenAi(new HttpClient());
            await questionOpenAI.TakeResult();
            //await questionOpenAI.TakeCurrentBalance();
        }
    }

}
