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
    public class ComplementaryProducts : BaseObject { // Inherit from a different class to provide a custom primary key, concurrency and deletion behavior, etc. (https://docs.devexpress.com/eXpressAppFramework/113146/business-model-design-orm/business-model-design-with-xpo/base-persistent-classes).
        // Use CodeRush to create XPO classes and properties with a few keystrokes.
        // https://docs.devexpress.com/CodeRushForRoslyn/118557
        public ComplementaryProducts(Session session)
            : base(session) {
        }
        public override void AfterConstruction() {
            base.AfterConstruction();
            // Place your initialization code here (https://docs.devexpress.com/eXpressAppFramework/112834/getting-started/in-depth-tutorial-winforms-webforms/business-model-design/initialize-a-property-after-creating-an-object-xpo?v=22.1).
        }



        string aiSuggestedProductList;
        string searchKeywords;
        string searchTerms;
        string complementaryProductList;
        Product product;



        public Product Product
        {
            get => product;
            set => SetPropertyValue(nameof(Product), ref product, value);
        }



        [Size(SizeAttribute.Unlimited)]
        public string SearchKeywords
        {
            get => searchKeywords;
            set => SetPropertyValue(nameof(SearchKeywords), ref searchKeywords, value);
        }
        
        [Size(SizeAttribute.Unlimited)]
        public string AiSuggestedProductList
        {
            get => aiSuggestedProductList;
            set => SetPropertyValue(nameof(AiSuggestedProductList), ref aiSuggestedProductList, value);
        }
        [Size(SizeAttribute.Unlimited)]
        public string ComplementaryProductList
        {
            get => complementaryProductList;
            set => SetPropertyValue(nameof(ComplementaryProductList), ref complementaryProductList, value);
        }
    }
}