# API for the object storage server (Minio)

Backend API, is a single access point to manage your Object Storage Server (minio).

## Tools and Technologies

### 1. Technologies

- **Web API** : netCore 3.1 LTS.
- **XUnit** : Testing framework. (Not yet)
- **Swagger** : Documentations for the Web API.
- **StyleCop** : Code Analyzer to enforce a set of style and consistency rules.
- **Minio** : Object Storage Server.
- **Docker** : Container platform.

### 2. Tools

- **Visual Studio 2017 Community - v15.x**
- **Postman**
- **Make tool**

## Getting started

### 1. Rules

In the API Project always use the Manager library for the bussiness rules.

- [General Branching Rules with Git.](https://gist.github.com/digitaljhelms/4287848)
- [General Versionnig Rules.](https://semver.org/)
- [API Urls Standards.](https://github.com/WhiteHouse/api-standards)
- [API Best practices.](https://medium.com/@schneidenbach/restful-api-best-practices-and-common-pitfalls-7a83ba3763b5)
- [Guide for declaring issues.](https://guides.github.com/features/issues/)

### 2. Useful commands

This commands are based on the Makefile. for more informations see [MakeFile.](https://github.com/Youssef-ben/backend-api-for-minio/blob/dev/Makefile)

```bash
## ================== MINIO CONTAINER ====================== ##
make start-minio

make stop-minio

make restart-minio

make clean-minio


## ================== APP CONTAINER ======================== ##
make start-docker-app

make stop-docker-app

make remove-docker-app

make remove-docker-image

make clean-docker-app

make nuke
```

### 3. API Endpoints

For more details about the endpoints, you can run the project locally and go to http://localhost:5100/swagger