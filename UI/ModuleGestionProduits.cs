using System;
using System.Collections.Generic;
using LocaMat.Metier;
using LocaMat.UI.Framework;
using System.IO;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Configuration;
namespace LocaMat.UI
{
    public class ModuleGestionProduits
    {
        private Menu menu;

        private void InitialiserMenu()
        {
            this.menu = new Menu("Gestion des produits");
            this.menu.AjouterElement(new ElementMenu("1", "Afficher les produits")
            {
                FonctionAExecuter = this.AfficherProduits
            });
            this.menu.AjouterElement(new ElementMenu("2", "Ajouter un produit")
            {
                FonctionAExecuter = this.AjouterProduit
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

        private void AfficherProduits()
        {
            ConsoleHelper.AfficherEntete("Produits");

            var connectionString = ConfigurationManager.ConnectionStrings["Connexion"].ConnectionString;
            var connexion = new SqlConnection(connectionString);
            var commande = new SqlCommand("SELECT * FROM Produits", connexion);
            connexion.Open();
            SqlDataReader dataReader = commande.ExecuteReader();
            while (dataReader.Read())
            {
                Console.WriteLine($"Id:{dataReader.GetInt32(0)}, Nom: {dataReader.GetString(1)}, DESCRIPTION: {dataReader.GetString(2).Tronquer(20)} , IdCategorie: {dataReader.GetInt32(3)}, PrixJourHt {dataReader.GetDecimal(4)}");
            }
            connexion.Close();

           // ConsoleHelper.AfficherListe(liste);



        }

        private void AjouterProduit()
        {
            ConsoleHelper.AfficherEntete("Nouveau produit");


            Console.Write("Nom du nouveau produit: ");
            var nom = Console.ReadLine();
            Console.Write("Description du nouveau produit: ");
            var description = Console.ReadLine();
            Console.Write("Prix du nouveau produit: ");
            var prixJourHt = decimal.Parse(Console.ReadLine());
            AfficherCategories();
            Console.Write("ID Catégorie du nouveau produit: ");
            var idCategorie = Console.ReadLine();

            using (var connexion = GetConnection())
            {
                // Creation d'une commande sql
                var sql = "INSERT INTO Produits (Nom,Description, PrixJourHt, IdCategorie) VALUES (@NOM, @DESCRIPTION, @PRIXJOURHT, @IDCATEGORIE )";
                var commande = new SqlCommand(sql, connexion);
                commande.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Nom",
                    Value = nom,

                });
                commande.Parameters.Add(new SqlParameter
                {
                    ParameterName = "Description",
                    Value = description,

                });
                commande.Parameters.Add(new SqlParameter
                {
                    ParameterName = "PrixJourHt",
                    Value = prixJourHt,

                });
                commande.Parameters.Add(new SqlParameter
                {
                    ParameterName = "IdCategorie",
                    Value = idCategorie,

                });
                connexion.Open();
                commande.ExecuteNonQuery();
                connexion.Close();
            }

        }

        private static void AfficherCategories()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Connexion"].ConnectionString;
            var connexion = new SqlConnection(connectionString);
            var commande = new SqlCommand("SELECT * FROM CategoriesProduits", connexion);
            connexion.Open();
            SqlDataReader dataReader = commande.ExecuteReader();
            while (dataReader.Read())
            {
                Console.WriteLine($"Id:{dataReader.GetInt32(0)}, Nom: {dataReader.GetString(1)}");
            }
            connexion.Close();
        }

        private static SqlConnection GetConnection()
        {
            var connectionString = ConfigurationManager.ConnectionStrings["Connexion"].ConnectionString;
            return new SqlConnection(connectionString);
        }

    }
    public static class ExtensionsString
    {
        public static string Tronquer(this string valeur, int nombreCaracteres)
        {
            const string points = "...";
            return string.IsNullOrEmpty(valeur) || valeur.Length <= nombreCaracteres
                ? valeur : valeur.Substring(0, nombreCaracteres - points.Length) + points;
        }
    }
}
