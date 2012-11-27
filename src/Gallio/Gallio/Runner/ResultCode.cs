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

namespace Gallio.Runner
{
    /// <summary>
    /// Describes the result codes used by the application.
    /// </summary>
    public static class ResultCode
    {
        /// <summary>
        /// The tests ran successfully or there were no tests to run.
        /// </summary>
        /// <value>0</value>
        public const int Success = 0;

        /// <summary>
        /// Some tests failed.
        /// </summary>
        /// <value>1</value>
        public const int Failure = 1;

        /// <summary>
        /// The tests were canceled.
        /// </summary>
        /// <value>2</value>
        public const int Canceled = 2;

        /// <summary>
        /// A fatal runtime exception occurred.
        /// </summary>
        /// <value>3</value>
        public const int FatalException = 3;

        /// <summary>
        /// Invalid arguments were supplied on the command-line.
        /// </summary>
        /// <value>10</value>
        public const int InvalidArguments = 10;

        /// <summary>
        /// No tests were found.
        /// </summary>
        /// <value>16</value>
        public const int NoTests = 16;
    }
}
