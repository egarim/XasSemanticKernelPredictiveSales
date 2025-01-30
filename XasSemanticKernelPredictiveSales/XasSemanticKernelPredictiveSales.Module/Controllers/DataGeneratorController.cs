using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
    public partial class DataGeneratorController : ViewController {
        SimpleAction GenerateData;
        // Use CodeRush to create Controllers and Actions with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/403133/
        public DataGeneratorController()
        {
            InitializeComponent();
            GenerateData = new SimpleAction(this, "Generate Data", "View");
            GenerateData.Execute += GenerateData_Execute;
            
            // Target required Views (via the TargetXXX properties) and create their Actions.
        }
        private async void GenerateData_Execute(object sender, SimpleActionExecuteEventArgs e)
        {
            var random = new Random();
            var productData = GetProductCategories();
            IEmbeddingGenerator<string, Embedding<float>> embeddingGenerator = new OllamaEmbeddingGenerator(new Uri("http://127.0.0.1:11434"), modelId: "all-minilm:latest");


        
            for (int i = 0; i < 500; i++)
            {
                var product = ObjectSpace.CreateObject<Product>();
                var category = productData.Keys.ElementAt(random.Next(productData.Keys.Count));
                var subcategory = productData[category].ElementAt(random.Next(productData[category].Count));

                product.Name = GenerateProductName(category, subcategory, random);
                product.Category = category;
                product.SubCategory = subcategory;
                product.UnitPrice = CalculateRealisticPrice(category, subcategory, random);
                product.UnitsInStock = CalculateRealisticStock(category, random);
                product.UnitOfMeasure = DetermineUnitOfMeasure(category, subcategory);
                product.Barcode = GenerateEAN13();
                product.SKU = GenerateSKU(category, i);
                product.IsDiscontinued = random.Next(100) < 3; // 3% chance
                product.ReorderLevel = CalculateReorderLevel(category, random);
                product.Description = GenerateDescription(category, subcategory);
                product.Brand = GetRandomBrand(category, random);
                product.Weight = (double)CalculateWeight(category, subcategory, random) ;
                product.ExpiryDays = CalculateExpiryDays(category);
                product.IsRefrigerated = RequiresRefrigeration(category);
                product.VATRate = (double)DetermineVATRate(category);
                product.SupplierPrice = (decimal)Math.Round(((double)product.UnitPrice) * 0.7, 2); // 30% margin

                var CurrentXpoEmbedding= this.ObjectSpace.CreateObject<XpoEmbedding>();
                CurrentXpoEmbedding.Text = product.ToString();
                Embedding<float> result = await embeddingGenerator.GenerateEmbeddingAsync(CurrentXpoEmbedding.Text);
                Debug.WriteLine($"vector lenght:{result.Vector.Length}");

                CurrentXpoEmbedding.Data = result.Vector.ToArray();

            }

            ObjectSpace.CommitChanges();
        }

        private Dictionary<string, List<string>> GetProductCategories()
        {
            return new Dictionary<string, List<string>>
            {
                ["Fresh Produce"] = new List<string> {
            "Vegetables", "Fruits", "Herbs", "Organic Produce", "Mushrooms", "Salad Mixes"
        },
                ["Dairy & Eggs"] = new List<string> {
            "Milk", "Cheese", "Yogurt", "Butter", "Eggs", "Cream", "Plant-based Alternatives"
        },
                ["Meat & Seafood"] = new List<string> {
            "Beef", "Pork", "Poultry", "Fish", "Shellfish", "Deli Meats", "Plant-based Meats"
        },
                ["Bakery"] = new List<string> {
            "Bread", "Pastries", "Cakes", "Cookies", "Tortillas", "Bagels", "Artisan Bread"
        },
                ["Pantry"] = new List<string> {
            "Pasta", "Rice", "Canned Goods", "Condiments", "Oils", "Spices", "Baking Supplies"
        },
                ["Beverages"] = new List<string> {
            "Soft Drinks", "Water", "Coffee", "Tea", "Juices", "Energy Drinks", "Alcoholic Beverages"
        },
                ["Snacks"] = new List<string> {
            "Chips", "Crackers", "Nuts", "Chocolate", "Candy", "Dried Fruits", "Popcorn"
        },
                ["Frozen Foods"] = new List<string> {
            "Frozen Meals", "Ice Cream", "Frozen Vegetables", "Pizza", "Frozen Meat", "Frozen Desserts"
        }
            };
        }

        private string GenerateProductName(string category, string subcategory, Random random)
        {
            var brands = GetBrandNames(category);
            var brand = brands[random.Next(brands.Count)];
            var descriptors = GetDescriptors(subcategory);
            var descriptor = descriptors[random.Next(descriptors.Count)];
            return $"{brand} {descriptor} {subcategory}";
        }

        private decimal CalculateRealisticPrice(string category, string subcategory, Random random)
        {
            var basePrice = category switch
            {
                "Fresh Produce" => random.Next(1, 8),
                "Dairy & Eggs" => random.Next(2, 10),
                "Meat & Seafood" => random.Next(8, 30),
                "Bakery" => random.Next(2, 15),
                "Pantry" => random.Next(2, 12),
                "Beverages" => random.Next(1, 15),
                "Snacks" => random.Next(2, 8),
                "Frozen Foods" => random.Next(4, 15),
                _ => random.Next(2, 10)
            };

            return Math.Round(basePrice + (random.Next(1, 100) / 100.0M), 2);
        }

        private string GenerateEAN13()
        {
            var random = new Random();
            string code = "20"; // Country code for internal store usage
            for (int i = 0; i < 10; i++)
                code += random.Next(10).ToString();

            // Calculate check digit (simplified)
            return code + "0";
        }

        private string GenerateSKU(string category, int index)
        {
            return $"{category.Substring(0, 2).ToUpper()}{index:D6}";
        }

        private int CalculateRealisticStock(string category, Random random)
        {
            return category switch
            {
                "Fresh Produce" => random.Next(50, 300),
                "Dairy & Eggs" => random.Next(100, 400),
                "Meat & Seafood" => random.Next(30, 150),
                "Bakery" => random.Next(50, 200),
                _ => random.Next(50, 500)
            };
        }

        private string DetermineUnitOfMeasure(string category, string subcategory)
        {
            return (category, subcategory) switch
            {
                ("Fresh Produce", _) => "kg",
                ("Meat & Seafood", _) => "kg",
                ("Dairy & Eggs", "Milk") => "L",
                ("Beverages", _) => "L",
                _ => "unit"
            };
        }

        private decimal CalculateWeight(string category, string subcategory, Random random)
        {
            return Math.Round(category switch
            {
                "Fresh Produce" => random.Next(100, 1000) / 1000.0M,
                "Meat & Seafood" => random.Next(200, 2000) / 1000.0M,
                "Dairy & Eggs" => random.Next(200, 1000) / 1000.0M,
                _ => random.Next(100, 5000) / 1000.0M
            }, 3);
        }

        private int CalculateExpiryDays(string category)
        {
            return category switch
            {
                "Fresh Produce" => 7,
                "Dairy & Eggs" => 14,
                "Meat & Seafood" => 5,
                "Bakery" => 5,
                "Frozen Foods" => 180,
                _ => 365
            };
        }

        private bool RequiresRefrigeration(string category)
        {
            return category switch
            {
                "Fresh Produce" => true,
                "Dairy & Eggs" => true,
                "Meat & Seafood" => true,
                "Frozen Foods" => true,
                _ => false
            };
        }

        private decimal DetermineVATRate(string category)
        {
            return category switch
            {
                "Fresh Produce" => 0.0M,
                "Dairy & Eggs" => 0.0M,
                "Bakery" => 0.0M,
                _ => 0.21M
            };
        }
        private List<string> GetBrandNames(string category)
        {
            return category switch
            {
                "Fresh Produce" => new List<string> {
            "Nature's Best", "Farm Fresh", "Organic Valley", "Green Fields", "Fresh Choice",
            "Earth's Bounty", "Garden Fresh", "Sun Ripened", "Valley Green", "Fresh Picks"
        },
                "Dairy & Eggs" => new List<string> {
            "Dairy Pure", "Meadow Gold", "Green Pastures", "Farm Fresh", "Nature's Dairy",
            "Happy Cow", "Pure Dairy", "Golden Fields", "Morning Fresh", "Alpine Dairy"
        },
                "Meat & Seafood" => new List<string> {
            "Premium Cut", "Butcher's Choice", "Sea Fresh", "Prime Select", "Quality Meats",
            "Ocean Catch", "Farm Select", "Fresh Market", "Master Cut", "Atlantic Fresh"
        },
                "Bakery" => new List<string> {
            "Golden Crust", "Artisan Baker", "Fresh Bake", "Heritage Ovens", "Master Baker",
            "Rustic Loaf", "Baker's Best", "Hearth & Home", "Classic Bake", "Morning Fresh"
        },
                "Pantry" => new List<string> {
            "Essential Choice", "Pantry Select", "Kitchen Basics", "Chef's Choice", "Premium Stock",
            "Culinary Classic", "Kitchen Staples", "Gourmet Choice", "Basic Essentials", "Prime Pantry"
        },
                "Beverages" => new List<string> {
            "Refresh Co", "Pure Spring", "Beverage Bros", "Thirst Quencher", "Crystal Clear",
            "Mountain Spring", "Valley Fresh", "Pure Delight", "Fresh Sip", "Natural Flow"
        },
                "Snacks" => new List<string> {
            "Snack Time", "Crunch Master", "Tasty Bites", "Snack Attack", "Munch Magic",
            "Flavor Burst", "Snack Smart", "Crispy Choice", "Happy Snacks", "Premium Munch"
        },
                "Frozen Foods" => new List<string> {
            "Frost Fresh", "Frozen Select", "Arctic Choice", "Freeze Fresh", "Cool Kitchen",
            "Frozen Gourmet", "Icy Fresh", "Premium Frozen", "Arctic Delights", "Fresh Frozen"
        },
                _ => new List<string> {
            "Generic Brand", "Store Brand", "Value Choice", "Essential Pick", "Basic Select"
        }
            };
        }

        private List<string> GetDescriptors(string subcategory)
        {
            return subcategory switch
            {
                "Vegetables" => new List<string> {
            "Fresh", "Crispy", "Garden", "Premium", "Organic", "Local", "Selected", "Natural"
        },
                "Fruits" => new List<string> {
            "Ripe", "Sweet", "Fresh", "Premium", "Organic", "Juicy", "Hand-Picked", "Selected"
        },
                "Dairy" => new List<string> {
            "Fresh", "Creamy", "Pure", "Natural", "Premium", "Farm Fresh", "Organic", "Classic"
        },
                _ => new List<string> {
            "Premium", "Classic", "Signature", "Select", "Choice", "Deluxe", "Quality", "Fresh"
        }
            };
        }

        private string GenerateDescription(string category, string subcategory)
        {
            var random = new Random();
            var descriptors = new List<string> {
        "High-quality", "Premium", "Fresh", "Hand-selected", "Carefully sourced",
        "Sustainably produced", "Locally sourced", "Finest quality"
    };

            var benefits = new List<string> {
        "perfect for your daily needs",
        "bringing quality to your table",
        "meeting the highest standards",
        "satisfying customers since 1990",
        "your trusted choice"
    };

            return $"{descriptors[random.Next(descriptors.Count)]} {category.ToLower()} product from " +
                   $"the {subcategory.ToLower()} range, {benefits[random.Next(benefits.Count)]}";
        }

        private string GetRandomBrand(string category, Random random)
        {
            var brands = GetBrandNames(category);
            return brands[random.Next(brands.Count)];
        }

        private int CalculateReorderLevel(string category, Random random)
        {
            return category switch
            {
                "Fresh Produce" => random.Next(30, 100),
                "Dairy & Eggs" => random.Next(50, 150),
                "Meat & Seafood" => random.Next(20, 80),
                "Bakery" => random.Next(25, 75),
                "Frozen Foods" => random.Next(30, 120),
                _ => random.Next(40, 100)
            };
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
