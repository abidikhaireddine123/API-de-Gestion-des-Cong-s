API de Gestion des Congés (OData)
Cette API ASP.NET Core 8 permet de gérer les demandes de congé des employés via des endpoints OData et REST. Elle utilise SQLite comme base de données pour une configuration légère.
Fonctionnalités

Création, mise à jour, suppression, et récupération des demandes de congé via OData.
Validation des règles métier :
Pas de chevauchement des dates de congé pour un employé.
Maximum 20 jours de congé annuel par an.
Congé maladie avec raison obligatoire.


Rapport récapitulatif des congés par employé (total, annuel, maladie) via REST.
Approbation des demandes en attente par les administrateurs via REST.
Persistance des données avec SQLite.

Structure de la Table LeaveRequest

Id : int, clé primaire, identifiant unique.
EmployeeId : int, clé étrangère vers l'employé.
LeaveType : string, valeurs : "Annual", "Sick", "Other".
StartDate : DateTime, date de début du congé.
EndDate : DateTime, date de fin du congé.
Status : string, valeurs : "Pending", "Approved", "Rejected".
Reason : string, raison du congé (obligatoire pour "Sick").
CreatedAt : DateTime, date de création.

Prérequis

.NET 8 SDK

Docker et Docker Compose (pour l'exécution conteneurisée)

Un éditeur comme Visual Studio Code ou Visual Studio

(Optionnel) Entity Framework Core CLI pour les migrations :
dotnet tool install --global dotnet-ef


(Optionnel) Postman pour tester les endpoints




Installation et Exécution Locale
1. Cloner le Répertoire
git clone <url-du-répertoire>
cd WebApi

2. Restaurer les Dépendances
dotnet restore

3. Configurer la Base de Données SQLite

La chaîne de connexion est dans appsettings.json :
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  }
}


Générer et appliquer les migrations :
dotnet ef migrations add InitialCreate
dotnet ef database update



4. Exécuter l'Application
dotnet run


L'API sera accessible à http://localhost:8080.

Exécution avec Docker
1. Construire et Lancer avec Docker Compose
docker-compose up -d


L'API est accessible à http://localhost:8080.
La base de données SQLite est persistée dans le volume sqlite-data.

2. Arrêter les Services
docker-compose down


Pour supprimer les données SQLite :
docker-compose down -v



3. Logs
docker-compose logs

Configuration Docker

Dockerfile : Construit une image pour .NET 8 avec SQLite.
docker-compose.yml :
Service webapi : Exécute l'API sur le port 8080.
Volume sqlite-data : Persiste app.db dans /app/data.
Variables d'environnement :
ASPNETCORE_URLS=http://+:8080
ConnectionStrings__DefaultConnection=Data Source=/app/data/app.db





Tester avec Postman
Une collection Postman est fournie dans LeaveRequestAPI.postman_collection.json.
1. Importer la Collection

Ouvrez Postman.
Cliquez sur Import > File > Sélectionnez LeaveRequestAPI.postman_collection.json.
La collection inclut :
Requêtes OData pour lister, récupérer, créer, mettre à jour, et supprimer des demandes.
Requêtes REST pour approuver une demande et obtenir un rapport.



2. Configurer l'Environnement

Créez un environnement avec :
Variable : baseUrl
Valeur : http://localhost:8080


Sélectionnez cet environnement.

3. Exécuter les Requêtes

Testez les requêtes OData (par exemple, GET odata/LeaveRequest?$filter=Status eq 'Pending').
Ajustez les paramètres (par exemple, id, year).

Exemples d'Utilisation
1. Lister les Demandes de Congé (OData)
curl "http://localhost:8080/odata/LeaveRequest?$filter=Status eq 'Pending'&$select=Id,EmployeeId,StartDate"


Réponse :
{
  "value": [
    {
      "Id": 1,
      "EmployeeId": 1,
      "StartDate": "2025-06-01T00:00:00Z"
    }
  ]
}



2. Créer une Demande de Congé (OData)
curl -X POST http://localhost:8080/odata/LeaveRequest \
-H "Content-Type: application/json" \
-d '{
  "EmployeeId": 1,
  "LeaveType": "Annuel",
  "StartDate": "2025-06-01",
  "EndDate": "2025-06-05",
  "Reason": "Vacances",
  "Status": "Pending"
}'

3. Approuver une Demande (REST)
curl -X POST http://localhost:8080/odata/LeaveRequest/1/approve


Réponse : "Demande de congé approuvée."

4. Obtenir un Rapport Récapitulatif (REST)
curl "http://localhost:8080/odata/LeaveRequest/report?year=2025&department=IT"


Réponse :
{
  "items": [
    {
      "employeeName": "Jean Dupont",
      "totalLeaves": 15,
      "annualLeaves": 10,
      "sickLeaves": 5
    }
  ]
}



Gestion de la Base de Données

SQLite : Le fichier app.db est créé dans /app/data (conteneur) ou localement (app.db).
Migrations : Appliquées automatiquement au démarrage si configuré dans Program.cs, ou manuellement avec dotnet ef database update.
Persistance : Le volume sqlite-data conserve les données dans Docker.

Notes

OData : Utilisez $filter, $select, $orderby, etc., pour des requêtes flexibles (par exemple, GET odata/LeaveRequest?$filter=LeaveType eq 'Sick'&$top=5).
Sécurité : Pour la production, configurez HTTPS et une authentification (par exemple, JWT). Restreignez l'endpoint d'approbation avec [Authorize(Roles = "Admin")].
Port : 8080 par défaut. Modifiez docker-compose.yml si nécessaire.
Nom du Projet : Si la DLL n'est pas WebApi.dll, mettez à jour ENTRYPOINT dans Dockerfile.
Performance : SQLite convient aux petites applications. Pour des charges élevées, envisagez PostgreSQL.

Dépannage

Erreur OData : Vérifiez la configuration OData dans Program.cs (par exemple, AddOData).
Erreur SQLite : Vérifiez la chaîne de connexion.
Erreur Docker : Assurez-vous que Dockerfile et .csproj sont à la racine.
Logs : Utilisez docker-compose logs.
Support : Contactez l'équipe pour des problèmes spécifiques.


Licence : MIT
