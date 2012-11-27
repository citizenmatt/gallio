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

using System;
using System.Text.RegularExpressions;
using Gallio.Framework.Assertions;
using MbUnit.Framework;

namespace MbUnit.Tests.Framework
{
    [TestsOn(typeof(Assert))]
    public class AssertTest_Strings : BaseAssertTest
    {
        #region Contains
        [Test]
        public void Contains_simple_test()
        {
            Assert.Contains("Abcdef", "cde");
        }

        [Test]
        public void Contains_fails_when_simple_substring_is_not_found()
        {
            AssertionFailure[] failures = Capture(() => Assert.Contains("ABCDEF","xyz"));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected string to contain a particular substring.", failures[0].Description);
            Assert.AreEqual("Expected Substring", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("\"xyz\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("\"ABCDEF\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test, ExpectedArgumentNullException("Value cannot be null.")]
        public void Contains_fails_when_expected_substring_is_null()
        {
            Assert.Contains("Abcdef", null);
        }

        [Test]
        public void Contains_fail_test_with_custom_message()
        {
            AssertionFailure[] failures = Capture(() => Assert.Contains("ABCDEF", "xyz","{0} message", "custom"));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected string to contain a particular substring.", failures[0].Description);
            Assert.AreEqual("Expected Substring", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("\"xyz\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("\"ABCDEF\"", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("custom message", failures[0].Message);
        }

        [Test]
        public void Contains_with_comparison_type_simple_test()
        {
            Assert.Contains("Abcdef", "CDe", StringComparison.OrdinalIgnoreCase);
        }
        #endregion
        
        #region DoesNotContain 
        
        [Test]
        public void DoesNotContain_simple_test()
        {
            Assert.DoesNotContain("Abcdef", "xyz");
        }

        [Test]
        public void DoesNotContain_should_not_find_substring_with_different_casing()
        {
            Assert.DoesNotContain("Abcdef", "ABC");
        }

        [Test]
        public void DoesNotContain_fails_when_simple_substring_is_found()
        {
            AssertionFailure[] failures = Capture(() => Assert.DoesNotContain("ABCDEF", "ABCDE"));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected string to not contain a particular substring.", failures[0].Description);
            Assert.AreEqual("Unexpected Substring", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("\"ABCDE\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("\"ABCDEF\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test, ExpectedArgumentNullException("Value cannot be null.")]
        public void DoesNotContain_fails_when_expected_substring_is_null()
        {
            Assert.DoesNotContain("Abcdef", null);
        }
        
        [Test]
        public void DoesNotContain_fail_test_with_custom_message()
        {
            AssertionFailure[] failures = Capture(() => Assert.DoesNotContain("ABCDEF", "BCD", "{0} message", "custom"));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected string to not contain a particular substring.", failures[0].Description);
            Assert.AreEqual("Unexpected Substring", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("\"BCD\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("\"ABCDEF\"", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("custom message", failures[0].Message);
        }

        [Test]
        public void DoesNotContain_with_comparison_type_simple_test()
        {
            Assert.DoesNotContain("Abcdef", "xyz", StringComparison.OrdinalIgnoreCase);
        }

        [Test]
        public void DoesNotContain_with_comparison_type_fail_test_with_custom_message()
        {
            AssertionFailure[] failures = Capture(() => Assert.DoesNotContain("ABCDEF", "bcD", StringComparison.OrdinalIgnoreCase, "{0} message", "custom"));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected string to not contain a particular substring.", failures[0].Description);
            Assert.AreEqual("Comparison Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("{OrdinalIgnoreCase}", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Unexpected Substring", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"bcD\"", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("\"ABCDEF\"", failures[0].LabeledValues[2].FormattedValue.ToString());
            Assert.AreEqual("custom message", failures[0].Message);
        }

        #endregion

        #region AreEqual
        [Test]
        public void AreEqualIgnoreCase_Test_with_non_null_values()
        {
            Assert.AreEqual("test", "TeSt", StringComparison.InvariantCultureIgnoreCase);
        }

        [Test]
        public void AreEqualIgnoreCase_fails_when_simple_values_different()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.AreEqual("test", "tEsm", StringComparison.InvariantCultureIgnoreCase));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected values to be equal according to string comparison type.", failures[0].Description);
            Assert.AreEqual("Comparison Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("{InvariantCultureIgnoreCase}", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Expected Value", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"test\"", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[2].Label);
            Assert.AreEqual("\"tEsm\"", failures[0].LabeledValues[2].FormattedValue.ToString());
        }

        [Test]
        public void AreEqualIgnoreCase_fails_when_one_of_the_values_is_null()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.AreEqual("test", null, StringComparison.InvariantCultureIgnoreCase));
            Assert.Count(1, failures);
            Assert.AreEqual("{InvariantCultureIgnoreCase}", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("\"test\"", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("null", failures[0].LabeledValues[2].FormattedValue.ToString());
        }


        [Test]
        public void AreEqualIgnoreCase_fails_with_custom_message()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.AreEqual(null, "test", "{0} message {1}", "MB1", "Mb2", StringComparison.InvariantCultureIgnoreCase));
            Assert.Count(1, failures);
            Assert.AreEqual("null", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("\"test\"", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("MB1 message Mb2", failures[0].Message);
        }
        #endregion

        #region FullMatch

        [Test]
        public void FullMatch_sucessful_tests_with_Regex()
        {
            Assert.FullMatch("mbTest", new Regex(@"[\w]{6}"));
        }

        [Test]
        public void FullMatch_sucessful_with_pattern()
        {
            Assert.FullMatch("mbTest", @"[\w]{6}");
        }

        [Test, ExpectedArgumentNullException]
        public void FullMatch_test_for_ArgumentNullException_when_regex_is_null()
        {
            const Regex re = null;
            Assert.FullMatch("mbTest", re);
        }

        [Test, ExpectedArgumentNullException]
        public void FullMatch_test_for_ArgumentNullException_when_pattern_is_null()
        {
            const string pattern = null;
            Assert.FullMatch("mbTest", pattern);
        }

        [Test]
        public void FullMatch_fails_when_testValue_is_null()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.FullMatch(null, new Regex(@"[\d]{6}")));
            Assert.Count(1, failures);
        }

        [Test]
        public void FullMatch_fails_when_testValue_does_not_match_regex_pattern()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.FullMatch("mbTest", new Regex(@"[\d]{6}")));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected a string to exactly match a regular expression pattern.", failures[0].Description);
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("\"mbTest\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Regex Pattern", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"[\\\\d]{6}\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void FullMatch_fails_when_testValue_matches_regex_pattern_but_lenght_is_different()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.FullMatch("mbTest", new Regex(@"[\w]{7}")));
            Assert.Count(1, failures);
            Assert.AreEqual("\"mbTest\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("\"[\\\\w]{7}\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void FullMatch_fail_test_with_custom_message()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.FullMatch("mbTest", new Regex(@"[\w]{7}"), "{0} message {1}", "MB1", "Mb2"));
            Assert.Count(1, failures);
            Assert.AreEqual("MB1 message Mb2", failures[0].Message);
        }

        #endregion

        #region Like

        [Test]
        public void Like_sucessful_tests_with_Regex()
        {
            Assert.Like("mbTest", new Regex(@"[\w]+"));
        }

        [Test]
        public void Like_sucessful_with_pattern()
        {
            Assert.Like("mbTest", @"[\w]*");
        }

        [Test]
        public void Like_fails_when_testValue_is_null()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.Like(null, new Regex(@"[\d]+")));
            Assert.Count(1, failures);
        }

        [Test, ExpectedArgumentNullException]
        public void Like_test_for_ArgumentNullException_when_regex_is_null()
        {
            const Regex re = null;
            Assert.Like("mbTest", re);
        }

        [Test, ExpectedArgumentNullException]
        public void Like_test_for_ArgumentNullException_when_pattern_is_null()
        {
            const string pattern = null;
            Assert.Like("mbTest", pattern);
        }

        [Test]
        public void Like_fails_when_testValue_does_not_match_regex_pattern()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.Like("mbTest", new Regex(@"[\d]+")));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected a string to contain a full or partial match of a regular expression pattern.", failures[0].Description);
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("\"mbTest\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Regex Pattern", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"[\\\\d]+\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void Like_fail_test_with_custom_message()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.Like("mbTest", new Regex(@"[\w]{7}"), "{0} message {1}", "MB1", "Mb2"));
            Assert.Count(1, failures);
            Assert.AreEqual("MB1 message Mb2", failures[0].Message);
        }

        #endregion

        #region NotLike

        [Test]
        public void NotLike_sucessful_tests_with_Regex()
        {
            Assert.NotLike("mbTest", new Regex(@"[\d]+"));
        }

        [Test]
        public void NotLike_sucessful_with_pattern()
        {
            Assert.NotLike("mbTest", @"[\d]{2}");
        }

        [Test]
        public void NotLike_fails_when_testValue_is_null()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.NotLike(null, new Regex(@"[\w]+")));
            Assert.Count(1, failures);
        }

        [Test, ExpectedArgumentNullException]
        public void NotLike_test_for_ArgumentNullException_when_regex_is_null()
        {
            const Regex re = null;
            Assert.NotLike("mbTest", re);
        }

        [Test, ExpectedArgumentNullException]
        public void NotLike_test_for_ArgumentNullException_when_pattern_is_null()
        {
            const string pattern = null;
            Assert.NotLike("mbTest", pattern);
        }

        [Test]
        public void NotLike_fails_when_testValue_does_not_match_regex_pattern()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.NotLike("mbTest", new Regex(@"[\w]+")));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected a string to not contain a full or partial match of a regular expression pattern.", failures[0].Description);
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("\"mbTest\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Regex Pattern", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"[\\\\w]+\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void NotLike_fail_test_with_custom_message()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.NotLike("mbTest", new Regex(@"[\w]{6}"), "{0} message {1}", "MB1", "Mb2"));
            Assert.Count(1, failures);
            Assert.AreEqual("MB1 message Mb2", failures[0].Message);
        }

        #endregion

        #region StartsWith

        [Test]
        public void StartsWith_sucessful_tests()
        {
            Assert.StartsWith("mbTest", "mbT");
        }

        [Test]
        public void StartsWith_fails_when_testValue_is_null()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.StartsWith(null, ""));
            Assert.Count(1, failures);
        }

        [Test, ExpectedArgumentNullException]
        public void StartsWith_test_for_ArgumentNullException_when_pattern_is_null()
        {
            Assert.StartsWith("mbTest", null);
        }

        [Test]
        public void StartsWith_fails_when_testValue_does_not_start_with_pattern()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.StartsWith("mbTest", "jb"));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected string to start with the specified text.", failures[0].Description);
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("\"mbTest\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Expected Text", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"jb\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void StartsWith_fail_test_with_custom_message()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.StartsWith("mbTest", "jb", "{0} message {1}", "MB1", "Mb2"));
            Assert.Count(1, failures);
            Assert.AreEqual("MB1 message Mb2", failures[0].Message);
        }

        [Test]
        public void StartsWith_with_comparison_type_sucessful_tests()
        {
            Assert.StartsWith("mbTest", "MBT", StringComparison.OrdinalIgnoreCase);
        }

        [Test]
        public void StartsWith_with_comparison_type_fails_when_testValue_does_not_start_with_pattern()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.StartsWith("mbTest", "jb", StringComparison.OrdinalIgnoreCase));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected string to start with the specified text.", failures[0].Description);
            Assert.AreEqual("Comparison Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("{OrdinalIgnoreCase}", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"mbTest\"", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("Expected Text", failures[0].LabeledValues[2].Label);
            Assert.AreEqual("\"jb\"", failures[0].LabeledValues[2].FormattedValue.ToString());
        }
        #endregion

        #region EndsWith

        [Test]
        public void EndsWith_sucessful_tests()
        {
            Assert.EndsWith("mbTest", "est");
        }

        [Test]
        public void EndsWith_fails_when_testValue_is_null()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.EndsWith(null, ""));
            Assert.Count(1, failures);
        }

        [Test, ExpectedArgumentNullException]
        public void EndsWith_test_for_ArgumentNullException_when_pattern_is_null()
        {
            Assert.EndsWith("mbTest", null);
        }

        [Test]
        public void EndsWith_fails_when_testValue_does_not_start_with_pattern()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.EndsWith("mbTest", "jb"));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected string to end with the specified text.", failures[0].Description);
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("\"mbTest\"", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Expected Text", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"jb\"", failures[0].LabeledValues[1].FormattedValue.ToString());
        }

        [Test]
        public void EndsWith_fail_test_with_custom_message()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.EndsWith("mbTest", "jb", "{0} message {1}", "MB1", "Mb2"));
            Assert.Count(1, failures);
            Assert.AreEqual("MB1 message Mb2", failures[0].Message);
        }

        [Test]
        public void EndsWith_with_comparison_type_sucessful_tests()
        {
            Assert.EndsWith("mbTest", "EsT", StringComparison.OrdinalIgnoreCase);
        }

        [Test]
        public void EndsWith_with_comparison_type_fails_when_testValue_is_null()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.EndsWith(null, "", StringComparison.OrdinalIgnoreCase));
            Assert.Count(1, failures);
        }

        [Test, ExpectedArgumentNullException]
        public void EndsWith_with_comparison_type_test_for_ArgumentNullException_when_pattern_is_null()
        {
            Assert.EndsWith("mbTest", null, StringComparison.OrdinalIgnoreCase);
        }

        [Test]
        public void EndsWith_with_comparison_type_fails_when_testValue_does_not_start_with_pattern()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.EndsWith("mbTest", "jb", StringComparison.OrdinalIgnoreCase));
            Assert.Count(1, failures);
            Assert.AreEqual("Expected string to end with the specified text.", failures[0].Description);
            Assert.AreEqual("Comparison Type", failures[0].LabeledValues[0].Label);
            Assert.AreEqual("{OrdinalIgnoreCase}", failures[0].LabeledValues[0].FormattedValue.ToString());
            Assert.AreEqual("Actual Value", failures[0].LabeledValues[1].Label);
            Assert.AreEqual("\"mbTest\"", failures[0].LabeledValues[1].FormattedValue.ToString());
            Assert.AreEqual("Expected Text", failures[0].LabeledValues[2].Label);
            Assert.AreEqual("\"jb\"", failures[0].LabeledValues[2].FormattedValue.ToString());
        }

        [Test]
        public void EndsWith_with_comparison_type_fail_test_with_custom_message()
        {
            AssertionFailure[] failures = AssertTest.Capture(() => Assert.EndsWith("mbTest", "jb", StringComparison.OrdinalIgnoreCase, "{0} message {1}", "MB1", "Mb2"));
            Assert.Count(1, failures);
            Assert.AreEqual("MB1 message Mb2", failures[0].Message);
        }
        #endregion

    }
}