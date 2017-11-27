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