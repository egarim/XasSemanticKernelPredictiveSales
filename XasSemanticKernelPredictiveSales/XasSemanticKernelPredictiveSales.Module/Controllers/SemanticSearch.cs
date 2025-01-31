using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics.Tensors;
using System.Text;
using DevExpress.Data.Filtering;
using DevExpress.ExpressApp;
using DevExpress.ExpressApp.Actions;
using DevExpress.ExpressApp.Editors;
using DevExpress.ExpressApp.Layout;
using DevExpress.ExpressApp.Model.NodeGenerators;
using DevExpress.ExpressApp.SystemModule;
using DevExpress.ExpressApp.Templates;
using DevExpress.ExpressApp.Utils;
using DevExpress.Office.Utils;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Microsoft.Extensions.AI;
using OpenAI;
using XasSemanticKernelPredictiveSales.Module.BusinessObjects;

namespace XasSemanticKernelPredictiveSales.Module.Controllers {

    public class ComplementaryProductResult
    {
        public string Name { get; set; }
        public string SKU { get; set; }
        public string Brand { get; set; }
        public ComplementaryProductResult()
        {
            
        }
   
        public override string ToString()
        {
            return $"Name:{Name} SKU:{SKU} Brand:{Brand}";
        }
    }
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ViewController.
    public partial class SemanticSearch : ViewController {
        SimpleAction SearchComplementaryProducts;
        ParametrizedAction SearchVectors;
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public SemanticSearch()
        {
            InitializeComponent();
            SearchVectors = new ParametrizedAction(this, "SearchVectors", "View", typeof(string));
            SearchVectors.Execute += SearchVectors_Execute;


            SearchComplementaryProducts = new SimpleAction(this, "SearchComplementaryProducts", "View");
            SearchComplementaryProducts.Execute += SearchComplementaryProducts_Execute;
            

            // Target required Views (via the TargetXXX properties) and create their Actions.
        }

        private async void SearchComplementaryProducts_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            string modelId = "gpt-4o";
            string OpenAiKey = Environment.GetEnvironmentVariable("OpenAiTestKey");
            var client = new OpenAIClient(new System.ClientModel.ApiKeyCredential(OpenAiKey));
            var ChatClient = new OpenAIChatClient(client, modelId)
                .AsBuilder()
                .UseFunctionInvocation()
                .Build();


            //IChatClient ChatClient =
            //      new OllamaChatClient(new Uri("http://localhost:11434/"), "llama3.2:3b");


           
            var CurrentProduct= View.CurrentObject as ComplementaryProducts;

            var Message = new ChatMessage(ChatRole.User, $"based on this product {CurrentProduct.Product.ToString()} generate 10 keywords for a semantic search for complementary products products, for example if the product is potato chips, the complementary product might be a soda or a beer, avoid to generate keyword for the same product for example if the product is pizza, dont generate search terms for pizza");
            //var Result=await ChatClient.CompleteAsync<Quiz>(new List<ChatMessage>() { Message }); 

            var ChatCompletion = await ChatClient.CompleteAsync<List<string>>(new List<ChatMessage>() { Message });

            //Join the values of this string list in a single string ChatCompletion.Result
            var Keywords = string.Join(" ", ChatCompletion.Result);
            CurrentProduct.SearchKeywords = Keywords;
            var SearchResults=await Search(Keywords);
            var RelatedResults = new StringBuilder();
            foreach (var item in SearchResults)
            {
                RelatedResults.AppendLine($"{item.Text}");
            }
            CurrentProduct.ComplementaryProductList = RelatedResults.ToString();

            var AiSuggestionPrompt = new ChatMessage(ChatRole.User, $"Based on the search results:  {RelatedResults}, suggest 10 products that are complementary to the product {CurrentProduct.Product.ToString()}, only provide answers from the search results");
            var AiSuggestedProducts = await ChatClient.CompleteAsync<List<ComplementaryProductResult>>(new List<ChatMessage>() { AiSuggestionPrompt });
            CurrentProduct.AiSuggestedProductList = string.Join(System.Environment.NewLine, AiSuggestedProducts.Result);
            this.View.ObjectSpace.CommitChanges();
        }


     
        private async void SearchVectors_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            var parameterValue = (string)e.ParameterCurrentValue;
            var Closest = await Search(parameterValue);

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var item in Closest)
            {
                stringBuilder.AppendLine($"Code:{item.Code} Text:{item.Text} Similarity:{item.Similarity}");
            }

            MessageOptions options = new MessageOptions();
            options.Duration = 20000;
            options.Message = stringBuilder.ToString();
            options.Type = InformationType.Success;
            options.Web.Position = InformationPosition.Right;
            options.Win.Caption = "Success";
            options.Win.Type = WinMessageType.Toast;
            Application.ShowViewStrategy.ShowMessage(options);
        }

        private async Task<IEnumerable<dynamic>> Search(string parameterValue,int MaxResults=50)
        {
            IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = new OllamaEmbeddingGenerator(new Uri("http://127.0.0.1:11434"), modelId: "all-minilm:latest");
            Embedding<float> InputEmbedding = await embeddingGenerator.GenerateEmbeddingAsync(parameterValue);


            Debug.WriteLine($"vector lenght:{InputEmbedding.Vector.Length}");


            var Embeddings = this.View.ObjectSpace.GetObjectsQuery<XpoEmbedding>().ToList();

            var Closest = (from candidate in Embeddings
                           let similarity = TensorPrimitives.CosineSimilarity(candidate.GetEmbedding().Vector.Span, InputEmbedding.Vector.Span)
                           orderby similarity descending
                           select new { Text = candidate.Text, Similarity = similarity, Code = candidate.Code }).Take(MaxResults);

            //var Closest = (from candidate in Embeddings
            //               let similarity = TensorPrimitives.CosineSimilarity(candidate.GetEmbedding().Vector.Span, InputEmbedding.Vector.Span)
            //               orderby similarity ascending
            //               select new { Text = candidate.Text, Similarity = similarity, Code = candidate.Code }).Take(MaxResults);

            //I want the last 10 of Closest products


            return Closest;
        }

        protected override void OnActivated() {
            base.OnActivated(); 
            // Perform various tasks depending on the target View.
        }
        protected override void OnViewControlsCreated() {
            base.OnViewControlsCreated(); 
            // Access and customize the target View control.
        }
        protected override void OnDeactivated() {
            // Unsubscribe from previously subscribed events and release other references and resources.
            base.OnDeactivated();
        }
    }
}
