using System;
using System.Collections.Generic;
using LocaMat.Metier;
using LocaMat.UI.Framework;
using System.Data.SqlClient;
using System.Configuration;

namespace LocaMat.UI
{
    public class ModuleGestionAgences
    {
        private Menu menu;

        private void InitialiserMenu()
        {
            this.menu = new Menu("Gestion des agences");
            this.menu.AjouterElement(new ElementMenu("1", "Afficher les agences")
            {
                FonctionAExecuter = this.AfficherAgences
            });
            this.menu.AjouterElement(new ElementMenu("2", "Ajouter une agence")
            {
                FonctionAExecuter = this.AjouterAgence
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

        private void AfficherAgences()
        {
            ConsoleHelper.AfficherEntete("Agences");

            var connectionString = ConfigurationManager.ConnectionStrings["Connexion"].ConnectionString;
            var connexion = new SqlConnection(connectionString);
            var commande = new SqlCommand("SELECT * FROM Agences", connexion);
            connexion.Open();
            SqlDataReader dataReader = commande.ExecuteReader();
            while (dataReader.Read())
            {
                Console.WriteLine($"Id:{dataReader.GetInt32(0)}, Ville: {dataReader.GetString(1)}, Adresse: {dataReader.GetString(2)}");
            }
            connexion.Close();
        }

        private void AjouterAgence()
        {
            ConsoleHelper.AfficherEntete("Nouvelle agence");
            Console.Write("Entrez la nouvelle ville: ");
            string ville = Console.ReadLine();
            Console.Write("Entrez l'adresse: ");
            string adresse = Console.ReadLine();

            using (var connexion = GetConnection())
            {
                // Creation d'une commande sql
                var sql = "INSERT INTO Agences (Ville,Adresse) VALUES (@VILLE, @ADRESSE )";
                var commande = new SqlCommand(sql, connexion);
                commande.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Ville",
                    Value = ville,

                });
                commande.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Adresse",
                    Value = adresse,

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
