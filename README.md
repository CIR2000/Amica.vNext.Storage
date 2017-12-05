# Storage.Remote
Storage client for the Amica.vNext storage platform.

## Installation
First add the C2K-WEB1 nuget feed to your nuget sources (the api key is provided offline, just ask for it):
```
nuget setapikey [apikey] -source http://nuget.gestionaleamica.com/nuget
nuget sources add -name "C2K-WEB1" -source http://nuget.gestionaleamica.com/nuget
```

Once your nuget source is configured all you have to do is:
```
nuget install Amica.Storage.Remote
```
Alternatively, you can use Visual Studio. Go to Tools/Options/NuGet Package Manager/Package Sources, then 
add a new source.

## Usage
The `Amica.Models` namespace provides a number of DTO classes. Instances of these can be stored, updated, 
deleted and retrieved from the remote service via the `Amica.Storage` SDK.

```c#	
using Amica.Models;
using Amica.Storage;

var transport = new Fee 
{
	Name = "Spese di trasporto",
	CompanyId = "<companyid>",
	Amount = 10.20m,
	Vat = new Vat { 
		Name = "IVA 10%", 
		Code = "10",
		Rate = 10.0f
	},
};

var remote = new RemoteBulkRepository() 
{
	BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVICE_URI")),
	ApiKey = Environment.GetEnvironmentVariable("SERVICE_API_KEY"),
}

// Store object and get it back updated with service meta-data such as the UniqueId.
transport = await remote.Insert(transport);
assert.IsNotNull(transport.UniqueId);

transport.Amount = 15m;
var previousUpdate = transport.Updated;

// Update the object by storing it back on the remote service.
transport = await remote.Replace(transport);
Assert.AreNotEqual(previousUpdate, transport.Updated)

// Get an object from server, or update it with a fresh version if there is one available
previousUpdate = transport.Updated;
transport = await remote.Get(transport);
if (transport.Updated != previousUpdate) Console.Writeline("It has changed!");

// Delete object 
try 
{ 
	await remote.Delete(transport); 
}
catch (RemoteObjectNotFoundStorageException) 
{
	Console.Writeline("Object already deleted or not found.")
}

Since `CompanyId` is required on almost all models and most queries will need to be company-restricted, you can 
leverage the `RemoteCompanyRepository` class which exposes a number of specialized `Get` methods:

```c#
	var remote = new RemoteCompanyRepository()
	{
		BaseAddress = new Uri(Environment.GetEnvironmentVariable("SERVICE_URI")),
		ApiKey = Environment.GetEnvironmentVariable("SERVICE_API_KEY"),
	}

	// Returns a List<Transport> from a specific company.
	var transports = remoteCompany.Get("<companyId>");
```