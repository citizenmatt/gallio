﻿// Copyright 2005-2011 Gallio Project - http://www.gallio.org/
// Portions Copyright 2000-2004 Jonathan de Halleux
// 
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
// 
//     http://www.apache.org/licenses/LICENSE-2.0
// 
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace Gallio.Framework.Data.DataObjects
{
    /// <summary>
    /// Builder class translates Xml Documents into dynamic XmlDataObject object graphs
    /// which can then be conveniently accessed using object property accessors.
    /// <remarks>
    /// <para>
    /// Take the following XML Document:
    /// 
    /// <Adaptive raw="6" ss="75" z="-1.67">
    ///     <SelfCare raw="6" ss="5" z="-1.67" ae="4">
    ///         <question id="SC" answer="a" />
    ///         <question id="SC" answer="b" />
    ///         <question id="SC" answer="c" />
    ///     </SelfCare>
    ///     <PersonalResponsibility raw="" ss="" z="" ae="">Sample Inner Text 123</PersonalResponsibility>
    /// </Adaptive>
    /// 
    /// XmlDataObject xmlDataObject = XmlDataObjectBuilder.EmitDataObject(Element);
    /// Console.WriteLine(xmlDataObject.Adapative.PersonalResponsibility.Value); // Prints "Sample Inner Text 123"
    /// Console.WriteLine(xmlDataObject.Adapative.raw); // Prints "6"
    /// Console.WriteLine(xmlDataObject.Adapative.SelfCare.ss); // Prints "5"
    /// Console.WriteLine(xmlDataObject.Adapative.SelfCare.question[0].answer); // Prints "a"
    /// Console.WriteLine(xmlDataObject.Adapative.SelfCare.question[2].answer); // Prints "c"
    /// 
    /// 1.) Elements are accessed by their name
    /// 2.) The inner text of an Element is accessed by ".Value" after the Element name
    /// 3.) Attributes like "raw" are accessed as properties
    /// 4.) Multiple Elements are stored as a List of XmlDataObjects.  This is why in the example
    /// "question" elements are accessed by index
    /// </para>
    /// </remarks>
    /// </summary>
    public class XmlDataObjectBuilder
    {
        /// <summary>
        /// Returns XmlDataObject containing dynamic representation of Xml document starting
        /// with Document's root node
        /// </summary>
        public static dynamic EmitDataObject(string FilePath)
        {
            return EmitDataObject(XDocument.Load(FilePath));
        }

        /// <summary>
        /// Returns XmlDataObject containing dynamic representation of Xml document starting
        /// with Document's root node
        /// </summary>
        public static dynamic EmitDataObject(XDocument Document)
        {
            return EmitDataObject(Document.Root);
        }

        /// <summary>
        /// Translates the Element and all of its children into a tree structure
        /// using nested instances of XmlDataObject's.
        /// </summary>
        public static dynamic EmitDataObject(XElement Element)
        {
            // Remove all of the namespaces - overwrite local reference to original Element
            Element = XmlDataObjectBuilder.RemoveAllNamespaces(Element);

            // Initialize the return value
            XmlDataObject DataObject = new XmlDataObject(Element.Name.ToString());

            // Step 1 - process all of the attributes
            foreach (XAttribute Attribute in Element.Attributes())
            {
                DataObject.TrySetMember(Attribute.Name.ToString(), Attribute.Value);
            }

            // Step 2 - is this an Element without children
            if (Element.Elements().Count<XElement>() == 0)
            {
                DataObject.TrySetMember("Value", Element.Value);
                return DataObject;
            }

            // Step 3 - there are child Elements, so we'll cycle through each child Element and process
            foreach (string ElementName in UniqueElementNames(Element))
            {
                // Is this Element one among many child Elements with the same name?
                List<XElement> ChildElementList = 
                    Element.Elements().Where<XElement>(x => x.Name.ToString() == ElementName).ToList<XElement>();

                if (ChildElementList.Count > 1)
                {
                    // There are child Elements with the same name.  So, we'll return a List for the Elements.
                    List<XmlDataObject> ChildElementToObjectList = new List<XmlDataObject>();

                    foreach (XElement ChildElement in ChildElementList)
                    {
                        // Recursively call EmitDataObject on Child Elements having same name
                        ChildElementToObjectList.Add(EmitDataObject(ChildElement));
                    }

                    DataObject.TrySetMember(ElementName, ChildElementToObjectList);
                }
                else
                {
                    // Unique Element name.  Therefore, recursively call EmitDataObject on just this Element.
                    DataObject.TrySetMember(ElementName, 
                        EmitDataObject(Element.Elements().Single<XElement>(x => x.Name == ElementName)));
                }
            }

            return DataObject;
        }

        /// <summary>
        /// Returns a HashSet with a non-repeating list of all the child Element Names
        /// </summary>
        public static HashSet<string> UniqueElementNames(XElement Element)
        {
            HashSet<string> ListOfNames = new HashSet<string>();
            foreach (XElement ChildElement in Element.Elements())
                ListOfNames.Add(ChildElement.Name.ToString());

            return ListOfNames;
        }


        /// <summary>
        /// Returns a copy of the original xmlDocument with all the namespaces stripped-off
        /// </summary>
        public static XDocument RemoveAllNamespaces(XDocument xdocument)
        {
            return new XDocument(RemoveAllNamespaces(xdocument.Root));
        }

        private static XElement RemoveAllNamespaces(XElement element)
        {
            XElement outputelement = new XElement(element.Name.LocalName);
            outputelement.Value = element.Value;

            foreach (XAttribute attribute in element.Attributes())
                outputelement.SetAttributeValue(attribute.Name.LocalName, attribute.Value);

            foreach (XElement childelement in element.Elements())
                outputelement.Add(RemoveAllNamespaces(childelement));

            return outputelement;
        }
    }
}
