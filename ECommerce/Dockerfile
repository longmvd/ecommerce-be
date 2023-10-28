FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY ECommerce.csproj .
# COPY ../GraphQL/Ecommerce.GraphQL.csproj GraphQL/
# COPY ../Common/ECommerce.Common.csproj Common/
# COPY ../Types/Ecommerce.Types.csproj Types/
# COPY ../DL/ECommerce.DL.csproj DL/
# COPY /ECommerce/ECommerce.csproj ECommerce/
# COPY /BL/ECommerce.BL.csproj BL/
COPY ../BL/ECommerce.BL.csproj /src/BL
COPY ../DL/ECommerce.DL.csproj /src/DL
COPY ../Common/ECommerce.Common.csproj /src/Common
COPY ../Types/Ecommerce.Types.csproj /src/Types
COPY ../GraphQL/Ecommerce.GraphQL.csproj /src/GraphQL
RUN dotnet restore
COPY . .
RUN dotnet publish -c release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ECommerce.dll"]
