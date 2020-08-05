dotnet restore

dotnet build --configuration Debug
dotnet build --configuration Release

dotnet test -c Debug .\tests\TauCode.WebApi.Server.Cqrs.Tests\TauCode.WebApi.Server.Cqrs.Tests.csproj
dotnet test -c Release .\tests\TauCode.WebApi.Server.Cqrs.Tests\TauCode.WebApi.Server.Cqrs.Tests.csproj

nuget pack nuget\TauCode.WebApi.Server.Cqrs.nuspec
