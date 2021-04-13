#See https://aka.ms/containerfastmode to understand how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/core/runtime:3.1-buster-slim AS base
WORKDIR /app

FROM mcr.microsoft.com/dotnet/core/sdk:3.1-buster AS build
WORKDIR /src
COPY ["opc-twin-dm-client/opc-twin-dm-client.csproj", "opc-twin-dm-client/"]
COPY ["Microsoft.Azure.IIoT.OpcUa.Api.Twin/src/Microsoft.Azure.IIoT.OpcUa.Api.Twin.csproj", "Microsoft.Azure.IIoT.OpcUa.Api.Twin/src/"]
COPY ["Microsoft.Azure.IIoT.OpcUa.Api/src/Microsoft.Azure.IIoT.OpcUa.Api.csproj", "Microsoft.Azure.IIoT.OpcUa.Api/src/"]
COPY ["common/Microsoft.Azure.IIoT.Serializers.NewtonSoft/src/Microsoft.Azure.IIoT.Serializers.NewtonSoft.csproj", "common/Microsoft.Azure.IIoT.Serializers.NewtonSoft/src/"]
COPY ["common/Microsoft.Azure.IIoT.Core/src/Microsoft.Azure.IIoT.Core.csproj", "common/Microsoft.Azure.IIoT.Core/src/"]
COPY ["common/Microsoft.Azure.IIoT.Abstractions/src/Microsoft.Azure.IIoT.Abstractions.csproj", "common/Microsoft.Azure.IIoT.Abstractions/src/"]
RUN dotnet restore "opc-twin-dm-client/opc-twin-dm-client.csproj"
COPY . .
WORKDIR "/src/opc-twin-dm-client"
RUN dotnet build "opc-twin-dm-client.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "opc-twin-dm-client.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENV PATH="/app:${PATH}"
ENTRYPOINT ["dotnet", "opc-twin-dm-client.dll"]