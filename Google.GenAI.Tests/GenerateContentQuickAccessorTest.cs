/*
 * Copyright 2025 Google LLC
 *
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      https://www.apache.org/licenses/LICENSE-2.0
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Google.GenAI.Types;

namespace Google.GenAI.Tests
{
    [TestClass]
    public class GenerateContentResponseQuickAccessorTest
    {
        private static GenerateContentResponse ResponseWithParts(List<Part> parts)
        {
            return new GenerateContentResponse
            {
                Candidates = new List<Candidate>
                {
                    new Candidate
                    {
                        Content = new Content { Parts = parts }
                    }
                }
            };
        }


        [TestMethod]
        public void Text_SingleTextPart_ReturnsText()
        {
            var response = ResponseWithParts(new List<Part> { new Part { Text = "hello" }});
            Assert.AreEqual("hello", response.Text);
        }

        [TestMethod]
        public void Text_MultipleTextParts_ReturnsConcatenated()
        {
            var response = ResponseWithParts(new List<Part>
            {
                new Part { Text = "hello" },
                new Part { Text = " world" }
            });
            Assert.AreEqual("hello world", response.Text);
        }

        [TestMethod]
        public void Text_MixedParts_ReturnsOnlyText()
        {
            var response = ResponseWithParts(new List<Part>
            {
                new Part { Text = "answer" },
                new Part { FunctionCall = new FunctionCall { Name = "myFunc" } }
            });
            Assert.AreEqual("answer", response.Text);
        }

        [TestMethod]
        public void Text_NoTextParts_ReturnsNull()
        {
            var response = ResponseWithParts(new List<Part>
            {
                new Part { FunctionCall = new FunctionCall { Name = "myFunc" } }
            });
            Assert.IsNull(response.Text);
        }

        [TestMethod]
        public void Text_NullCandidates_ReturnsNull()
        {
            var response = new GenerateContentResponse { Candidates = null };
            Assert.IsNull(response.Text);
        }

        [TestMethod]
        public void Text_EmptyParts_ReturnsNull()
        {
            var response = ResponseWithParts(new List<Part>());
            Assert.IsNull(response.Text);
        }


        [TestMethod]
        public void FunctionCalls_WithFunctionCallParts_ReturnsList()
        {
            var fc1 = new FunctionCall { Name = "func1" };
            var fc2 = new FunctionCall { Name = "func2" };
            var response = ResponseWithParts(new List<Part>
            {
                new Part { FunctionCall = fc1 },
                new Part { Text = "some text" },
                new Part { FunctionCall = fc2 }
            });
            var result = response.FunctionCalls;
            Assert.IsNotNull(result);
            Assert.AreEqual(2, result.Count);
            Assert.AreEqual("func1", result[0].Name);
            Assert.AreEqual("func2", result[1].Name);
        }

        [TestMethod]
        public void FunctionCalls_NoFunctionCallParts_ReturnsNull()
        {
            var response = ResponseWithParts(new List<Part> { new Part { Text = "hello" } });
            Assert.IsNull(response.FunctionCalls);
        }

        [TestMethod]
        public void FunctionCalls_NullCandidates_ReturnsNull()
        {
            var response = new GenerateContentResponse { Candidates = null };
            Assert.IsNull(response.FunctionCalls);
        }


        [TestMethod]
        public void ExecutableCode_WithExecutableCodePart_ReturnsCode()
        {
            var response = ResponseWithParts(new List<Part>
            {
                new Part { ExecutableCode = new ExecutableCode { Code = "print('hi')" } }
            });
            Assert.AreEqual("print('hi')", response.ExecutableCode);
        }

        [TestMethod]
        public void ExecutableCode_NoExecutableCodePart_ReturnsNull()
        {
            var response = ResponseWithParts(new List<Part> { new Part { Text = "hello" } });
            Assert.IsNull(response.ExecutableCode);
        }

        [TestMethod]
        public void ExecutableCode_NullCandidates_ReturnsNull()
        {
            var response = new GenerateContentResponse { Candidates = null };
            Assert.IsNull(response.ExecutableCode);
        }


        [TestMethod]
        public void CodeExecutionResult_WithResultPart_ReturnsOutput()
        {
            var response = ResponseWithParts(new List<Part>
            {
                new Part { CodeExecutionResult = new CodeExecutionResult { Output = "42\n" } }
            });
            Assert.AreEqual("42\n", response.CodeExecutionResult);
        }

        [TestMethod]
        public void CodeExecutionResult_NoResultPart_ReturnsNull()
        {
            var response = ResponseWithParts(new List<Part> { new Part { Text = "hello" } });
            Assert.IsNull(response.CodeExecutionResult);
        }

        [TestMethod]
        public void CodeExecutionResult_NullCandidates_ReturnsNull()
        {
            var response = new GenerateContentResponse { Candidates = null };
            Assert.IsNull(response.CodeExecutionResult);
        }


        [TestMethod]
        public void Parts_ReturnsParts()
        {
            var parts = new List<Part> { new Part { Text = "a" }, new Part { Text = "b" } };
            var response = ResponseWithParts(parts);
            Assert.IsNotNull(response.Parts);
            Assert.AreEqual(2, response.Parts.Count);
        }

        [TestMethod]
        public void Parts_NullCandidates_ReturnsNull()
        {
            var response = new GenerateContentResponse { Candidates = null };
            Assert.IsNull(response.Parts);
        }
    }
}
