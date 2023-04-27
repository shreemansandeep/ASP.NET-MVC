# Set the base image
FROM mcr.microsoft.com/dotnet/sdk:5.0 AS build-env

# Set the working directory
WORKDIR /app

# Copy the project file
COPY *.csproj ./

# Restore the project dependencies
RUN dotnet restore

# Copy the entire project folder to the container
COPY . ./

# Publish the application
RUN dotnet publish -c Release -o out

# Set the final base image
FROM mcr.microsoft.com/dotnet/aspnet:5.0

# Set the working directory
WORKDIR /app

# Copy the published output from the build container to the final container
COPY --from=build-env /app/out .

# Set the environment variable for ASP.NET Core to use HTTPS
ENV ASPNETCORE_URLS=https://+:443

# Expose port 443 for HTTPS traffic
EXPOSE 443

# Start the application
ENTRYPOINT ["dotnet", "YourAppName.dll"]

