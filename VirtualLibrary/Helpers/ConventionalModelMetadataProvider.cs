using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace VirtualLibrary.Helper
{
    public static class StringExtensions
    {
        public static string SplitWords(this string value)
        {
            if (value != null) 
            {
                value = value.First().ToString().ToUpper() + value.Substring(1);
                return Regex.Replace(value, "([a-z](?=[A-Z0-9])|[A-Z](?=[A-Z][a-z]))", "$1 ").Trim(); 
            }
            else 
            {
                return null;
            }
        }
    }

    public class ConventionalModelMetadataProvider : DataAnnotationsModelMetadataProvider
    {
        protected override ModelMetadata CreateMetadata(IEnumerable<Attribute> attributes, Type containerType, Func<object> modelAccessor, Type modelType, string propertyName)
        {
            ModelMetadata modelMetadata = base.CreateMetadata(attributes, containerType, modelAccessor, modelType, propertyName);

            if (modelMetadata.DisplayName == null)
            {
                modelMetadata.DisplayName = modelMetadata.PropertyName.SplitWords();
            }

            return modelMetadata;
        }
    }
}