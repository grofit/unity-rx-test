FROM mcr.microsoft.com/dotnet/sdk:5.0 AS deps-build
WORKDIR /app/Dependencies
COPY ./Dependencies ./
RUN dotnet build
RUN rm ./bin/Debug/netstandard2.1/System.Runtime.InteropServices.WindowsRuntime.dll


FROM alpine/git AS rx-retriver
WORKDIR /app
RUN git clone --branch rxnet-v5.0.0 --depth 1 https://github.com/dotnet/reactive.git
RUN cp ./reactive/Rx.NET/Source/Directory.build.props ./reactive/Rx.NET/Source/src/System.Reactive/Directory.build.props

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS rx-build
WORKDIR /app
COPY --from=rx-retriver /app ./
COPY ./Patcher ./Patcher/

WORKDIR /app/Patcher
RUN dotnet run

WORKDIR /app
COPY ./Patches/SchedulerDefaults.cs ./reactive/Rx.NET/Source/src/System.Reactive/Concurrency/SchedulerDefaults.cs
COPY ./Patches/InternalsVisibleTo.cs ./reactive/Rx.NET/Source/src/System.Reactive/Properties/InternalsVisibleTo.cs
COPY ./Patches/global.json ./reactive/Rx.NET/Source/src/global.json

WORKDIR /app/reactive/Rx.NET/Source/src/System.Reactive
RUN dotnet build


FROM alpine/git AS rxdata-retriver
WORKDIR /app
RUN git clone --branch main --depth 1 https://github.com/Cosmic-Shores/Rx.Data.git

FROM mcr.microsoft.com/dotnet/sdk:5.0 AS rxdata-build
WORKDIR /app/Rx.Data
COPY --from=rx-build /app/reactive/Rx.NET/Source/src/System.Reactive/bin/Debug/netstandard2.0 ./src/
COPY --from=rxdata-retriver /app ../
COPY ./Patches/Directory.build.props ./src/Directory.build.props
RUN dotnet build


FROM scratch AS export-stage
COPY --from=deps-build /app/Dependencies/bin/Debug/netstandard2.1 ./
COPY --from=rx-build /app/reactive/Rx.NET/Source/src/System.Reactive/bin/Debug/netstandard2.0 ./
COPY --from=rxdata-build /app/Rx.Data/src/Rx.Extendibility/bin/Debug/netstandard2.0 ./
COPY --from=rxdata-build /app/Rx.Data/src/Rx.Data/bin/Debug/netstandard2.0 ./
