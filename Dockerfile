FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build-env
WORKDIR /App

# Copy everything
COPY . ./
RUN rm retrofit.sln
# Restore as distinct layers
RUN dotnet restore

# Build and publish a release
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /App
COPY --from=build-env /App/out .

# Install python dependencies (for ML models)
COPY requirements.txt /App/requirements.txt
RUN apt-get update && apt-get install -y python3-pip
RUN pip3 install -r requirements.txt --break-system-packages

# Copy Python models into the correct directory
COPY Models /App/Models

ENTRYPOINT ["dotnet", "Retrofit.dll"]
