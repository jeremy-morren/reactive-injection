ARG RUN_IMAGE
ARG BUILD_IMAGE

FROM mcr.microsoft.com/dotnet/${RUN_IMAGE} as run-base
WORKDIR /App

FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-base
WORKDIR /Source

FROM build-base as nuget-build
COPY ["ReactiveInjection.Abstractions/ReactiveInjection.Abstractions.csproj", "ReactiveInjection.Abstractions/"]
COPY ["ReactiveInjection/ReactiveInjection.csproj", "ReactiveInjection/"]

RUN dotnet restore "ReactiveInjection.Abstractions/ReactiveInjection.Abstractions.csproj"
RUN dotnet restore "ReactiveInjection/ReactiveInjection.csproj"

COPY . .
ARG PACKAGE_VERSION="0.1.0"
RUN dotnet pack "ReactiveInjection.Abstractions/ReactiveInjection.Abstractions.csproj" \
    -c Release -o /app/pack "-p:Version=$PACKAGE_VERSION" 

RUN dotnet pack "ReactiveInjection/ReactiveInjection.csproj" \
   -c Release -o /app/pack "-p:Version=$PACKAGE_VERSION"

FROM build-base as build

RUN echo '<?xml version="1.0" encoding="utf-8"?><configuration><packageSources><add key="Local" value="/packages" /> </packageSources></configuration>' > 'nuget.config'
COPY --from=nuget-build /app/pack /packages

COPY "ReactiveInjection.Tests.Generator/ReactiveInjection.Tests.Generator.csproj" .

RUN dotnet restore "ReactiveInjection.Tests.Generator.csproj"

COPY "ReactiveInjection.Tests.Generator/" "./"
ARG FRAMEWORK
RUN dotnet build "ReactiveInjection.Tests.Generator.csproj" -c Release -o /app/build -f "${FRAMEWORK}"

FROM run-base as final
COPY --from=build /app/build .
ENTRYPOINT [ "dotnet", "test", "ReactiveInjection.Tests.Generator.dll" ]
