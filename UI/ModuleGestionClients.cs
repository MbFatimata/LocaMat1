using System;
using LocaMat.Metier;
using LocaMat.UI.Framework;
using System.Data.SqlClient;
using System.Configuration;

namespace LocaMat.UI
{
    public class ModuleGestionClients
    {
        private Menu menu;

        private void InitialiserMenu()
        {
            this.menu = new Menu("Gestion des clients");
            this.menu.AjouterElement(new ElementMenu("1", "Afficher les clients")
            {
                FonctionAExecuter = this.AfficherClients
            });
            this.menu.AjouterElement(new ElementMenu("2", "Ajouter un client")
            {
                FonctionAExecuter = this.AjouterClient
            });
            this.menu.AjouterElement(new ElementMenuQuitterMenu("R", "Revenir au menu principal..."));
        }

        public void Demarrer()
        {
            if (this.menu == null)
            {
                this.InitialiserMenu();
            }

            this.menu.Afficher();
        }

        private void AfficherClients()
        {
            ConsoleHelper.AfficherEntete("Clients");

            var connectionString = ConfigurationManager.ConnectionStrings["Connexion"].ConnectionString;
            var connexion = new SqlConnection(connectionString);
            var commande = new SqlCommand("SELECT * FROM Clients", connexion);
            connexion.Open();
            SqlDataReader dataReader = commande.ExecuteReader();
            while (dataReader.Read())
            {
                Console.WriteLine($"Id:{dataReader.GetInt32(0)}, Nom: {dataReader.GetString(1)}, Prenom: {dataReader.GetString(2)} , Adresse: {dataReader.GetString(3)}");
            }
            connexion.Close();
        }

        private void AjouterClient()
        {
            ConsoleHelper.AfficherEntete("Nouveau client");

            var client = new Client
            {
                Nom = ConsoleSaisie.SaisirChaine("Nom : ", false),
                Prenom = ConsoleSaisie.SaisirChaine("Prenom : ", false),
                Adresse = ConsoleSaisie.SaisirChaine("Adresse : ", false)
            };
            using (var connexion = GetConnection())
            {
                // Creation d'une commande sql
                var sql = "INSERT INTO Clients (Nom, Prenom, Adresse) VALUES (@NOM, @PRENOM, @ADRESSE )";
                var commande = new SqlCommand(sql, connexion);
                commande.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Nom",
                    Value = client.Nom,

                });
                commande.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Prenom",
                    Value = client.Prenom,

                });
                commande.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Adresse",
                    Value = client.Adresse,

                });
                connexion.Open();
                commande.ExecuteNonQuery();
                connexion.Close();
            }
        }
        private static SqlConnection GetConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Connexion"].ConnectionString;
            return new SqlConnection(connectionString);
        }
    }
}
