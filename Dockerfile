# Base image
# FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

# Set the working directory
# WORKDIR /app

# Copy the .csproj file and restore dependencies
# COPY *.csproj .
# RUN dotnet restore

# Copy the source code and build the application
# COPY . .
# RUN dotnet build -c Release -o /app/build

# Run unit tests
# RUN dotnet test

# Build the production image
# FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
# WORKDIR /app
# COPY --from=build /app/build .
# ENTRYPOINT ["dotnet", "MyApp.dll"]


FROM mcr.microsoft.com/dotnet/core/sdk:2.2 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:2.2
WORKDIR /app
COPY --from=build-env /app/out .
ENTRYPOINT ["dotnet", "aspnetcoreapp.dll"]
