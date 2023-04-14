docker kill $(docker container ls -a -q)
docker rm $(docker container ls -a -q)
docker rmi $(docker images -a -q) -f
docker network prune -f