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
using DevExpress.Xpo.Metadata;
using Microsoft.Extensions.AI;

namespace XasSemanticKernelPredictiveSales.Module.BusinessObjects {


    using System;
    using System.Linq;
    using System.Globalization;

    public class FloatArrayConverter
    {
        private const string DEFAULT_SEPARATOR = ",";
        private static readonly CultureInfo INVARIANT_CULTURE = CultureInfo.InvariantCulture;

        /// <summary>
        /// Converts a float array to a string with the specified separator
        /// </summary>
        /// <param name="array">The float array to convert</param>
        /// <param name="separator">The separator to use between values (default is comma)</param>
        /// <returns>A string representation of the float array, or empty string if array is null</returns>
        public static string ArrayToString(float[] array, string separator = DEFAULT_SEPARATOR)
        {
            if (array == null || array.Length == 0)
                return string.Empty;

            return string.Join(separator, array.Select(x => x.ToString(INVARIANT_CULTURE)));
        }

        /// <summary>
        /// Converts a string back to a float array
        /// </summary>
        /// <param name="str">The string to convert</param>
        /// <param name="separator">The separator used between values (default is comma)</param>
        /// <returns>A float array parsed from the string, or empty array if string is null or empty</returns>
        public static float[] StringToArray(string str, string separator = DEFAULT_SEPARATOR)
        {
            if (string.IsNullOrEmpty(str))
                return Array.Empty<float>();

            try
            {
                return str.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                         .Select(x => float.Parse(x.Trim(), INVARIANT_CULTURE))
                         .ToArray();
            }
            catch (FormatException)
            {
                return Array.Empty<float>();
            }
        }
    }
}