using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Net.Http;
using System.Text.Json;
using System.Text;
using System.Configuration;

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

    public class TokensVaue
    {
        public int InputTokens { get; set; }
        public int OutputTokens { get; set; }

    }

    public class QuestionOpenAi
    {
        private readonly HttpClient _httpClient;

        public QuestionOpenAi(HttpClient httpClient)
        {
            _httpClient = new HttpClient();
            var key = ConfigurationManager.AppSettings["OPENAI_KEY"];
            _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {key}");
        }


        public async Task<TokensVaue> TakeResult()
        {

                var text = Console.ReadLine();
                var question = new InputChatGPTModel(text);

               

                var requestBody = JsonSerializer.Serialize(question);


                var content = new StringContent(requestBody, Encoding.UTF8, "application/json");

                var completionResponse = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

                var responseBody = await completionResponse.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<OutputChatGPTModel>(responseBody, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            var answer = new TokensVaue()
            {
                InputTokens = responseObject.usage.prompt_tokens,
                OutputTokens = responseObject.usage.completion_tokens,
            };

               




            Console.WriteLine(responseObject.usage.total_tokens);
            Console.WriteLine(responseObject.usage.prompt_tokens);
            Console.WriteLine(responseObject.usage.completion_tokens);


            return answer;
        }
    }

    public class Program
    {
        public static async Task Main()
        {
            var inputValue = 0;
            var outputValue = 0;
            double price = 0;

            var questionOpenAI = new QuestionOpenAi(new HttpClient());
            for (int i = 0; i <= 20; i++)
            { 
                var result = await questionOpenAI.TakeResult();

                inputValue += result.InputTokens;
                outputValue += result.OutputTokens;

                Console.WriteLine($"Quantia de tokens do prompt {inputValue}");
                Console.WriteLine($"Quantia de tokens do output {outputValue}");


                if (inputValue +  >= 1000)
                {
                    price += 0.03;
                    inputValue -= 1000;
                    Console.WriteLine($"You have been charge U${price} 'till now");
                }

                if (outputValue >= 1000)
                {
                    price += 0.06;
                    outputValue -= 1000;
                    Console.WriteLine($"You have been charge U${price} 'till now");
                }


            }

        }
    }

}
