FROM microsoft/dotnet:2.2-aspnetcore-runtime-stretch-slim as base

# Install Visual Studio Snapshot Debugger prerequisites
RUN apt-get update \
    && apt-get install -y --no-install-recommends \
        curl libxml2 uuid libunwind8

# Install Visual Studio Snapshot Debugger
ARG VSSNAPSHOTDEBUGGER_VERSION=2.0.8
ARG VSSNAPSHOTDEBUGGER_SHA512=18cc72ca2bdb291fb126b11ec705e270ad11e0b053010afb19d0252d3f2911a02af7496adf89eec883b9183771867b234902db6a02926ad1f0f736cba5e71e77
RUN curl -SL --output vssnapshotdebugger.tar.gz "https://aka.ms/vssnapshotdebugger/release/${VSSNAPSHOTDEBUGGER_VERSION}/vssnapshotdebugger-${VSSNAPSHOTDEBUGGER_VERSION}-linux-x64.tar.gz" \
    && echo "${VSSNAPSHOTDEBUGGER_SHA512}  vssnapshotdebugger.tar.gz" | sha512sum -c - \
    && mkdir -p /diag \
    && mkdir -p /tmp/diag \
    && tar -pzxf vssnapshotdebugger.tar.gz -C /diag \
    && rm vssnapshotdebugger.tar.gz

# Set environment variables to load Visual Studio Snapshot Debugger into .NET Core applications
ENV CORECLR_ENABLE_PROFILING=1
ENV CORECLR_PROFILER={324F817A-7420-4E6D-B3C1-143FBED6D855}
ENV CORECLR_PROFILER_PATH_64=/diag/Instrumentation64/libInstrumentationEngine.so
ENV MicrosoftInstrumentationEngine_Host={CA487940-57D2-10BF-11B2-A3AD5A13CBC0}
ENV MicrosoftInstrumentationEngine_HostPath_64=/diag/Instrumentation64/libMicrosoftExtensionsHost.so
ENV MicrosoftInstrumentationEngine_FileLog=Errors
ENV MicrosoftInstrumentationEngine_FileLogPath=/tmp/diag/log.txt

# Install Visual Studio Debugger script prerequisites
RUN apt-get install -y --no-install-recommends \
        bash unzip

# Install Visual Studio Debugger
# Check the repo's README for version compatibility with Visual Studio
RUN curl -SL --output getvsdbg.sh "https://aka.ms/getvsdbgsh" \
    && bash ./getvsdbg.sh -v vs2017u5 -l /diag/vsdbg \
    && rm getvsdbg.sh

# Remove package lists to save space; can be restored with 'apt-get update'
RUN rm -rf /var/lib/apt/lists/*

WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build
WORKDIR /src
COPY ["DasBlog.Web.UI/DasBlog.Web.csproj", "DasBlog.Web.UI/"]
COPY ["DasBlog.Web.Core/DasBlog.Core.csproj", "DasBlog.Web.Core/"]
COPY ["newtelligence.DasBlog.Runtime/newtelligence.DasBlog.Runtime.csproj", "newtelligence.DasBlog.Runtime/"]
COPY ["DasBlog.Services/DasBlog.Services.csproj", "DasBlog.Services/"]
COPY ["DasBlog.Web.Repositories/DasBlog.Managers.csproj", "DasBlog.Web.Repositories/"]
RUN dotnet restore "DasBlog.Web.UI/DasBlog.Web.csproj"
COPY . .
WORKDIR "/src/DasBlog.Web.UI"
RUN dotnet build "DasBlog.Web.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "DasBlog.Web.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "DasBlog.Web.dll"]