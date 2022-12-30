# Identity Provider

Microservice used for authenticating users with JWT. Provides generic functionality that can be scaled up depending on the system requirements.

To run locally, use the command: 
###### dotnet run

To run inside Docker, run the commands:
###### docker build -t identity_web_api .
###### docker-compose up

To see a list of available endpoints, access the Swagger doc at http://localhost:5000/swagger/index.html. Any URL in the Doc has to be appended to the service's address (http://localhost:5000).
