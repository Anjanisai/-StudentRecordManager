# Use the official Microsoft SDK image to build and compile the application code
FROM ://microsoft.com AS build-env
WORKDIR /app

# Copy the project configuration tracking maps and restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy all remaining source application layout code files and build the production binaries
COPY . ./
RUN dotnet publish -c Release -o out

# Build the lightweight runtime container hosting layer
FROM ://microsoft.com
WORKDIR /app
COPY --from=build-env /app/out .

# Tell the engine where the local embedded database table resides
ENV ASPNETCORE_URLS=http://+:10000
EXPOSE 10000

ENTRYPOINT ["dotnet", "StudentRecordManager.dll"]

