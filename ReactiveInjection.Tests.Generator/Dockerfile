ARG TAG

FROM mcr.microsoft.com/dotnet/sdk:7.0 as build

ARG FRAMEWORK

WORKDIR /Source
COPY ["ReactiveInjection.Tests.Generator/ReactiveInjection.Tests.Generator.csproj", "ReactiveInjection.Tests.Generator/"]
COPY ["ReactiveInjection.Abstractions/ReactiveInjection.Abstractions.csproj", "ReactiveInjection.Abstractions/"]
COPY ["ReactiveInjection/ReactiveInjection.csproj", "ReactiveInjection/"]

RUN dotnet restore "ReactiveInjection.Tests.Generator/ReactiveInjection.Tests.Generator.csproj"

COPY . .
RUN dotnet build "ReactiveInjection.Tests.Generator/ReactiveInjection.Tests.Generator.csproj" -o /app/build -f "$FRAMEWORK"

FROM mcr.microsoft.com/dotnet/sdk:$TAG as run
WORKDIR /App
COPY --from=build /app/build .
ENTRYPOINT [ "dotnet", "test", "ReactiveInjection.Tests.Generator.dll" ]
