using System;
using System.Collections.Generic;
using System.Xml.Linq;
using System.Xml.XPath;

namespace dax.Extensions
{
    public static class XDocumentExtensions
    {
        public static bool HasNode(this XNode content, string xmlPath)
        {
            return content.XPathSelectElement(xmlPath) != null;
        }

        public static String GetNodeValue(this XNode content, string xmlPath)
        {
            var elem = content.XPathSelectElement(xmlPath);

            if (elem != null)
            {
                return elem.Value;
            }

            throw new InvalidOperationException("Cannot find node: " + xmlPath);
        }

        public static IEnumerable<XElement> GetNodes(this XNode content, string xPath)
        {
            return content.XPathSelectElements(xPath);
        }

        public static String GetSafeAttribute(this XElement content, string name, String defaultVal = null)
        {
            var attr = content.Attribute(name);

            return attr != null ? attr.Value : defaultVal;
        }

        public static String GetAttribute(this XElement content, string name)
        {
            var attr = content.Attribute(name);

            if (attr != null)
            {
                return attr.Value;
            }

            throw new InvalidOperationException(String.Format("Attribute '{0}' not found in node '{1}'", name, content.Name));
        }
    }
}
