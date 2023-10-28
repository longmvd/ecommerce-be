FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src
COPY /ECommerce/ECommerce.csproj ECommerce/
# COPY /GraphQL/ GraphQL/
# COPY /Common/ Common/
# COPY /Types/ Types/
# COPY /DL/ DL/
# COPY /ECommerce/ ECommerce/
# COPY /BL/ BL/
# COPY /BL/ECommerce.BL.csproj /BL
# COPY /DL/ECommerce.DL.csproj /DL
# COPY /ECommerce/ECommerce.sln /ECommerce
# COPY /Common/ECommerce.Common.csproj /Common
# COPY /Types/Ecommerce.Types.csproj /Types
# COPY /GraphQL/Ecommerce.GraphQL.csproj /GraphQL

COPY . .
RUN dotnet restore ECommerce/ECommerce.sln
RUN dotnet publish ECommerce/ECommerce.sln -c release -o /app
RUN dotnet tool install -g Microsoft.dotnet-httprepl
RUN dotnet dev-certs https --trust

FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
EXPOSE 5000
COPY --from=build /app .
ENTRYPOINT ["dotnet", "ECommerce.dll"]
