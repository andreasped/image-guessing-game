FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build-env
WORKDIR /GuessingGame

EXPOSE 80
EXPOSE 443

COPY *.csproj ./
RUN dotnet restore

COPY . ./

RUN dotnet publish -c Release -o out
RUN dotnet dev-certs https
RUN dotnet tool install --global dotnet-ef --version 7.0.13
ENV PATH="$PATH:/root/.dotnet/tools"

RUN mkdir -p Infrastructure/Data

RUN dotnet ef migrations add database -o ./Migrations
RUN dotnet ef database update

FROM mcr.microsoft.com/dotnet/aspnet:7.0
WORKDIR /GuessingGame

COPY --from=build-env /GuessingGame/out .
COPY --from=build-env /GuessingGame/Infrastructure/Data ./Infrastructure/Data
COPY --from=build-env /GuessingGame/Assets ./Assets
COPY --from=build-env /root/.dotnet/corefx/cryptography/x509stores/my/* /root/.dotnet/corefx/cryptography/x509stores/my/

ENTRYPOINT ["dotnet", "GuessingGame.dll"]
