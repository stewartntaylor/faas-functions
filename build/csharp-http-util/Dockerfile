FROM mcr.microsoft.com/dotnet/core/sdk:2.2.203-stretch-arm32v7 as builder

MAINTAINER S Taylor

COPY qemu-arm-static /usr/bin

ENV DOTNET_CLI_TELEMETRY_OPTOUT 1

# Optimize for Docker builder caching by adding projects first.

RUN mkdir -p /root/src/function
WORKDIR /root/src/function
COPY ./function/FunctionHandler.csproj  .

WORKDIR /root/src/
COPY ./root.csproj  .
RUN dotnet restore ./root.csproj

COPY .  .

RUN dotnet publish -c release -o published -r linux-arm

FROM mcr.microsoft.com/dotnet/core/runtime:2.2.4-stretch-slim-arm32v7

RUN apt-get update -qy \
    && apt-get install -qy curl ca-certificates --no-install-recommends \ 
    && echo "Pulling watchdog binary from Github." \
    && curl -sSLf https://github.com/openfaas-incubator/of-watchdog/releases/download/0.2.3/of-watchdog-armhf > /usr/bin/fwatchdog \
    && chmod +x /usr/bin/fwatchdog \
    && apt-get -qy remove curl \
    && apt-get clean \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /root/
COPY --from=builder /root/src/published .

RUN apt-get update && apt-get install -qy libunwind8

ENV fprocess="dotnet ./root.dll"
ENV cgi_headers="true"
ENV mode="http"
ENV upstream_url="http://127.0.0.1"

EXPOSE 8080
CMD ["fwatchdog"]
