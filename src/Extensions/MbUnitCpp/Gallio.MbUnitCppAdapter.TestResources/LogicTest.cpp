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

#include "stdafx.h"
#include "mbunit.h"

#pragma warning (disable: 4800)

TESTFIXTURE(Logic)
{
    TEST(Assert_IsTrue_should_pass)
    {
		bool value = true;
        Assert.IsTrue(value);
    }

    TEST(Assert_IsTrue_should_fail)
    {
		bool value = false;
        Assert.IsTrue(value);
    }

    TEST(Assert_IsTrue_as_int_should_pass)
    {
		int value = 1;
        Assert.IsTrue(value);
    }

    TEST(Assert_IsTrue_as_int_should_fail)
    {
		int value = 0;
        Assert.IsTrue(value);
    }

    TEST(Assert_IsTrue_constant_should_pass)
    {
        Assert.IsTrue(true);
    }

    TEST(Assert_IsTrue_constant_should_fail)
    {
        Assert.IsTrue(false);
    }

    TEST(Assert_IsTrue_as_constant_int_should_pass)
    {
        Assert.IsTrue(1);
    }

    TEST(Assert_IsTrue_as_constant_int_should_fail)
    {
        Assert.IsTrue(0);
    }

    TEST(Assert_IsTrue_should_fail_with_custom_message)
    {
		bool value = false;
        Assert.IsTrue(value, L"This is a custom message.");
    }

    TEST(Assert_IsFalse_should_pass)
    {
		bool value = false;
        Assert.IsFalse(value);
    }

    TEST(Assert_IsFalse_should_fail)
    {
		bool value = true;
        Assert.IsFalse(value);
    }

    TEST(Assert_IsFalse_as_int_should_pass)
    {
		int value = 0;
        Assert.IsFalse(value);
    }

    TEST(Assert_IsFalse_as_int_should_fail)
    {
		int value = 1;
        Assert.IsFalse(value);
    }

    TEST(Assert_IsFalse_constant_should_pass)
    {
        Assert.IsFalse(false);
    }

    TEST(Assert_IsFalse_constant_should_fail)
    {
        Assert.IsFalse(true);
    }

    TEST(Assert_IsFalse_as_constant_int_should_pass)
    {
        Assert.IsFalse(0);
    }

    TEST(Assert_IsFalse_as_constant_int_should_fail)
    {
        Assert.IsFalse(1);
    }

    TEST(Assert_IsFalse_should_fail_with_custom_message)
    {
		bool value = true;
        Assert.IsFalse(value, L"This is a custom message.");
    }
}

