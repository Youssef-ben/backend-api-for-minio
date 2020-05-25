.PHONY: 

BACKEND_CONTAINER_NAME		= backend-minio-api-dev
BACKEND_IMAGE_NAME			= $(BACKEND_CONTAINER_NAME)-image:latest-dev
DOCKER_COMPOSE				= docker-compose.yml
DOCKER_COMPOSE_DEV			= docker-compose.development.yml

BACKEND_CONTAINER_UP		= $(shell docker ps --no-trunc --quiet --filter name='^$(BACKEND_CONTAINER_NAME)$$' | wc -l | sed -e 's/^[ \t]*//')

start-minio:
	@echo "Creating and starting the Minio container using {docker-compose}..."
	@docker-compose up -d

stop-minio:
	@echo "Stopping the Minio container using {docker-compose}..."
	@docker-compose stop

restart-minio: stop-minio start-minio

clean-minio:
	@echo "Stopping and deleting the Minio container using {docker-compose}..."
	@docker-compose -f $(DOCKER_COMPOSE)  down

start-docker-app:
	@echo "Starting docker app..."
	@docker-compose -f $(DOCKER_COMPOSE) -f $(DOCKER_COMPOSE_DEV) up -d 

stop-docker-app: ## Stops the APP container if running.
	@echo "Stopping the Backend container..."

	@if [ $(BACKEND_CONTAINER_UP) -eq 1 ]; then \
		docker stop $(BACKEND_CONTAINER_NAME) > /dev/null; \
	fi

remove-docker-app:
	@echo "Removing the backend container..."
	@docker rm $(BACKEND_CONTAINER_NAME)

remove-docker-image:
	@echo "Cleaning the Backend App image..."
	@docker rmi $(BACKEND_IMAGE_NAME)

clean-docker-app: stop-docker-app remove-docker-app remove-docker-image

nuke: 
	@docker-compose -f $(DOCKER_COMPOSE) -f $(DOCKER_COMPOSE_DEV) down