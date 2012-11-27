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

TESTFIXTURE(MetadataInTest)
{
    TEST(WithCategory, 
		CATEGORY("Sample"))
    {
    }

    TEST(WithAuthor, 
		AUTHOR("Charlie Chaplin"))
    {
    }

    TEST(WithDescription, 
		DESCRIPTION("This is a simple test"))
    {
    }

    TEST(WithSeveralAttributes, 
		CATEGORY("Sample"), 
		AUTHOR("Charlie Chaplin"), 
		DESCRIPTION("This is a simple test"))
    {
    }
}

TESTFIXTURE(MetadataInTestFixture,
		CATEGORY("Sample"), 
		AUTHOR("Charlie Chaplin"))
{
    TEST(Sample1)
    {
    }

    TEST(Sample2, DESCRIPTION("This is a simple test"))
    {
    }
}

TESTFIXTURE(DataDrivenTestWithMetadata, CATEGORY("Yipee"))
{
	DATA(Source, int x)
	{
		ROW(123, "123")
		ROW(456, "456")
		ROW(789, "789")
	}

    TEST(Test, BIND(Source, row), DESCRIPTION("blah blah"))
    {
    }
}