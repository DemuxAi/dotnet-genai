This directory contains the end to end tests for the Google Gen AI SDK for
dotnet. It uses the TestServerSdk as test dependency. It has two test modes:
`record` and `replay`

## Folder structure
1. `Recordings` folder contains all the request and response payloads in json
2. `test-server.yml` is the config file used by the test server
3. `test.runsettings` is the config file used by MSTest framework for structured
test logs
4. `packages.lock.json` is the lock file for central package management
5. `Google.GenAI.E2E.Tests.csproj` configures the entire test project
6. Other folders should be named like a test feature, e.g. `GenerateContent`. It
should contain the test suite code for the end to end tests

## Test preparation
1. `export GOOGLE_CLOUD_PROJECT=YOUR_CLOUD_PROJECT`
2. `export GOOGLE_CLOUD_LOCATION=YOUR_CLOUD_LOCATION`
3. `export GOOGLE_API_KEY=YOUR_API_KEY_FOR_GEMINI_API`
4. `gcloud auth application-default login`

## Writing Tests
Currently, there is no parameterization for Gemini and Vertex tests, so they
must be written separately. Test names should be formatted as the following:
`<MethodNameInSDK><FeatureUnderTest><Vertex/Gemini>Test`

Ex. `GenerateContentGenerationConfigGeminiTest`

You may configure a test method to be skipped in replay mode with the following
line at the top of the test:
```
[TestMethod]
public async Task testMethod()
{
    if (TestServer.IsReplayMode)
    {
        Assert.Inconclusive("skip reason");
    }
    ...
}
```

## How to run test in record mode
Record mode allows you to record request and response payload for new tests or
update request response payload for existing tests.

1. `export TEST_MODE=record`
2. `dotnet test --settings test.runsettings` for tests against `net8.0` target
3. `dotnet test --settings test.runsettings ./Netstandard2_0/Google.GenAI.Netstandard2_0Tests.csproj` for tests against `netstandard2.0` target

### Test Recording Redaction
If your JSON test recordings contain the numerical GCP project ID, this ID must
be specified in the `TestServerSecrets` at `TestServer.cs` prior to recording the
replay files.

```
var _project_id = "987654321";
TestServerSecrets = $"{_project},{_apiKey},{_project_id}"
```

Once the tests have been recorded, you must remove the numerical
project ID from the `TestServerSecrets`.

## How to run test in replay mode
Replay mode assumes all the test cases have run through the record mode, so that
there is a json file in the `Recordings` folder for the request and response.

1. `export TEST_MODE=replay`
2. `dotnet test --settings test.runsettings`

## How to run test for a specific class only
1. `dotnet test --settings test.runsettings --filter "ClassName=TestClassName"`

## How to run test for a specific test method only
1. `dotnet test --settings test.runsettings --filter "ClassName=TestClassName&Name=TestMethodName"`
