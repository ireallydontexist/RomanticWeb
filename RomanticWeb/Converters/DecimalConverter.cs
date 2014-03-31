using System;
using System.Collections.Generic;
using System.Xml;
using RomanticWeb.Model;
using RomanticWeb.Vocabularies;

namespace RomanticWeb.Converters
{
    /// <summary>
    /// Converter for xsd:decimal
    /// </summary>
    public class DecimalConverter:XsdConverterBase
    {
        /// <summary>
        /// Gets Uri of xsd:decimal
        /// </summary>
        protected override IEnumerable<Uri> SupportedDataTypes
        {
            get
            {
                yield return Xsd.Decimal;
            }
        }
        
        public override Node ConvertBack(object value)
        {
            return Node.ForLiteral(XmlConvert.ToString((Decimal)value),Xsd.Decimal);
        }

        protected override object ConvertInternal(Node literalNode)
        {
            return XmlConvert.ToDecimal(literalNode.Literal);
        }
    }
}