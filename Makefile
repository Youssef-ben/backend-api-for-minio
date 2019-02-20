.PHONY: start-docker stop-docker nuke

start-docker:
	@echo "Starting the containers using docker-compose..."
	@docker-compose up -d

stop-docker:
	@echo "Stopping the containers using docker-compose..."
	@docker-compose stop
	
nuke:
	@echo "Stopping and deleting the containers using docker-compose..."
	@docker-compose down

	
##bin/elasticsearch-plugin install ingest-attachment
	