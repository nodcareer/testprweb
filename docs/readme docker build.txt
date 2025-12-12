# publish 
dotnet publish <name>.csproj -o published

# no exe file in published
cd .\testpr.web
dotnet publish .\testpr.web.csproj -o published  /p:UseAppHost=false  

#dockerfile creation
new file "Dockerfile"
docker build -t testprweb:latest . 

# multistage dockerfile creation
new file "Dockerfile"
docker build -t testprweb:latest .

# docker image directly from dotnet sdk
dotnet publish --os linux --arch x64 /t:publishContainer -p ContainerImageTag=1.1

# publish docker image to dockerhub
docker tag hello:latest <repo-name>/hello:1.0
docker push <repo-name>/hello:1.0

>>>>>>>>>>>>>>>>

docker run -d --rm -p 8080:8080  --name testprweb-cont testprweb 

>>>>>>>>>>>>>>>>>>>>
Docker Commands
>>>>>>>>>>>>>>>>>
docker run -d --rm -p 8080:80 v nginx-data:/usr/share/nginx/html --name nginx-test nginx

docker exec -it nginx-test /bin/bash

docker volume list
docker ps
docker ls

docker tag hello:latest hello:1.0