using System;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DevExpress.ExpressApp;
using System.ComponentModel;
using DevExpress.ExpressApp.DC;
using DevExpress.Data.Filtering;
using DevExpress.Persistent.Base;
using System.Collections.Generic;
using DevExpress.ExpressApp.Model;
using DevExpress.Persistent.BaseImpl;
using DevExpress.Persistent.Validation;

namespace XasSemanticKernelPredictiveSales.Module.BusinessObjects {
    [DefaultClassOptions]
    //[ImageName("BO_Contact")]
    //[DefaultProperty("DisplayMemberNameForLookupEditorsOfThisType")]
    //[DefaultListViewOptions(MasterDetailMode.ListViewOnly, false, NewItemRowPosition.None)]
    //[Persistent("DatabaseTableName")]
    // Specify more UI options using a declarative approach (https://docs.devexpress.com/eXpressAppFramework/112701/business-model-design-orm/data-annotations-in-data-model).
    public class Product : BaseObject { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://docs.devexpress.com/eXpressAppFramework/113146/business-model-design-orm/business-model-design-with-xpo/base-persistent-classes).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public Product(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place your initialization code here (https://docs.devexpress.com/eXpressAppFramework/112834/getting-started/in-depth-tutorial-winforms-webforms/business-model-design/initialize-a-property-after-creating-an-object-xpo?v=22.1).
        }

        string hashTags;
        string code;
        string description;
        string name;
        string category;
        string subCategory;
        decimal unitPrice;
        int unitsInStock;
        string unitOfMeasure;
        string barcode;
        string sku;
        bool isDiscontinued;
        int reorderLevel;
        string brand;
        double weight;
        int expiryDays;
        bool isRefrigerated;
        double vatRate;
        decimal supplierPrice;

        [Size(250)]
        public string Category
        {
            get => category;
            set => SetPropertyValue(nameof(Category), ref category, value);
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string SubCategory
        {
            get => subCategory;
            set => SetPropertyValue(nameof(SubCategory), ref subCategory, value);
        }

        public decimal UnitPrice
        {
            get => unitPrice;
            set => SetPropertyValue(nameof(UnitPrice), ref unitPrice, value);
        }

        public int UnitsInStock
        {
            get => unitsInStock;
            set => SetPropertyValue(nameof(UnitsInStock), ref unitsInStock, value);
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string UnitOfMeasure
        {
            get => unitOfMeasure;
            set => SetPropertyValue(nameof(UnitOfMeasure), ref unitOfMeasure, value);
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Barcode
        {
            get => barcode;
            set => SetPropertyValue(nameof(Barcode), ref barcode, value);
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string SKU
        {
            get => sku;
            set => SetPropertyValue(nameof(SKU), ref sku, value);
        }

        public bool IsDiscontinued
        {
            get => isDiscontinued;
            set => SetPropertyValue(nameof(IsDiscontinued), ref isDiscontinued, value);
        }

        public int ReorderLevel
        {
            get => reorderLevel;
            set => SetPropertyValue(nameof(ReorderLevel), ref reorderLevel, value);
        }

        [Size(200)]
        public string Brand
        {
            get => brand;
            set => SetPropertyValue(nameof(Brand), ref brand, value);
        }

        public double Weight
        {
            get => weight;
            set => SetPropertyValue(nameof(Weight), ref weight, value);
        }

        public int ExpiryDays
        {
            get => expiryDays;
            set => SetPropertyValue(nameof(ExpiryDays), ref expiryDays, value);
        }

        public bool IsRefrigerated
        {
            get => isRefrigerated;
            set => SetPropertyValue(nameof(IsRefrigerated), ref isRefrigerated, value);
        }

        public double VATRate
        {
            get => vatRate;
            set => SetPropertyValue(nameof(VATRate), ref vatRate, value);
        }

        public decimal SupplierPrice
        {
            get => supplierPrice;
            set => SetPropertyValue(nameof(SupplierPrice), ref supplierPrice, value);
        }
        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Name
        {
            get => name;
            set => SetPropertyValue(nameof(Name), ref name, value);
        }

        [Size(300)]
        public string Description
        {
            get => description;
            set => SetPropertyValue(nameof(Description), ref description, value);
        }

        [Size(SizeAttribute.DefaultStringMappingFieldSize)]
        public string Code
        {
            get => code;
            set => SetPropertyValue(nameof(Code), ref code, value);
        }
        
        [Size(SizeAttribute.Unlimited)]
        public string HashTags
        {
            get => hashTags;
            set => SetPropertyValue(nameof(HashTags), ref hashTags, value);
        }

        public override string ToString()
        {
            return $"""
        Product Details:
        Name: {Name}
        Code: {Code}
        Category: {Category}
        SubCategory: {SubCategory}
        Brand: {Brand}
        SKU: {SKU}
        Barcode: {Barcode}
        Description: {Description}
        HashTags: {HashTags}
        
        Pricing:
        Unit Price: {UnitPrice:C2}
        Supplier Price: {SupplierPrice:C2}
        VAT Rate: {VATRate:P2}
        
        Inventory:
        Units in Stock: {UnitsInStock}
        Unit of Measure: {UnitOfMeasure}
        Reorder Level: {ReorderLevel}
        Is Discontinued: {IsDiscontinued}
        
        Product Specs:
        Weight: {Weight:F2} kg
        Expiry Days: {ExpiryDays}
        Requires Refrigeration: {IsRefrigerated}
        """;
        }
    }
}