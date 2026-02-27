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
using System;
using System.Collections.Generic;
using Google.GenAI.Types;

namespace Google.GenAI.Tests
{
    [TestClass]
    public class PartQuickAccessorTest
    {

        [TestMethod]
        public void FromText_SetsTextProperty()
        {
            var part = Part.FromText("hello world");
            Assert.AreEqual("hello world", part.Text);
            Assert.IsNull(part.FileData);
            Assert.IsNull(part.InlineData);
        }


        [TestMethod]
        public void FromUri_ExplicitMimeType_SetsFileData()
        {
            var part = Part.FromUri("gs://bucket/file.jpg", mimeType: "image/jpeg");
            Assert.IsNotNull(part.FileData);
            Assert.AreEqual("gs://bucket/file.jpg", part.FileData.FileUri);
            Assert.AreEqual("image/jpeg", part.FileData.MimeType);
            Assert.IsNull(part.MediaResolution);
        }

        [TestMethod]
        public void FromUri_InferredMimeType_SetsFileData()
        {
            var part = Part.FromUri("https://example.com/photo.jpg");
            Assert.IsNotNull(part.FileData);
            Assert.AreEqual("https://example.com/photo.jpg", part.FileData.FileUri);
            Assert.AreEqual("image/jpeg", part.FileData.MimeType);
        }

        [TestMethod]
        public void FromUri_WithMediaResolution_SetsMediaResolution()
        {
            var resolution = new PartMediaResolution { Level = PartMediaResolutionLevel.MediaResolutionLow };
            var part = Part.FromUri("gs://bucket/file.png", "image/png", resolution);
            Assert.IsNotNull(part.MediaResolution);
            Assert.AreEqual(PartMediaResolutionLevel.MediaResolutionLow, part.MediaResolution.Level);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void FromUri_UnknownExtension_ThrowsArgumentException()
        {
            Part.FromUri("gs://bucket/file.unknownxyz");
        }


        [TestMethod]
        public void FromBytes_SetsInlineData()
        {
            var data = new byte[] { 0x89, 0x50, 0x4E, 0x47 };
            var part = Part.FromBytes(data, "image/png");
            Assert.IsNotNull(part.InlineData);
            CollectionAssert.AreEqual(data, part.InlineData.Data);
            Assert.AreEqual("image/png", part.InlineData.MimeType);
            Assert.IsNull(part.MediaResolution);
        }

        [TestMethod]
        public void FromBytes_WithMediaResolution_SetsMediaResolution()
        {
            var data = new byte[] { 0xFF, 0xD8 };
            var resolution = new PartMediaResolution { Level = PartMediaResolutionLevel.MediaResolutionHigh };
            var part = Part.FromBytes(data, "image/jpeg", resolution);
            Assert.IsNotNull(part.MediaResolution);
            Assert.AreEqual(PartMediaResolutionLevel.MediaResolutionHigh, part.MediaResolution.Level);
        }


        [TestMethod]
        public void FromFunctionCall_SetsNameAndArgs()
        {
            var args = new Dictionary<string, object> { ["city"] = "Paris" };
            var part = Part.FromFunctionCall("getWeather", args);
            Assert.IsNotNull(part.FunctionCall);
            Assert.AreEqual("getWeather", part.FunctionCall.Name);
            Assert.AreEqual("Paris", part.FunctionCall.Args!["city"]);
        }

        [TestMethod]
        public void FromFunctionCall_NullArgs_SetsNullArgs()
        {
            var part = Part.FromFunctionCall("doSomething");
            Assert.IsNotNull(part.FunctionCall);
            Assert.AreEqual("doSomething", part.FunctionCall.Name);
            Assert.IsNull(part.FunctionCall.Args);
        }


        [TestMethod]
        public void FromFunctionResponse_SetsNameAndResponse()
        {
            var response = new Dictionary<string, object> { ["temperature"] = 22 };
            var part = Part.FromFunctionResponse("getWeather", response);
            Assert.IsNotNull(part.FunctionResponse);
            Assert.AreEqual("getWeather", part.FunctionResponse.Name);
            Assert.AreEqual(22, part.FunctionResponse.Response!["temperature"]);
        }

        [TestMethod]
        public void FromFunctionResponse_NullArgs_SetsNullFields()
        {
            var part = Part.FromFunctionResponse("noop");
            Assert.IsNotNull(part.FunctionResponse);
            Assert.AreEqual("noop", part.FunctionResponse.Name);
            Assert.IsNull(part.FunctionResponse.Response);
            Assert.IsNull(part.FunctionResponse.Parts);
        }


        [TestMethod]
        public void FromExecutableCode_SetsCodeAndLanguage()
        {
            var part = Part.FromExecutableCode("print('hi')", Language.Python);
            Assert.IsNotNull(part.ExecutableCode);
            Assert.AreEqual("print('hi')", part.ExecutableCode.Code);
            Assert.AreEqual(Language.Python, part.ExecutableCode.Language);
        }


        [TestMethod]
        public void FromCodeExecutionResult_SetsOutcomeAndOutput()
        {
            var part = Part.FromCodeExecutionResult(Outcome.OutcomeOk, "42\n");
            Assert.IsNotNull(part.CodeExecutionResult);
            Assert.AreEqual(Outcome.OutcomeOk, part.CodeExecutionResult.Outcome);
            Assert.AreEqual("42\n", part.CodeExecutionResult.Output);
        }
    }
}
