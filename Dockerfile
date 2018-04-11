FROM microsoft/dotnet
RUN mkdir /app && cd /app
COPY . /app
WORKDIR /app/eshop.api.article
RUN dotnet restore
CMD [ "dotnet", "run" ]
EXPOSE 8001
