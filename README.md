API de Gestion des Congés
Cette API ASP.NET Core 8 permet de gérer les demandes de congé des employés, avec des fonctionnalités pour créer, approuver, et générer des rapports sur les congés. Elle utilise SQLite comme base de données pour une configuration légère.
Fonctionnalités

Création et gestion des demandes de congé (annuel, maladie).
Validation des règles métier :
Pas de chevauchement des dates de congé pour un employé.
Maximum 20 jours de congé annuel par an.
Congé maladie avec raison obligatoire.


Rapport récapitulatif des congés par employé (total, annuel, maladie).
Approbation des demandes en attente par les administrateurs.
Persistance des données avec SQLite.

Prérequis

.NET 8 SDK

Docker et Docker Compose (pour l'exécution conteneurisée)

Un éditeur comme Visual Studio Code ou Visual Studio

(Optionnel) Entity Framework Core CLI pour les migrations :
dotnet tool install --global dotnet-ef





Installation et Exécution Locale
1. Cloner le Répertoire
git clone <url-du-répertoire>
cd WebApi

2. Restaurer les Dépendances
dotnet restore

3. Configurer la Base de Données SQLite

La chaîne de connexion par défaut est définie dans appsettings.json :
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=app.db"
  }
}


Générer et appliquer les migrations Entity Framework Core :
dotnet ef migrations add InitialCreate
dotnet ef database update



4. Exécuter l'Application
dotnet run


L'API sera accessible à http://localhost:8080 (ou le port configuré dans appsettings.json ou Program.cs).

Exécution avec Docker
1. Construire et Lancer avec Docker Compose
Assurez-vous que Dockerfile et docker-compose.yml sont à la racine du projet.
docker-compose up -d


Cela construit l'image et lance le conteneur.
L'API est accessible à http://localhost:8080.
La base de données SQLite est persistée dans un volume nommé sqlite-data.

2. Arrêter les Services
docker-compose down


Pour supprimer le volume (et les données SQLite) :
docker-compose down -v



3. Logs
Pour voir les logs du conteneur :
docker-compose logs

Configuration Docker

Dockerfile : Construit une image optimisée pour .NET 8 avec SQLite.
docker-compose.yml :
Service webapi : Exécute l'API sur le port 8080.
Volume sqlite-data : Persiste le fichier app.db dans /app/data.
Variables d'environnement :
ASPNETCORE_URLS=http://+:8080
ConnectionStrings__DefaultConnection=Data Source=/app/data/app.db





Exemples d'Utilisation
1. Créer une Demande de Congé
curl -X POST http://localhost:8080/api/leaverequests \
-H "Content-Type: application/json" \
-d '{
  "employeeId": 1,
  "leaveType": "Annuel",
  "startDate": "2025-06-01",
  "endDate": "2025-06-05",
  "reason": "Vacances",
  "status": "En attente"
}'

2. Approuver une Demande
curl -X POST http://localhost:8080/api/leaverequests/1/approve


Réponse : "Demande de congé approuvée."

3. Obtenir un Rapport Récapitulatif
curl "http://localhost:8080/api/leaverequests/report?year=2025&department=IT"


Exemple de réponse :
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
Persistance : Dans Docker, le volume sqlite-data conserve les données entre les redémarrages.

Notes

Sécurité : Pour la production, configurez HTTPS et une authentification (par exemple, JWT). L'endpoint d'approbation peut être restreint aux admins avec [Authorize(Roles = "Admin")].
Port : Par défaut, 8080. Modifiez docker-compose.yml ou appsettings.json si nécessaire.
Nom du Projet : Si la DLL principale n'est pas WebApi.dll, mettez à jour ENTRYPOINT dans Dockerfile.
Performance : SQLite convient aux petites applications. Pour des charges élevées, envisagez PostgreSQL ou SQL Server.

Dépannage

Erreur de connexion SQLite : Vérifiez la chaîne de connexion dans appsettings.json ou docker-compose.yml.
Erreur de build Docker : Assurez-vous que le Dockerfile et le .csproj sont à la racine.
Logs : Utilisez docker-compose logs pour diagnostiquer les erreurs.
Support : Contactez l'équipe de développement pour des problèmes spécifiques.



