// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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

//	Original XmlUnit license
/*
******************************************************************
Copyright (c) 2001, Jeff Martin, Tim Bacon
All rights reserved.

Redistribution and use in source and binary forms, with or without
modification, are permitted provided that the following conditions
are met:

    * Redistributions of source code must retain the above copyright
      notice, this list of conditions and the following disclaimer.
    * Redistributions in binary form must reproduce the above
      copyright notice, this list of conditions and the following
      disclaimer in the documentation and/or other materials provided
      with the distribution.
    * Neither the name of the xmlunit.sourceforge.net nor the names
      of its contributors may be used to endorse or promote products
      derived from this software without specific prior written
      permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
"AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS
FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE
COPYRIGHT OWNER OR CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT,
INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING,
BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER
CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT
LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
POSSIBILITY OF SUCH DAMAGE.

******************************************************************
*/

#pragma warning disable 1591
#pragma warning disable 3001
#pragma warning disable 618


namespace MbUnit.Framework.Xml 
{
    using System.IO;
    using System.Security.Policy;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;
    
    public class Xslt 
	{
        private readonly XmlInput _xsltInput;
    	private readonly XmlResolver _xsltResolver;
    	private readonly Evidence _evidence;
    	
        public Xslt(XmlInput xsltInput, XmlResolver xsltResolver, Evidence evidence) {
            _xsltInput = xsltInput;
        	_xsltResolver = xsltResolver;
        	_evidence = evidence;
        }
        
        public Xslt(XmlInput xsltInput) 
        	: this(xsltInput, null, null) {
        }
        
        public Xslt(string xslt, string baseURI) 
            : this(new XmlInput(xslt, baseURI)) {
        }
        
        public Xslt(string xslt) 
            : this(new XmlInput(xslt)) {
        }
        
        public XmlOutput Transform(string someXml) {
        	return Transform(new XmlInput(someXml)); 
        }
        
        public XmlOutput Transform(XmlInput someXml) {
        	return Transform(someXml, null);
        }
        
        public XmlOutput Transform(XmlInput someXml, XsltArgumentList xsltArgs) {
        	return Transform(someXml.CreateXmlReader(), null, xsltArgs);
        }
        
        public XmlOutput Transform(XmlReader xmlTransformed, XmlResolver resolverForXmlTransformed, XsltArgumentList xsltArgs) {
            XslTransform transform = new XslTransform();
	        XmlReader xsltReader = _xsltInput.CreateXmlReader();
	            
            transform.Load(xsltReader, _xsltResolver);
            
            XmlSpace space = XmlSpace.Default;
            XPathDocument document = new XPathDocument(xmlTransformed, space);
            XPathNavigator navigator = document.CreateNavigator();
            
            return new XmlOutput(transform, xsltArgs, navigator, resolverForXmlTransformed, 
                                 new XmlReader[] {xmlTransformed, xsltReader});
        }
    }
}
