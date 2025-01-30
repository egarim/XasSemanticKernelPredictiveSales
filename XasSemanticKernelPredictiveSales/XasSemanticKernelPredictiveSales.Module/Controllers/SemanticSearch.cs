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
using DevExpress.Persistent.Base;
using DevExpress.Persistent.Validation;
using Microsoft.Extensions.AI;
using XasSemanticKernelPredictiveSales.Module.BusinessObjects;

namespace XasSemanticKernelPredictiveSales.Module.Controllers {
    // For more typical usage scenarios, be sure to check out https://docs.devexpress.com/eXpressAppFramework/DevExpress.ExpressApp.ViewController.
    public partial class SemanticSearch : ViewController {
        ParametrizedAction SearchVectors;
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public SemanticSearch()
        {
            InitializeComponent();
            SearchVectors = new ParametrizedAction(this, "SearchVectors", "View", typeof(string));
            SearchVectors.Execute += SearchVectors_Execute;
            
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        private async void SearchVectors_Execute(object sender, ParametrizedActionExecuteEventArgs e)
        {
            var parameterValue = (string)e.ParameterCurrentValue;
            IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = new OllamaEmbeddingGenerator(new Uri("http://127.0.0.1:11434"), modelId: "all-minilm:latest");
            Embedding<float> InputEmbedding = await embeddingGenerator.GenerateEmbeddingAsync(parameterValue);


            Debug.WriteLine($"vector lenght:{InputEmbedding.Vector.Length}");


            var Embeddings = this.View.ObjectSpace.GetObjectsQuery<XpoEmbedding>().ToList();

            var Closest = from candidate in Embeddings
                          let similarity = TensorPrimitives.CosineSimilarity(candidate.GetEmbedding().Vector.Span, InputEmbedding.Vector.Span)
                          orderby similarity descending
                          select new { Text = candidate.Text, Similarity = similarity, Code = candidate.Code };


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
