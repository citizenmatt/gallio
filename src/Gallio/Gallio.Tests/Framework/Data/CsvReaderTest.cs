﻿// Copyright 2005-2010 Gallio Project - http://www.gallio.org/
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
using System.IO;
using Gallio.Framework.Data;
using MbUnit.Framework;

namespace Gallio.Tests.Framework.Data
{
    [TestFixture]
    [TestsOn(typeof(CsvReader))]
    public class CsvReaderTest : BaseTestWithMocks
    {
        [Test, ExpectedArgumentNullException]
        public void ConstructorThrowsIfReaderIsNull()
        {
            new CsvReader(null);
        }

        [Test]
        public void DefaultFieldDelimiterIsComma()
        {
            StringReader document = new StringReader("");
            CsvReader reader = new CsvReader(document);

            Assert.AreEqual(',', reader.FieldDelimiter);
        }

        [Test]
        public void DefaultCommentPrefixIsPound()
        {
            StringReader documentReader = new StringReader("");
            CsvReader reader = new CsvReader(documentReader);

            Assert.AreEqual('#', reader.CommentPrefix);
        }

        [Test]
        [Row("", ',', '#',
            new object[] { },
            Description = "Empty document.")]
        [Row("abc,def,ghi", ',', '#',
            new object[] { new string[] { "abc", "def", "ghi" }},
            Description = "Single record document with no terminal newline.")]
        [Row("abc,def,ghi\n", ',', '#',
            new object[] { new string[] { "abc", "def", "ghi" } },
            Description = "Single record document with a terminal newline.")]
        [Row(",,", ',', '#',
            new object[] { new string[] { "", "", "" } },
            Description = "Single record document with empty fields.")]
        [Row("abc|def|ghi\n; Comment\n123|456", '|', ';',
            new object[] { new string[] { "abc", "def", "ghi" }, new string[] { "123", "456" } },
            Description = "Multiple record document with comments and non-standard delimiter.")]
        [Row("   ab c  ,   def,ghi   ", ',', '#',
            new object[] { new string[] { "ab c", "def", "ghi" } },
            Description = "Single record with excess whitespace.")]
        [Row("\"   quoted   \",\"escaped\"\"quote\"", ',', '#',
            new object[] { new string[] { "   quoted   ", "escaped\"quote" } },
            Description = "Quoted fields.")]
        [Row("\"terminal quote\"", ',', '#',
            new object[] { new string[] { "terminal quote" } },
            Description = "Quoted fields at end of record.")]
        public void ReadRecords(string document, char fieldDelimiter, char commentPrefix, object[] expectedRecords)
        {
            StringReader documentReader = new StringReader(document);
            CsvReader reader = new CsvReader(documentReader);

            reader.FieldDelimiter = fieldDelimiter;
            Assert.AreEqual(fieldDelimiter, reader.FieldDelimiter);

            reader.CommentPrefix = commentPrefix;
            Assert.AreEqual(commentPrefix, reader.CommentPrefix);

            List<string[]> actualRecords = new List<string[]>();
            string[] record;
            while ((record = reader.ReadRecord()) != null)
                actualRecords.Add(record);

            Assert.Over.Pairs(expectedRecords, actualRecords, (expectedRecord, actualRecord) => Assert.AreEqual((string[])expectedRecord, actualRecord));
        }

        [Test]
        public void UpdatesTheLineNumber()
        {
            StringReader documentReader = new StringReader("abc\n#123\ndef");
            CsvReader reader = new CsvReader(documentReader);

            Assert.AreEqual(0, reader.PreviousRecordLineNumber);
            Assert.AreEqual(new string[] { "abc" }, reader.ReadRecord());
            Assert.AreEqual(1, reader.PreviousRecordLineNumber);
            Assert.AreEqual(new string[] { "def" }, reader.ReadRecord());
            Assert.AreEqual(3, reader.PreviousRecordLineNumber);
            Assert.IsNull(reader.ReadRecord());
            Assert.AreEqual(3, reader.PreviousRecordLineNumber);
        }

        [Test]
        public void CloseClosesTheTextReader()
        {
            TextReader mockTextReader = Mocks.StrictMock<TextReader>();
            mockTextReader.Close();

            Mocks.ReplayAll();

            CsvReader reader = new CsvReader(mockTextReader);
            reader.Close();
        }

        [Test]
        public void DisposeClosesTheTextReader()
        {
            TextReader mockTextReader = Mocks.StrictMock<TextReader>();
            mockTextReader.Close();

            Mocks.ReplayAll();

            IDisposable reader = new CsvReader(mockTextReader);
            reader.Dispose();
        }
    }
}
