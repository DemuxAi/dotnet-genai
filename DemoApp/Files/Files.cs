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

using Google.GenAI;
using Google.GenAI.Types;

public class Files {
  public static async Task SendRequestAsync() {
    string apiKey = System.Environment.GetEnvironmentVariable("GEMINI_API_KEY");
    var geminiClient = new Client(apiKey: apiKey);
    try {
      var uploadResponse1 = await geminiClient.Files.UploadAsync(filePath: "./google.png");
      Console.WriteLine("Gemini API Files UploadAsync Response from file path for png:");
      Console.WriteLine(uploadResponse1);

      var uploadResponse2 = await geminiClient.Files.UploadAsync(filePath: "./google.jpg");
      Console.WriteLine("Gemini API Files UploadAsync Response from file path for jpg:");
      Console.WriteLine(uploadResponse2);

      var uploadResponse3 = await geminiClient.Files.UploadAsync(filePath: "./story.txt");
      Console.WriteLine("Gemini API Files UploadAsync Response from file path for txt:");
      Console.WriteLine(uploadResponse3);

      var uploadResponse4 = await geminiClient.Files.UploadAsync(filePath: "./story.pdf");
      Console.WriteLine("Gemini API Files UploadAsync Response from file path for pdf:");
      Console.WriteLine(uploadResponse4);

      var uploadResponse5 = await geminiClient.Files.UploadAsync(filePath: "./pixel.m4a");
      Console.WriteLine("Gemini API Files UploadAsync Response from file path for m4a:");
      Console.WriteLine(uploadResponse5);

      byte[] fileBytes = await System.IO.File.ReadAllBytesAsync("./google.png");
      var uploadResponse6 = await geminiClient.Files.UploadAsync(
          bytes: fileBytes,
          fileName: "google.png"
      );
      Console.WriteLine("Gemini API Files UploadAsync Response from bytes:");
      Console.WriteLine(uploadResponse6);


      var pager = await geminiClient.Files.ListAsync();
      await foreach (var page in pager) {
        Console.WriteLine("Gemini API Files ListAsync Response page:");
        Console.WriteLine(page);
      }

      string fileName = uploadResponse6.Name.ToString();
      Console.WriteLine($"File name: {fileName}");
      var getResponse = await geminiClient.Files.GetAsync(name: fileName);
      Console.WriteLine("Gemini API Files GetAsync Response GetAsync:");
      Console.WriteLine(getResponse);

      var deleteResponse = await geminiClient.Files.DeleteAsync(name: fileName);
      Console.WriteLine("Gemini API Files DeleteAsync Response deleted file:");
      Console.WriteLine(deleteResponse);
    } catch (HttpRequestException ex) {
      Console.WriteLine($"An error occurred with Gemini API: {ex.ToString()}");
    }
  }
}
