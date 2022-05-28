FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
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
