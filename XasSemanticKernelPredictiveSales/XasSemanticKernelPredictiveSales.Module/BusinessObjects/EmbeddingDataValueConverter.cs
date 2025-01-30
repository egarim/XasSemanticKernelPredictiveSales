using DevExpress.Xpo.Metadata;
using System;
using System.Linq;

namespace XasSemanticKernelPredictiveSales.Module.BusinessObjects
{


    using System;
    public class EmbeddingDataValueConverter : ValueConverter
    {


        public EmbeddingDataValueConverter()
        {

        }

        public override Type StorageType => typeof(string);

        public override object ConvertFromStorageType(object value)
        {
            return FloatArrayConverter.StringToArray(value.ToString());
        }

        public override object ConvertToStorageType(object value)
        {
            return FloatArrayConverter.ArrayToString((float[])value);
        }
    }
}