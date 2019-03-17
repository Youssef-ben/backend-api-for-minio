.PHONY: init-docker start-docker stop-docker restart-docker nuke  init-docker-dev start-docker-dev refresh-api-dev

EsContainerName='backend-elasticsearch'

BaseCompose="docker-compose.yml"
DevCompose="docker-compose.dev.yml"
ProdCompose="docker-compose.prod.yml"

init-docker:
	@echo "Creating and starting the containers using docker-compose..."
	@docker-compose up -d

	@echo "installing Elasticsearch ingest-attachement plugin..."
	@docker exec $(EsContainerName) bin/elasticsearch-plugin install -b ingest-attachment

	@echo "Restarting Elasticsearch..."
	@docker stop $(EsContainerName)
	@docker start $(EsContainerName)

	@echo "done."

start-docker:
	@echo "Creating the containers using docker-compose..."
	@docker-compose up -d

stop-docker:
	@echo "Stopping the containers using docker-compose..."
	@docker-compose stop

restart-docker: stop-docker start-docker

nuke:
	@echo "Stopping and deleting the containers using docker-compose..."
	@docker-compose -f $(BaseCompose) -f $(DevCompose) -f $(ProdCompose) down

init-docker-dev:
	@echo "starting Docker for Environment={Development}..."
	@docker build -t backend-api .
	@docker-compose -f $(BaseCompose) -f $(DevCompose) up -d 

	@echo "installing Elasticsearch ingest-attachement plugin..."
	@docker exec $(EsContainerName)-dev bin/elasticsearch-plugin install -b ingest-attachment

	@echo "Restarting Elasticsearch..."
	@docker stop $(EsContainerName)-dev
	@docker start $(EsContainerName)-dev

	@echo "done."

start-docker-dev:
	@echo "starting Docker for Environment={Development}..."
	@docker build -t backend-api .
	@docker-compose -f $(BaseCompose) -f $(DevCompose) up -d 

refresh-api-dev:
	@echo "Refreshing the backend API with Environment={Development}..."
	@docker build -t backend-api .
	@docker-compose -f $(BaseCompose) -f $(DevCompose) up -d --no-deps --build backend-api