Book Store Sample app

Architecture

* Serverless architecture with Azure Functions in StoreAPI project
* CosmosDB to store data in Azure.
* AzureExtensions.Swashbuckle for Swagger json generation
* Azure Active Directory B2C for user registration and authentication
* Microsoft open-api tools (Service Reference) for StoreClient code generation from Swagger
* Microsoft.Identity.Client (MSAL) to authenticate clients to B2C. Donwside is that is does not work on WebAssembly (WASM).
* UNO platform and ReactiveUI for a client, that hopefully with a few modifications can run on WASM, iOS, Android and UWP.

Known issues:
* It does not work on WebAssembly.
* I only tested it on UWP.
* There are no UNIT tests.
* There is no logging implemented.
* This was the first time I used this stack - but definitely interesting possibilities.

