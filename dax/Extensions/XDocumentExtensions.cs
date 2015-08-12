/*
 * Copyright 2015 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

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
