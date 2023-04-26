using System;
using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.IO;

namespace Velomax_Core
{
    class Program
    {
        private static MySqlConnection connexion = null;

        static Program()
        {
            try
            {
                string mysqlinfo = $"SERVER=localhost;PORT=3306;DATABASE=velomax;UID=velomax;PASSWORD=password";
                connexion = new MySqlConnection(mysqlinfo);
                connexion.Open();
                //Console.WriteLine("> Connecté à la base de donnée");
            }
            catch (Exception e)
            {
                Console.WriteLine("> Erreur connexion : " + e.ToString());
            }
        }

        static string ToString<T>(List<T> liste)
        {
            string r = "";
            for(int i = 0; i < liste.Count; i++)
            {
                r += liste[i] + " ";
            }
            return r;
        }

        static void Main(string[] args)
        { 
            //CreerClientParticulier("Cluet", "Arthur", "arthur.cluet@edu.devinci.fr", "0647642460", "21 Allee Jean Moulin", "Carrières sous Poissy", "78955", "Ile de France");
            //CreerClientParticulier("Cournil", "Arnaud", "arnaud.cournil@edu.devinci.fr", "0612345678", "53 Rue Jeanne d'Arc", "Paris", "75013", "Ile de France");
            //CreerClientParticulier("De Chantérac", "Arthur", "arthur.de_chanterac@edu.devinci.fr", "0798765412", "5 Avenue Foch", "Paris", "75001", "Ile de France");
            //CreerCommande(1, new int[] { 110, 111, 111 }, new string[] { });
 
            //(bool, string, string) result = CreerCommande(1, new int[] { 101, 101 }, new string[] { "C32" });
            //CreerCommande(1, new int[] { 110, 111, 111 }, new string[] { });

            //SupprimerCommande(1);

            //Console.WriteLine(SupprimerCommande(1));

            MainMenu();

            connexion.Close();
        }


        static void MainMenu(int selected = 0)
        {
            Console.Clear();

            string[] liens = new string[] { "Clients", "Commandes", "Vélos", "Pièces", "Statistiques", "Demo" };
            Console.WriteLine("***** MENU PRINCIPAL ***** (utilisez les flèches et la touche Entrée)\n");
            for(int i = 0; i < liens.Length; i++)
            {
                Console.WriteLine($"[{ ((i == selected) ? "*" : " ") }] {liens[i]}");
            }

            ConsoleKey r = Console.ReadKey().Key;
            if (r == ConsoleKey.DownArrow)
            {
                MainMenu(selected == liens.Length - 1 ? selected : selected + 1);
            } else if (r == ConsoleKey.UpArrow)
            {
                MainMenu(selected == 0 ? selected : selected - 1);
            } else if (r == ConsoleKey.Escape)
            {
                MainMenu(selected);
            
            } else if(r == ConsoleKey.Enter)
            {
                // do smth
                if (selected == 0) MenuClients();
                else if (selected == 1) MenuCommandes();
                else if (selected == 2) MenuVoirVelos();
                else if (selected == 3) MenuPieces();
                else if (selected == 4) MenuStats();
                else if (selected == 5) Demo();
                else MainMenu(selected);
            }
        }

        static void MenuStats()
        {
            Console.Clear();
            Console.WriteLine("***** STATISTIQUES *****");

            Console.WriteLine("\nNombre de ventes par pièce & vélo :");
            foreach(string line in VentesPieces())
            {
                Console.WriteLine(line);
            }
            foreach (string line2 in VentesVelos())
            {
                Console.WriteLine(line2);
            }

            Console.WriteLine("\nClients abonnés à Fidelio :");
            string[] clientfidelio = ClientParProgramme();
            foreach(string line3 in clientfidelio)
            {
                Console.WriteLine(line3);
            }

            Console.WriteLine("\nMeilleurs clients (nombre à spécifier en paramètres):");
            string[] tot = MeilleursClient();
            foreach (string t in tot)
            {
                Console.WriteLine(t);
            }

            Console.WriteLine("\nStats commandes :");
            string[] sc = StatsCommandes();
            foreach (string t in sc)
            {
                Console.WriteLine(t);
            }

            Console.ReadKey();
            MainMenu();
        } 

        static void Demo()
        {
            Console.Clear();
            Console.WriteLine("***** DEMO *****\n");
            Console.WriteLine("Nombre de clients : " + GetClientsParticuliers().Length + "\n");

            Console.ReadKey();

            string[] tot = GetTotalCommandes();
            foreach(string t in tot)
            {
                Console.WriteLine(t);
            }
            Console.WriteLine();
            Console.ReadKey();

            Dictionary<int, int> sv = StocksVelos();
            Dictionary<string, int> sp = StocksPieces();
            foreach(KeyValuePair<int, int> svkvp in sv)
            {
                if(svkvp.Value <= 2)
                    Console.WriteLine($"Vélo {svkvp.Key} : {svkvp.Value}");
            }
            foreach (KeyValuePair<string, int> spkvp in sp)
            {
                if (spkvp.Value <= 2)
                    Console.WriteLine($"Pièce {spkvp.Key} : {spkvp.Value}");
            }
            Console.WriteLine();
            Console.ReadKey();

            Dictionary<string, int> ppf = PiecesParFournisseur();
            foreach(KeyValuePair<string, int> a in ppf)
            {
                Console.WriteLine($"Fournisseur {a.Key} : {a.Value}");
            }
            Console.WriteLine();
            Console.ReadKey();

            string fidelioXML = ExportFidelioXML();
            Console.WriteLine(fidelioXML);


            Console.ReadKey();

            MainMenu();
        }

        static void MenuClients(int selected = 0)
        {
            Console.Clear();
            Console.WriteLine("***** CLIENTS *****\n");

            string[] clients = GetClientsParticuliers();
            int[] idclients = new int[clients.Length];
            for(int i = 0; i < clients.Length; i++)
                idclients[i] = Convert.ToInt32(clients[i].Split(' ')[0]);

            for(int i = 0; i < clients.Length; i++)
            {
                //int id = Convert.ToInt32(client.Split(' ')[0]);
                Console.WriteLine($"[{ ((i == selected) ? "*" : " ") }] " + clients[i]);
                if (i == clients.Length - 1) Console.WriteLine($"[{ ((i + 1 == selected) ? "*" : " ") }] " + "Nouveau client");
            }

            ConsoleKey r = Console.ReadKey().Key;
            if (r == ConsoleKey.DownArrow)
            {
                MenuClients(selected == clients.Length ? selected : selected + 1);
            }
            else if (r == ConsoleKey.UpArrow)
            {
                MenuClients(selected == 0 ? selected : selected - 1);
            }
            else if (r == ConsoleKey.Escape)
            {
                MainMenu();
            } else if(r == ConsoleKey.Enter)
            {
                // so smth
                if(selected < clients.Length)
                {
                    // Modifier client
                    MenuModifierClient(idclients[selected]);
                } else
                {
                    MenuCreerClient();
                }
            } else
            {
                MenuClients(selected);
            }

        }

        static void MenuModifierClient(int idClient)
        {
            Console.Clear();
            Console.WriteLine("*****  MODIFIER CLIENT : *****\n");

            string[] client = GetClient(idClient);
            string nom = client[0];
            string prenom = client[1];
            string email = client[2];
            string telephone = client[3];
            string voie = client[4];
            string cp = client[5];
            string ville = client[6];
            string province = client[7];

            string nom2;
            string prenom2;
            string email2;
            string telephone2;
            string voie2;
            string cp2;
            string ville2;
            string province2;

            Console.Write($"Nom ({nom}) : ");
            nom2 = Console.ReadLine();
            Console.Write($"Prénom ({prenom}) : ");
            prenom2 = Console.ReadLine();
            Console.Write($"Email ({email}) : ");
            email2 = Console.ReadLine();
            Console.Write($"Téléphone ({telephone}) : ");
            telephone2 = Console.ReadLine();
            Console.Write($"Voie ({voie}) : ");
            voie2 = Console.ReadLine();
            Console.Write($"Code Postal ({cp}) : ");
            cp2 = Console.ReadLine();
            Console.Write($"Ville ({ville}) : ");
            ville2 = Console.ReadLine();
            Console.Write($"Province ({province}) : ");
            province2 = Console.ReadLine();

            ModifierClient(idClient, nom: nom2, prenom: prenom2, email: email2, telephone: telephone2);
            if((voie2 != null && voie2.Length > 0) || (ville2 != null && ville2.Length > 0) || (cp2 != null && cp2.Length > 0) || (province2 != null && province2.Length > 0))
            {
                if (voie2.Length > 0)
                    voie = voie2;
                if (ville2.Length > 0)
                    ville = ville2;
                if (cp2.Length > 0)
                    cp = cp2;
                if (province2.Length > 0)
                    province = province2;
                ModifierAdresseClient(idClient, voie, ville, cp, province);
            }

            MenuClients();
        }

        static void MenuVoirCommande(int idCommande, int selected = 0)
        {
            Console.Clear();
            Console.WriteLine($"***** COMMANDE {idCommande} *****\n");

            string commande = GetCommande(idCommande);

            Console.WriteLine(commande);

            Console.WriteLine();

            string[] liens = new string[] { "Supprimer la commande", "Toutes les commandes", "Menu principal"};
            for (int i = 0; i < liens.Length; i++)
            {
                Console.WriteLine($"[{ ((i == selected) ? "*" : " ") }] {liens[i]}");
            }

            ConsoleKey r = Console.ReadKey().Key;
            if (r == ConsoleKey.DownArrow)
            {
                MenuVoirCommande(idCommande, selected == liens.Length - 1 ? selected : selected + 1);
            }
            else if (r == ConsoleKey.UpArrow)
            {
                MenuVoirCommande(idCommande, selected == 0 ? selected : selected - 1);
            } else if (r == ConsoleKey.Escape)
            {
                MenuCommandes();
            }
            else if (r == ConsoleKey.Enter)
            {
                // do smth
                if (selected == 0)
                {
                    SupprimerCommande(idCommande);
                    MenuCommandes();
                }
                else if (selected == 1) MenuCommandes();
                else if (selected == 2) MainMenu();
                else MenuVoirCommande(idCommande, selected);
            }

        }

        static void MenuCreerClient()
        {
            Console.Clear();
            Console.WriteLine("***** NOUVEAU CLIENT *****\n");

            string nom2;
            string prenom2;
            string email2;
            string telephone2;
            string voie2;
            string cp2;
            string ville2;
            string province2;

            Console.Write($"Nom : ");
            nom2 = Console.ReadLine();
            Console.Write($"Prénom : ");
            prenom2 = Console.ReadLine();
            Console.Write($"Email : ");
            email2 = Console.ReadLine();
            Console.Write($"Téléphone : ");
            telephone2 = Console.ReadLine();
            Console.Write($"Voie : ");
            voie2 = Console.ReadLine();
            Console.Write($"Code Postal : ");
            cp2 = Console.ReadLine();
            Console.Write($"Ville : ");
            ville2 = Console.ReadLine();
            Console.Write($"Province : ");
            province2 = Console.ReadLine();

            CreerClientParticulier(nom2, prenom2, email2, telephone2, voie2, ville2, cp2, province2);
            MenuClients();
        }

        static void MenuCommandes(int selected = 0)
        {
            Console.Clear();
            Console.WriteLine("***** COMMANDES *****\n");

            string[] commandes = GetCommandes();
            int[] idcommandes = new int[commandes.Length];
            for (int i = 0; i < commandes.Length; i++)
                idcommandes[i] = Convert.ToInt32(commandes[i].Split(' ')[0]);

            for (int i = 0; i < commandes.Length; i++)
            {
                //int id = Convert.ToInt32(client.Split(' ')[0]);
                Console.WriteLine($"[{ ((i == selected) ? "*" : " ") }] " + commandes[i]);
                if (i == commandes.Length - 1) Console.WriteLine($"[{ ((i + 1 == selected) ? "*" : " ") }] " + "Nouvelle commande");
            }

            if (commandes.Length == 0) Console.WriteLine("Aucune commande trouvée");

            ConsoleKey r = Console.ReadKey().Key;
            if (r == ConsoleKey.DownArrow)
            {
                MenuCommandes(selected == commandes.Length ? selected : selected + 1);
            }
            else if (r == ConsoleKey.UpArrow)
            {
                MenuCommandes(selected == 0 ? selected : selected - 1);
            }
            else if (r == ConsoleKey.Escape)
            {
                MainMenu();
            }
            else if (r == ConsoleKey.Enter)
            {
                // so smth
                if (selected < commandes.Length)
                {
                    // Modifier client
                    MenuVoirCommande(idcommandes[selected]);
                }
                else
                {
                    MenuCreerCommande();
                }
            }
            else
            {
                MenuCommandes(selected);
            }
        }

        static void MenuCreerCommande()
        {
            Console.Clear();
            Console.WriteLine("***** NOUVELLE COMMANDE *****\n");

            Console.Write("ID du client : ");
            int idClient = Convert.ToInt32(Console.ReadLine());
            Console.Write("ID Vélos (séparés par des espaces) : ");
            string saisieVelos = Console.ReadLine();
            Console.Write("ID Pièces (séparés par des espaces) : ");
            string saisiePieces = Console.ReadLine();

            string[] splitVelos = saisieVelos.Split(' ');
            string[] pieces = saisiePieces.Split(' ');
            int[] velos = new int[splitVelos.Length];
            for (int i = 0; i < splitVelos.Length; i++)
                if(saisieVelos.Length > 0)
                    velos[i] = Convert.ToInt32(splitVelos[i]);

            if (saisiePieces.Length == 0)
                pieces = new string[0];
            if (saisieVelos.Length == 0)
                velos = new int[0];

            (bool, string, string) resultat = CreerCommande(idClient, velos, pieces);

            if (resultat.Item3 != null)
            {
                Console.WriteLine("Une erreur est survenue lors de la création de la commande :");
                Console.WriteLine(resultat.Item3);
                Console.ReadKey();
                MenuCommandes();
            } else if(resultat.Item2 != null)
            {
                Console.WriteLine("Impossible de créer la commande. Pièces manquantes en stock :");
                Console.WriteLine(resultat.Item2);
                Console.ReadKey();
                MenuCommandes();
            }
            else
            {
                MenuCommandes();
            }
        }

        static void MenuVoirVelos(int selected = 0)
        {
            Console.Clear();
            Console.WriteLine("***** VELOS *****\n");

            string[] velos = GetVelos();
            int[] idvelos = new int[velos.Length];
            for (int i = 0; i < velos.Length; i++)
                idvelos[i] = Convert.ToInt32(velos[i].Split(' ')[0]);

            for (int i = 0; i < velos.Length; i++)
            {
                Console.WriteLine($"[{ ((i == selected) ? "*" : " ") }] " + velos[i]);
                //if (i == velos.Length - 1) Console.WriteLine($"[{ ((i + 1 == selected) ? "*" : " ") }] " + "Nouveau vélo");
            }

            if (velos.Length == 0) Console.WriteLine("Aucun vélo trouvé");

            ConsoleKey r = Console.ReadKey().Key;
            if (r == ConsoleKey.DownArrow)
            {
                MenuVoirVelos(selected == velos.Length - 1 ? selected : selected + 1);
            }
            else if (r == ConsoleKey.UpArrow)
            {
                MenuVoirVelos(selected == 0 ? selected : selected - 1);
            }
            else if (r == ConsoleKey.Escape)
            {
                MainMenu();
            }
            else if (r == ConsoleKey.Enter)
            {
                if (selected < velos.Length)
                {
                    // Modifier velo
                    MenuModifierVelo(idvelos[selected]);
                }
                else
                {
                    MenuCreerCommande();
                }
            }
            else
            {
                MenuVoirVelos(selected);
            }
        }

        static void MenuModifierVelo(int idVelo, int selected = 0)
        {

            Console.Clear();
            Console.WriteLine($"***** COMMANDE {idVelo} *****\n");

            string velo = GetVelo(idVelo);

            Console.WriteLine(velo);

            Console.WriteLine();

            string[] liens = new string[] { "Supprimer le vélo", "Tous les vélos", "Menu principal" };
            for (int i = 0; i < liens.Length; i++)
            {
                Console.WriteLine($"[{ ((i == selected) ? "*" : " ") }] {liens[i]}");
            }

            ConsoleKey r = Console.ReadKey().Key;
            if (r == ConsoleKey.DownArrow)
            {
                MenuModifierVelo(idVelo, selected == liens.Length - 1 ? selected : selected + 1);
            }
            else if (r == ConsoleKey.UpArrow)
            {
                MenuModifierVelo(idVelo, selected == 0 ? selected : selected - 1);
            }
            else if (r == ConsoleKey.Escape)
            {
                MenuVoirVelos();
            }
            else if (r == ConsoleKey.Enter)
            {
                // do smth
                if (selected == 0)
                {
                    //
                    SupprimerBicyclette(idVelo);
                    MenuVoirVelos();
                }
                else if (selected == 1) MenuVoirVelos();
                else if (selected == 2) MainMenu();
                else MenuModifierVelo(idVelo, selected);
            }



        }

        static void MenuPieces()
        {
            Console.Clear();
            Console.WriteLine("***** PIÈCES *****\n");

            string[] pieces = GetPieces();
            for (int i = 0; i < pieces.Length; i++)
            {
                Console.WriteLine(pieces[i]);
            }

            Console.WriteLine("Appuyez sur une touche pour retourner au menu principal.");
            Console.ReadKey();
            MainMenu();
        }

        static int CreerAdresse(string voie, string ville, string cp, string province)
        {
            string req = "INSERT INTO adresse(id, voie, ville, code_postal, province) VALUES (NULL, @voie, @ville, @cp, @province)";
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = req;
            command.Parameters.AddWithValue("@voie", voie);
            command.Parameters.AddWithValue("@ville", ville);
            command.Parameters.AddWithValue("@cp", cp);
            command.Parameters.AddWithValue("@province", province);
            try
            {
                command.ExecuteNonQuery();
                // Adresse crée
            }
            catch
            {
                //Console.WriteLine("Une erreur est survenue");
                return -1; // adresse non créée
            }

            // On récupère l'ID de l'adresse crée
            try
            {
                MySqlCommand command2 = connexion.CreateCommand();
                command2.CommandText = "SELECT LAST_INSERT_ID();";
                int result = Convert.ToInt32(command2.ExecuteScalar());
                return result;
            }
            catch
            {
                return -1;
            }
        }

        static bool SupprimerAdresse(int id)
        {
            string query = "DELETE FROM adresse WHERE id = @id";
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@id", id);
            try
            {
                int i = command.ExecuteNonQuery();
                return (i == 1);
            } catch
            {
                return false;
            }
        }

        static bool CreerFournisseur(string siret, string nom, int libelle, string nom_contact, string prenom_contact, int IDadresse)
        {
            string req = "INSERT INTO fournisseur(siret, nom, adresse, libelle, nom_contact, prenom_contact) VALUES (@siret, @nom, @adresse, @libelle, @nomc, @prenomc)";
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = req;
            command.Parameters.AddWithValue("@siret", siret);
            command.Parameters.AddWithValue("@nom", nom);
            command.Parameters.AddWithValue("@adresse", IDadresse);
            command.Parameters.AddWithValue("@libelle", libelle);
            command.Parameters.AddWithValue("@nomc", nom_contact);
            command.Parameters.AddWithValue("@prenomc", prenom_contact);
            try
            {
                int count = command.ExecuteNonQuery();
                return (count == 1);
            }
            catch
            {
                return false;
            }
        }

        static bool CreerFournisseur(string siret, string nom, int libelle, string nom_contact, string prenom_contact, string voie, string ville, string cp, string province)
        {
            // Si une erreur survient lors de la création de l'adresse on créé pas de fournisseur
            int IDadresse = CreerAdresse(voie, ville, cp, province);
            if (IDadresse < 0) return false;

            string req = "INSERT INTO fournisseur(siret, nom, adresse, libelle, nom_contact, prenom_contact) VALUES (@siret, @nom, @adresse, @libelle, @nomc, @prenomc)";
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = req;
            command.Parameters.AddWithValue("@siret", siret);
            command.Parameters.AddWithValue("@nom", nom);
            command.Parameters.AddWithValue("@adresse", IDadresse);
            command.Parameters.AddWithValue("@libelle", libelle);
            command.Parameters.AddWithValue("@nomc", nom_contact);
            command.Parameters.AddWithValue("@prenomc", prenom_contact);
            try
            {
                int count = command.ExecuteNonQuery();
                return (count == 1);
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Une erreur est survenue");
                return false;
            }

        }

        static bool SupprimerFournisseur(string siret)
        {
            string requete = "DELETE FROM fournisseur WHERE siret = @siret;";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = requete;
            commande.Parameters.AddWithValue("@siret", siret);
            try
            {
                int rowsAffected = commande.ExecuteNonQuery();
                return rowsAffected == 1;
            }
            catch
            {
                return false;
            }
        }

        static bool SupprimerPiece(string id)
        {
            string requete = "DELETE FROM piece WHERE id = @id;";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = requete;
            commande.Parameters.AddWithValue("@id", id);
            try
            {
                int rowsAffected = commande.ExecuteNonQuery();
                return rowsAffected == 1;
            }
            catch
            {
                return false;
            }
        }

        static string CreerPiece(string id, string description, string fournisseur, int numCatalogue, float prix, DateTime introduction, DateTime discontinuation, int delaiApprovisionnement)
        {
            // SELECT COUNT(siret) FROM fournisseur WHERE siret = "30613890001294"; on vérifie que le fournisseur existe

            string chReq = "SELECT COUNT(siret) FROM fournisseur WHERE siret = @siret";
            MySqlCommand selectCommand = connexion.CreateCommand();
            selectCommand.CommandText = chReq;
            selectCommand.Parameters.AddWithValue("@siret", fournisseur);
            int count;
            try
            {
                count = Convert.ToInt32(selectCommand.ExecuteScalar());
                //Console.WriteLine($"count : {count}");
            }
            catch (Exception)
            {
                Console.WriteLine("Une erreur est survenue");
                count = 0;
            }

            // On vérifie que le fournisseur existe
            if (count == 1)
            {
                MySqlCommand insertCommand = connexion.CreateCommand();
                insertCommand.CommandText = "INSERT INTO piece(id, details, fournisseur, numcatalogue, prix, date_introduction, date_discontinuation, delai_approvisionnement) VALUES (@id, @details, @fournisseur, @numcatalogue, @prix, @introduction, @discontinuation, @delai); ";
                insertCommand.Parameters.AddWithValue("@id", id);
                insertCommand.Parameters.AddWithValue("@details", description);
                insertCommand.Parameters.AddWithValue("@fournisseur", fournisseur);
                insertCommand.Parameters.AddWithValue("@numcatalogue", numCatalogue);
                insertCommand.Parameters.AddWithValue("@prix", prix);
                insertCommand.Parameters.AddWithValue("@introduction", introduction.ToString("u").Substring(0, 10));
                insertCommand.Parameters.AddWithValue("@discontinuation", discontinuation.ToString("u").Substring(0, 10));
                insertCommand.Parameters.AddWithValue("@delai", delaiApprovisionnement);
                try
                {
                    insertCommand.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Une erreur est survenue");
                    
                }
                return "";
            }
            else
            {
                return "";
            }

        }

        static void ModifierPiece(string id, string newId = null, string details = null, string fournisseur = null, int numCatalogue = -1, int prix = -1, DateTime introduction = default, DateTime discontinuation = default, int delai = -1)
        {
            string query = "UPDATE piece SET ";
            List<string> parametres = new List<string>();
            if (newId != null)
                parametres.Add("id = @newId");
            if (details != null)
                parametres.Add("details = @details");
            if (fournisseur != null)
                parametres.Add("fournisseur = @fournisseur");
            if (numCatalogue >= 0)
                parametres.Add("numcatalogue = @numcatalogue");
            if (prix >= 0)
                parametres.Add("prix = @prix");
            if (introduction != default)
                parametres.Add("date_introduction = @introduction");
            if (discontinuation != default)
                parametres.Add("date_discontinuation = @discontinuation");
            if (delai >= 0)
                parametres.Add("delai_approvisionnement = @delai");

            if (parametres.Count == 0)
            {
                return;
            }

            query += String.Join(", ", parametres);
            query += " WHERE id = @id";

            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;

            if (newId != null)
                commande.Parameters.AddWithValue("@newId", newId);
            if (details != null)
                commande.Parameters.AddWithValue("@details", details);
            if (fournisseur != null)
                commande.Parameters.AddWithValue("@fournisseur", fournisseur);
            if (numCatalogue >= 0)
                commande.Parameters.AddWithValue("@numcatalogue", numCatalogue);
            if (prix >= 0)
                commande.Parameters.AddWithValue("@prix", prix);
            if (introduction != default)
                commande.Parameters.AddWithValue("@introduction", introduction.ToString("u").Substring(0, 10));
            if (discontinuation != default)
                commande.Parameters.AddWithValue("@discontinuation", discontinuation.ToString("u").Substring(0, 10));
            if (delai >= 0)
                commande.Parameters.AddWithValue("@delai", delai);

            /*Console.WriteLine(commande.CommandText);
            for (int i = 0; i < commande.Parameters.Count; i++)
            {
                Console.WriteLine($"{commande.Parameters[i]}");
            }*/

            commande.Parameters.AddWithValue("@id", id);

            try
            {
                commande.ExecuteNonQuery();
                //Console.WriteLine("piece mise a jour");
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
            }

        }

        static bool CreerBicyclette(int id, int idAssemblage, string nom, int grandeur, float prix, string ligneProduit)
        {
            string query = "INSERT INTO bicyclette(id, id_assemblage, nom, grandeur, prix, ligne_produit) VALUES (@id, @assemblage, @nom, @grandeur, @prix, @ligne)";
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@assemblage", idAssemblage);
            command.Parameters.AddWithValue("@nom", nom);
            command.Parameters.AddWithValue("@grandeur", grandeur);
            command.Parameters.AddWithValue("@prix", prix);
            command.Parameters.AddWithValue("@ligne", ligneProduit);

            try {
                int i = command.ExecuteNonQuery();
                return i == 1;
            } catch (Exception e)
            {
                return false;
            }
        }

        static bool SupprimerBicyclette(int id)
        {
            string requete = "DELETE FROM bicyclette WHERE id = @id;";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = requete;
            commande.Parameters.AddWithValue("@id", id);
            try
            {
                int rowsAffected = commande.ExecuteNonQuery();
                return rowsAffected == 1;
            }
            catch
            {
                return false;
            }
        }

        static void ModifierBicyclette(int id, int newId = -1, int assemblage = -1, string nom = null, int grandeur = -1, float prix = -1, string ligneProduit = null)
        {
            string query = "UPDATE bicyclette SET ";
            List<string> parametres = new List<string>();
            if (newId >= 0)
                parametres.Add("id = @newId");
            if (assemblage >= 0)
                parametres.Add("id_assemblage = @assemblage");
            if (grandeur >= 0)
                parametres.Add("grandeur = @grandeur");
            if (prix >= 0)
                parametres.Add("prix = @prix");
            if (nom != null)
                parametres.Add("nom = @nom");
            if (ligneProduit != null)
                parametres.Add("ligne_produit = @ligne");

            if (parametres.Count == 0)
            {
                return;
            }

            query += String.Join(", ", parametres);
            query += " WHERE id = @id";

            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;

            if (newId >= 0)
                commande.Parameters.AddWithValue("@newId", newId);
            if (assemblage >= 0)
                commande.Parameters.AddWithValue("@assemblage", assemblage);
            if (grandeur >= 0)
                commande.Parameters.AddWithValue("@grandeur", grandeur);
            if (prix >= 0)
                commande.Parameters.AddWithValue("@prix", prix);
            if (nom != null)
                commande.Parameters.AddWithValue("@nom", nom);
            if (ligneProduit != null)
                commande.Parameters.AddWithValue("@ligne", ligneProduit);

            /*Console.WriteLine(commande.CommandText);
            for (int i = 0; i < commande.Parameters.Count; i++)
            {
                Console.WriteLine($"{commande.Parameters[i]}");
            }*/

            commande.Parameters.AddWithValue("@id", id);

            try
            {
                commande.ExecuteNonQuery();
                //Console.WriteLine("piece mise a jour");
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
            }
        }

        static bool CreerAssemblage(int id, string cadre = null, string guidon = null, string freins = null, string selle = null, string derailleur_avant = null, string derailleur_arriere = null, string roue_avant = null, string roue_arriere = null, string reflecteurs = null, string pedalier = null, string ordinateur = null, string panier = null)
        {
            string query = "INSERT INTO assemblage (id, cadre, guidon, freins, selle, derailleur_avant, derailleur_arriere, roue_avant, roue_arriere, reflecteurs, pedalier, ordinateur, panier) VALUES (@id, @cadre, @guidon, @freins, @selle, @da, @dv, @ra, @rv, @reflecteurs, @pedalier, @ordinateur, @panier);";

            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = query;

            command.Parameters.AddWithValue("@id", id);
            command.Parameters.AddWithValue("@cadre", cadre);
            command.Parameters.AddWithValue("@guidon", guidon);
            command.Parameters.AddWithValue("@freins", freins);
            command.Parameters.AddWithValue("@selle", selle);
            command.Parameters.AddWithValue("@da", derailleur_avant);
            command.Parameters.AddWithValue("@dv", derailleur_arriere);
            command.Parameters.AddWithValue("@ra", roue_avant);
            command.Parameters.AddWithValue("@rv", roue_arriere);
            command.Parameters.AddWithValue("@reflecteurs", reflecteurs);
            command.Parameters.AddWithValue("@pedalier", pedalier);
            command.Parameters.AddWithValue("@ordinateur", ordinateur);
            command.Parameters.AddWithValue("@panier", panier);

            try
            {
                int c = command.ExecuteNonQuery();
                return c == 1;
            } catch (Exception e) {
                //Console.WriteLine(e.Message);
                return false;

            }

        }

        static int CreerClientParticulier(string nom, string prenom, string email, string telephone, string voie, string ville, string cp, string province)
        {
            int IDadresse = CreerAdresse(voie, ville, cp, province);
            if (IDadresse < 0) return -1;

            string query = "INSERT INTO client(nom, prenom, email, telephone, adresse) VALUES (@nom, @prenom, @email, @telephone, @adresse)";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            commande.Parameters.AddWithValue("@nom", nom);
            commande.Parameters.AddWithValue("@prenom", prenom);
            commande.Parameters.AddWithValue("@email", email);
            commande.Parameters.AddWithValue("@telephone", telephone);
            commande.Parameters.AddWithValue("@adresse", IDadresse);

            try {
                commande.ExecuteNonQuery();
            } catch
            {
                return -1;
            }

            try
            {
                MySqlCommand command2 = connexion.CreateCommand();
                command2.CommandText = "SELECT LAST_INSERT_ID();";
                int result = Convert.ToInt32(command2.ExecuteScalar());
                return result;
            }
            catch
            {
                return -1;
            }


        }

        static int CreerClientCompagnie(string nom, string prenom, string email, string telephone, string voie, string ville, string cp, string province, string nomCompagnie, int remise = 0)
        {
            int IDadresse = CreerAdresse(voie, ville, cp, province);
            if (IDadresse < 0) return -1;

            string query = "INSERT INTO compagnie(nom, remise) VALUES (@nom, @remise);";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;

            commande.Parameters.AddWithValue("@nom", nomCompagnie);
            commande.Parameters.AddWithValue("@remise", remise);

            try
            {
                int c = commande.ExecuteNonQuery();
                if (c != 1) return -1;
            }
            catch
            {
                return -1;
            }

            int compagnieID = -1;
            try
            {
                MySqlCommand command2 = connexion.CreateCommand();
                command2.CommandText = "SELECT LAST_INSERT_ID();";
                compagnieID = Convert.ToInt32(command2.ExecuteScalar());
            }
            catch
            {
                return -1;
            }
            if (compagnieID < 0) return -1;

            string query2 = "INSERT INTO client(nom, prenom, email, id_compagnie, telephone, adresse) VALUES (@nom, @prenom, @email, @compagnie, @telephone, @adresse);";
            MySqlCommand commande2 = connexion.CreateCommand();
            commande2.CommandText = query2;
            commande2.Parameters.AddWithValue("@nom", nom);
            commande2.Parameters.AddWithValue("@prenom", prenom);
            commande2.Parameters.AddWithValue("@email", email);
            commande2.Parameters.AddWithValue("@compagnie", compagnieID);
            commande2.Parameters.AddWithValue("@telephone", telephone);
            commande2.Parameters.AddWithValue("@adresse", IDadresse);

            try
            {
                int c = commande2.ExecuteNonQuery();
                if (c != 1) return -1;
            } catch {
                return -1;
            }

            int clientID = -1;
            try
            {
                MySqlCommand command2 = connexion.CreateCommand();
                command2.CommandText = "SELECT LAST_INSERT_ID();";
                clientID = Convert.ToInt32(command2.ExecuteScalar());
                return clientID >= 0 ? clientID : -1;
            }
            catch
            {
                return -1;
            }

        }

        static bool ModifierClient(int idClient, string nom = null, string prenom = null, string email = null, string telephone = null)
        {
            bool log = true;
            string query = "UPDATE client SET ";
            List<string> parametres = new List<string>();
            if (nom != null && nom.Length > 0)
                parametres.Add("nom = @nom");
            if (prenom != null && prenom.Length > 0)
                parametres.Add("prenom = @prenom");
            if (email != null && email.Length >0)
                parametres.Add("email = @email");
            if (telephone != null && telephone.Length > 0)
                parametres.Add("telephone = @telephone");

            if (parametres.Count == 0)
            {
                if(log) Console.WriteLine("Count 0");
                return false;
            }

            query += String.Join(", ", parametres);
            query += " WHERE id_client = @id";

            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;

            if (nom != null)
                commande.Parameters.AddWithValue("@nom", nom);
            if (prenom != null)
                commande.Parameters.AddWithValue("@prenom", prenom);
            if (email != null)
                commande.Parameters.AddWithValue("@email", email);
            if (telephone != null)
                commande.Parameters.AddWithValue("@telephone", telephone);

            commande.Parameters.AddWithValue("@id", idClient);

            try
            {
                int c = commande.ExecuteNonQuery();

                return (c == 1);

            }
            catch (Exception e)
            {
                if (log) Console.WriteLine(e.Message);
                return false;
            }
        }

        static bool ModifierAdresseClient(int idClient, string voie, string ville, string cp, string province)
        {
            int idAdresse = CreerAdresse(voie, ville, cp, province);
            if (idAdresse < 0) return false;

            string query = "UPDATE CLIENT SET adresse = @adresse WHERE id_client = @id";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            commande.Parameters.AddWithValue("@adresse", idAdresse);
            commande.Parameters.AddWithValue("@id", idClient);

            try
            {
                int c = commande.ExecuteNonQuery();
                return (c == 1);
            }
            catch
            {
                return false;
            }
        }

        static bool SupprimerClient(int idClient)
        {
            string query = "DELETE FROM client WHERE id_client = @id";
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@id", idClient);
            try
            {
                int i = command.ExecuteNonQuery();
                return (i == 1);
            }
            catch
            {
                return false;
            }
        }

        static int AdresseClient(int idClient)
        {
            string query = "SELECT adresse FROM client WHERE id_client = @id";
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = query;
            command.Parameters.AddWithValue("@id", idClient);

            try {
                int adresse = Convert.ToInt32(command.ExecuteScalar());
                return adresse;
            }
            catch(Exception e)
            {
                //Console.WriteLine(e.Message);
                return -1;
            }
        }

        static (bool, string) VerificationStockVelo(List<int> idVelos, List<int> quVelos)
        {
            string query = $"SELECT id, stock FROM bicyclette WHERE id IN ({String.Join(',', idVelos)});";
            List<int> manque = new List<int>();
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;


            try
            {
                int count = 0;
                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int stock = reader.GetInt32(1);
                    int index = idVelos.IndexOf(id);
                    if (quVelos[index] > stock)
                    {
                        for (int i = 0; i < quVelos[index] - stock; i++)
                            manque.Add(id);
                    }
                    count++;
                }
                reader.Close();
                if (count != idVelos.Count) return (false, "Certains vélos n'existent pas.");
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
            if (manque.Count > 0)
                return (true, String.Join(' ', manque));
            return (true, "");

        }

        static (bool, string) VerificationStocks(List<string> idPieces, List<int> quPieces)
        {
            // Vérifiction du stock pour chaque pièce commandée
            string query = $"SELECT id, stock FROM `piece` WHERE id IN (\"" + String.Join("\",\"", idPieces) + "\");";
            List<string> manque = new List<string>();
            MySqlCommand command2 = connexion.CreateCommand();
            command2.CommandText = query;
            try
            {
                int count = 0;
                MySqlDataReader reader = command2.ExecuteReader();
                while (reader.Read())
                {
                    string id = reader.GetString(0);
                    int stock = reader.GetInt32(1);
                    int index = idPieces.IndexOf(id);
                    if (quPieces[index] > stock)
                    {
                        for (int i = 0; i < quPieces[index] - stock; i++)
                            manque.Add(id);
                    }
                    count++;
                }
                reader.Close();
                if (count != idPieces.Count) return (false, "Certaines pièces n'existent pas.");
            }
            catch (Exception e)
            {
                return (false, e.Message);
            }
            if (manque.Count > 0)
                return (true, String.Join(' ', manque));
            return (true, "");
        }

        static bool AjouterAchatVelo(int idCommande, int idVelo)
        {
            string query = "INSERT INTO achat(id_commande, id_bicyclette) VALUES (@commande, @velo);";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            commande.Parameters.AddWithValue("@commande", idCommande);
            commande.Parameters.AddWithValue("@velo", idVelo);
            int c = 0;
            try
            {
                c = commande.ExecuteNonQuery();
                return c == 1;
            } catch
            {
                return false;
            }

        }

        static bool AjouterAchatPiece(int idCommande, string idPiece)
        {
            string query = "INSERT INTO achat(id_commande, id_piece) VALUES (@commande, @piece);";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            commande.Parameters.AddWithValue("@commande", idCommande);
            commande.Parameters.AddWithValue("@piece", idPiece);
            int c = 0;
            try
            {
                c = commande.ExecuteNonQuery();
                return c == 1;
            }
            catch
            {
                return false;
            }

        }

        static List<string> GetAssemblage(int idVelo)
        {
            string query = "SELECT assemblage.* FROM bicyclette LEFT JOIN assemblage ON bicyclette.id_assemblage = assemblage.id WHERE bicyclette.id = @id";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            commande.Parameters.AddWithValue("@id", idVelo);
            List<string> pieces = new List<string>();
            try
            {
                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    for(int i = 1; i <= 12; i++)
                    {
                        if (!reader.IsDBNull(i))
                        {
                            pieces.Add(reader.GetString(i));
                        }  
                    }
                }
                reader.Close();
            } catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                return null;
            }

            return pieces;
        }

        static (bool, string, string) CreerCommande(int clientId, int[] velos, string[] pieces)
        {
            // On vérifie les vélos en stock

            List<int> idVelos = new List<int>();
            List<int> quVelos = new List<int>();
            for (int i = 0; i < velos.Length; i++)
            {
                int id = velos[i];
                // Déjà dans la liste
                if (idVelos.IndexOf(id) >= 0)
                {
                    quVelos[idVelos.IndexOf(id)] += 1;
                }
                else
                {
                    idVelos.Add(id);
                    quVelos.Add(1);
                }
            }

            string[] velosManquants = new string[0];
            if (velos.Length > 0)
            {
                // Si des vélos ne sont pas en stock on ajoute les pièces requises
                (bool, string) stockVelo = VerificationStockVelo(idVelos, quVelos);
                if (!stockVelo.Item1)
                {
                    return (false, null, "Erreur lors de la vérification du stock des vélos");
                }

                velosManquants = stockVelo.Item2.Length > 0 ? stockVelo.Item2.Split(' ') : new string[0];
            }

            List<string> piecesVelosManquants = new List<string>();
            for(int i =0; i < velosManquants.Length; i++)
            {
                //Console.WriteLine(velosManquants[i]);
                int idv = Convert.ToInt32(velosManquants[i]);
                piecesVelosManquants.AddRange(GetAssemblage(idv));
            }


            List<string> liste = new List<string>();
            liste.AddRange(pieces);
            liste.AddRange(piecesVelosManquants);

            //Console.WriteLine(ToString(liste));

            List<string> idPieces = new List<string>();
            List<int> quPieces = new List<int>();
            for (int i = 0; i < liste.Count; i++)
            {
                string id = liste[i];
                // Déjà dans la liste
                if (idPieces.IndexOf(id) >= 0)
                {
                    quPieces[idPieces.IndexOf(id)] += 1;
                }
                else
                {
                    idPieces.Add(id);
                    quPieces.Add(1);
                }
            }

            //Console.WriteLine(ToString(idPieces));
            //Console.WriteLine(ToString(quPieces));

            (bool, string) stock = VerificationStocks(idPieces, quPieces);
            if (!stock.Item1)
                return (false, null, "Erreur lors de la vérification du stock des pièces");
            if(stock.Item2.Length > 0)
                return (false, stock.Item2, null);

            // Si le stock est OK :

            int adresseClient = AdresseClient(clientId);
            if (adresseClient <= 0)
                return (false, null, "Adresse ou client introuvable");

            string req = "INSERT INTO commande(date_commande, id_client, adresse, remise) VALUES(@date, @client, @adresse, @remise);";
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = req;
            command.Parameters.AddWithValue("@date", DateTime.Now.ToString("u").Substring(0, 10));
            command.Parameters.AddWithValue("@client", clientId);
            command.Parameters.AddWithValue("@adresse", adresseClient);
            command.Parameters.AddWithValue("@remise", GetRabais(clientId));

            try
            {
                int count = command.ExecuteNonQuery();
                if (count != 1) return (false, null, "#003: Une erreur est survenue");
            }
            catch (Exception e)
            {
                return (false, null, "#001: " + e.Message);
            }

            // On récupère l'ID de la commande créée
            int IDCommande = -1;
            try
            {
                MySqlCommand command2 = connexion.CreateCommand();
                command2.CommandText = "SELECT LAST_INSERT_ID();";
                IDCommande = Convert.ToInt32(command2.ExecuteScalar());
            }
            catch (Exception e)
            {
                return (false, null, "#002: " + e.Message);
            }

            if (IDCommande <= 0)
                return (false, null,"#004: Impossible de créer la commande");

            // Maintenant on décrémente le stock de chaque pièce commandée seule
            // Et on ajoute à la commande le vélo OU la pièce

            // Pieces seules
            for(int i = 0; i < pieces.Length; i++)
            {
                DecrementerStockPiece(pieces[i]);
            }
            // Velos en stock
            // On fabrique les vélos manquants
            for(int i = 0; i < velosManquants.Length; i++)
            {
                int idv = Convert.ToInt32(velosManquants[i]);
                // stock piece decroit
                // stock velo up
                FabriquerVelo(idv);
                Console.WriteLine($"Le vélo {idv} doit être assemblé.");
            }
            // On réduit le stock de 1 pour chaque vélo
            for(int i = 0; i < velos.Length; i++)
            {
                DecrementerStockVelo(velos[i]);
            }


            for(int i = 0; i < velos.Length; i++)
            {
                AjouterAchatVelo(IDCommande, velos[i]);
            }

            for(int i = 0; i < pieces.Length; i++)
            {
                AjouterAchatPiece(IDCommande, pieces[i]);
            }

            return (true, null, null);

        }

        static void FabriquerVelo(int idVelo)
        {
            List<string> pieces = GetAssemblage(idVelo);
            for (int i = 0; i < pieces.Count; i++)
                DecrementerStockPiece(pieces[i]);
            IncrementerStockVelo(idVelo);
        }

        static List<int> GetVelos(int idCommande)
        {
            string query = "SELECT id_bicyclette FROM achat WHERE id_bicyclette IS NOT NULL AND id_commande = @commande";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            commande.Parameters.AddWithValue("@commande", idCommande);
            List<int> velos = new List<int>();
            try
            {
                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    velos.Add(reader.GetInt32(0));
                }
                reader.Close();
                return velos;
            }
            catch
            {
                return null;
            }
        }

        static List<string> GetPieces(int idCommande)
        {
            string query = "SELECT id_piece FROM achat WHERE id_commande = @commande AND id_piece IS NOT NULL";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            commande.Parameters.AddWithValue("@commande", idCommande);
            List<string> pieces = new List<string>();
            try
            {
                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    pieces.Add(reader.GetString(0));
                }
                reader.Close();
                return pieces;
            }
            catch
            {
                return null;
            }
        }

        static bool IncrementerStockPiece(string idPiece)
        {
            string query2 = "UPDATE piece SET stock = stock + 1 WHERE id = @piece;";
            MySqlCommand commande2 = connexion.CreateCommand();
            commande2.CommandText = query2;
            commande2.Parameters.AddWithValue("@piece", idPiece);
            int c = -1;
            try
            {
                c = commande2.ExecuteNonQuery();
                return (c == 1);
            }
            catch
            {
                return false;
            }
        }

        static bool IncrementerStockVelo(int idVelo)
        {
            string query2 = "UPDATE bicyclette SET stock = stock + 1 WHERE id = @piece;";
            MySqlCommand commande2 = connexion.CreateCommand();
            commande2.CommandText = query2;
            commande2.Parameters.AddWithValue("@piece", idVelo);
            int c = -1;
            try
            {
                c = commande2.ExecuteNonQuery();
                return (c == 1);
            }
            catch
            {
                return false;
            }
        }

        static bool DecrementerStockPiece(string idPiece)
        {
            string query2 = "UPDATE piece SET stock = stock - 1 WHERE id = @piece;";
            MySqlCommand commande2 = connexion.CreateCommand();
            commande2.CommandText = query2;
            commande2.Parameters.AddWithValue("@piece", idPiece);
            int c = -1;
            try
            {
                c = commande2.ExecuteNonQuery();
                return (c == 1);
            }
            catch
            {
                return false;
            }
        }

        static bool DecrementerStockVelo(int idVelo)
        {
            string query2 = "UPDATE bicyclette SET stock = stock - 1 WHERE id = @piece;";
            MySqlCommand commande2 = connexion.CreateCommand();
            commande2.CommandText = query2;
            commande2.Parameters.AddWithValue("@piece", idVelo);
            int c = -1;
            try
            {
                c = commande2.ExecuteNonQuery();
                return (c == 1);
            }
            catch
            {
                return false;
            }
        }

        static bool SupprimerCommande(int idCommande)
        {
            List<int> velos = GetVelos(idCommande);
            List<string> pieces = GetPieces(idCommande);

            for (int i = 0; i < velos.Count; i++)
            {
                IncrementerStockVelo(velos[i]);
            }

            for (int i = 0; i < pieces.Count; i++)
            {
                IncrementerStockPiece(pieces[i]);
            }


            // Suppression des achats
            string query = "DELETE FROM achat WHERE id_commande = @commande";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            commande.Parameters.AddWithValue("@commande", idCommande);

            try
            {
                int c = commande.ExecuteNonQuery();
                //Console.WriteLine(c == velos.Count + pieces.Count); // this should be true every time
            }
            catch
            {
                return false;
            }

            string query2 = "DELETE FROM commande WHERE id = @commande";
            MySqlCommand commande2 = connexion.CreateCommand();
            commande2.CommandText = query2;
            commande2.Parameters.AddWithValue("@commande", idCommande);

            try
            {
                int c = commande2.ExecuteNonQuery();
                return (c == 1);
            }
            catch
            {
                return false;
            }

            

        }

        // stocks

        static Dictionary<string, int> StocksPieces()
        {
            Dictionary<string, int> stocks = new Dictionary<string, int>();
            string query = "SELECT id, stock FROM piece;";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            try
            {
                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    stocks.Add(reader.GetString(0), reader.GetInt32(1));
                }
                reader.Close();

                return stocks;
            } catch
            {
                return null;
            }
        }

        static Dictionary<int, int> StocksVelos()
        {
            Dictionary<int, int> stocks = new Dictionary<int, int>();
            string query = "SELECT id, stock FROM bicyclette;";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            try
            {
                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    stocks.Add(reader.GetInt32(0), reader.GetInt32(1));
                }
                reader.Close();

                return stocks;
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                return null;
            }
        }

        static Dictionary<string, Dictionary<string, int>> StocksParFournisseur() {
            Dictionary<string, Dictionary<string, int>> stocks = new Dictionary<string, Dictionary<string, int>>();
            string query = "SELECT fournisseur.nom, piece.id, piece.stock FROM fournisseur RIGHT JOIN piece ON piece.fournisseur = fournisseur.siret;";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            try
            {
                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    string siret = reader.GetString(0);
                    string piece = reader.GetString(1);
                    int stock = reader.GetInt32(2);

                    if (stocks.ContainsKey(siret))
                    {
                        stocks[siret].Add(piece, stock);
                    } else
                    {
                        stocks.Add(siret, new Dictionary<string, int>());
                        stocks[siret].Add(piece, stock);
                    }
                }
                reader.Close();

                return stocks;
            } catch
            {
                return null;
            }
        }

        // affichages

        static string[] GetClient(int idClient)
        {
            string query = "SELECT client.id_client, client.nom, client.prenom, client.email, client.telephone, adresse.voie, adresse.code_postal, adresse.ville, adresse.province FROM client LEFT JOIN adresse ON client.adresse = adresse.id WHERE client.id_compagnie IS NULL AND id_client = @client;";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            commande.Parameters.AddWithValue("@client", idClient);
            try
            {
                List<string> liste = new List<string>();

                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string nom = reader.GetString(1);
                    string prenom = reader.GetString(2);
                    string email = reader.GetString(3);
                    string telephone = reader.GetString(4);
                    string voie = reader.GetString(5);
                    string cp = reader.GetString(6);
                    string ville = reader.GetString(7);
                    string province = reader.GetString(8);

                    liste.Add(nom);
                    liste.Add(prenom);
                    liste.Add(email);
                    liste.Add(telephone);
                    liste.Add(voie);
                    liste.Add(cp);
                    liste.Add(ville);
                    liste.Add(province);
                }
                reader.Close();

                return liste.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new string[0];
            }
        }

        static string[] GetClientsParticuliers()
        {
            string query = "SELECT client.id_client, client.nom, client.prenom, client.email, client.telephone, adresse.voie, adresse.code_postal, adresse.ville, adresse.province FROM client LEFT JOIN adresse ON client.adresse = adresse.id";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;

            try
            {
                List<string> liste = new List<string>();

                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    int idClient = reader.GetInt32(0);
                    string nom = reader.GetString(1);
                    string prenom = reader.GetString(2);
                    string email = reader.GetString(3);
                    string telephone = reader.GetString(4);
                    string voie = reader.GetString(5);
                    string cp = reader.GetString(6);
                    string ville = reader.GetString(7);
                    string province = reader.GetString(8);

                    liste.Add($"{idClient} {nom.ToUpper()} {prenom} | {email} | {telephone} | {voie}, {cp} {ville} {province}");
                }
                reader.Close();

                return liste.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new string[0];
            }
        }

        static string[] GetCommandes()
        {
            string query = "SELECT commande.id, commande.date_commande, client.nom, client.prenom FROM commande LEFT JOIN client ON commande.id_client = client.id_client;";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;

            try
            {
                List<string> liste = new List<string>();

                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    int idCommande = reader.GetInt32(0);
                    string date = reader.GetString(1);
                    string nom = reader.GetString(2);
                    string prenom = reader.GetString(3);

                    liste.Add($"{idCommande} | {nom.ToUpper()} {prenom} | {date}");
                }
                reader.Close();

                return liste.ToArray();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return new string[0];
            }
        }

        static string GetCommande(int idCommande)
        {
            string str = "";
            string query = "SELECT commande.id, commande.date_commande, client.nom, client.prenom FROM commande LEFT JOIN client ON commande.id_client = client.id_client WHERE commande.id = @commande;";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            commande.Parameters.AddWithValue("@commande", idCommande);
            
            try
            {
                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    string date = reader.GetString(1);
                    string nom = reader.GetString(2);
                    string prenom = reader.GetString(3);
                    str += $"{id} | {nom.ToUpper()} {prenom} | {date}";
                }
                reader.Close();
            } catch
            {
                return "";
            }
            str += "\n\n------- Contenu de la commande -------";
            List<int> velos = GetVelos(idCommande);
            List<string> pieces = GetPieces(idCommande);
            for(int i =0; i< velos.Count; i++)
            {
                str += $"\nVélo {velos[i]}";
            }
            for(int i =0; i < pieces.Count; i++)
            {
                str += $"\nPièce {pieces[i]}";
            }

            return str;

        }

        static string[] GetVelos()
        {
            string query = "SELECT * FROM bicyclette";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            try
            {
                List<string> liste = new List<string>();
                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int assemblage = reader.GetInt32(1);
                    string nom = reader.GetString(2);
                    int grandeur = reader.GetInt32(3);
                    decimal prix = reader.GetDecimal(4);
                    string ligneproduit = reader.GetString(5);
                    int stock = reader.GetInt32(6);

                    liste.Add($"{id} | {nom} | { grandeur } | {prix} | {ligneproduit} | {stock}");
                }
                reader.Close();

                return liste.ToArray();
            }
            catch
            {
                return null;
            }


        }

        static string GetVelo(int idVelo)
        {
            string query = "SELECT * FROM bicyclette, assemblage WHERE bicyclette.id_assemblage = assemblage.id AND bicyclette.id = @velo";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            commande.Parameters.AddWithValue("@velo", idVelo);
            try
            {
                string str = "";
                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    int id_assemblage = reader.GetInt32(1);
                    //nom;grandeur;prix;ligneproduit;stock();id;//cadreguidonfreinselle,da,dv,ra,rv,reflecteur,pedalier,ordinateur,panier
                    string nom = reader.GetString(2);
                    int grandeur = reader.GetInt32(3);
                    decimal prix = reader.GetDecimal(4);
                    string ligneProduit = reader.GetString(5);
                    int stock = reader.GetInt32(6);
                    // 7 = id_assemblage
                    string cadre = (reader.IsDBNull(8) ? "NULL" : reader.GetString(8));
                    string guidon = (reader.IsDBNull(9) ? "NULL" : reader.GetString(9));
                    string freins = (reader.IsDBNull(10) ? "NULL" : reader.GetString(10));
                    string selle = (reader.IsDBNull(11) ? "NULL" : reader.GetString(11));
                    string da = (reader.IsDBNull(12) ? "NULL" : reader.GetString(12));
                    string dv = (reader.IsDBNull(13) ? "NULL" : reader.GetString(13));
                    string ra = (reader.IsDBNull(14) ? "NULL" : reader.GetString(14));
                    string rv = (reader.IsDBNull(15) ? "NULL" : reader.GetString(15));
                    string reflecteurs = (reader.IsDBNull(16) ? "NULL" : reader.GetString(16));
                    string pedalier = (reader.IsDBNull(17) ? "NULL" : reader.GetString(17));
                    string ordinateur = (reader.IsDBNull(18) ? "NULL" : reader.GetString(18));
                    string panier = (reader.IsDBNull(19) ? "NULL" : reader.GetString(19));

                    str += $"{id} | {nom} | { grandeur } | {prix} | {ligneProduit} | {stock}";
                    str += "\n\nCadre | Guidon | Freins | Selle | Dérailleur Av. | Dérailleur Arr. | Roue Av. | Roue Arr. | Réflecteurs | Pédalier | Ordinateur | Panier";
                    str += $"\n{cadre.PadRight(5)} | {guidon.PadRight(6)} | {freins.PadRight(6)} | {selle.PadRight(5)} | {da.PadRight(14)} | {dv.PadRight(15)} | {ra.PadRight(8)} | {rv.PadRight(9)} | {reflecteurs.PadRight(11)} | {pedalier.PadRight(8)} | {ordinateur.PadRight(10)} | {panier}";
                }
                reader.Close();
                return str;
            } catch
            {
                return "";
            }
        }

        static string[] GetPieces()
        {
            string query = "SELECT id, prix, stock, fournisseur.nom FROM piece LEFT JOIN fournisseur ON piece.fournisseur = fournisseur.siret;";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            try
            {
                List<string> str = new List<string>();

                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    string id = reader.GetString(0);
                    decimal prix = reader.GetDecimal(1);
                    int stock = reader.GetInt32(2);
                    string fournisseur = reader.GetString(3);

                    str.Add( $"{id} | {prix}€ | {stock} | {fournisseur}");
                }
                reader.Close();

                return str.ToArray();
            } catch
            {
                return null;
            }
        }

        static string[] VentesPieces()
        {
            string query = "SELECT piece.id, COUNT(achat.id) FROM piece LEFT JOIN achat on piece.id = achat.id_piece GROUP BY piece.id;";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            try
            {
                List<string> str = new List<string>();

                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    string id = reader.GetString(0);

                    str.Add($"Pièce {reader.GetString(0)}, nb ventes : {reader.GetInt32(1)}");
                }

                reader.Close();

                return str.ToArray();
            }
            catch
            {
                return null;
            }
        }

        static string[] VentesVelos()
        {
            string query = "SELECT bicyclette.id, COUNT(achat.id) FROM bicyclette LEFT JOIN achat on bicyclette.id = achat.id_bicyclette GROUP BY bicyclette.id;";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            try
            {
                List<string> str = new List<string>();

                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    string id = reader.GetString(0);

                    str.Add($"Vélo {reader.GetInt32(0)}, nb ventes : {reader.GetInt32(1)}");
                }

                reader.Close();

                return str.ToArray();
            }
            catch
            {
                return null;
            }
        }

        static (string, int, int, int) GetFidelio(int id)
        {


            string query = "SELECT * FROM `fidelio` WHERE id_programme = @prog;";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            commande.Parameters.AddWithValue("@prog", id);

            string nom = null;
            int rabais = 0;
            int cout = 0;
            int duree = 0;

            try
            {
                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    nom = reader.GetString(1);
                    cout = reader.GetInt32(2);
                    duree = reader.GetInt32(3);
                    rabais = reader.GetInt32(4);
                }
                reader.Close();

                return (nom, cout, duree, rabais);
            }
            catch
            {
                
                return (null, 0, 0, 0);
            }
        }

        static string ExportFidelioXML()
        {


            string query = "SELECT * FROM `fidelio`;";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = query;
            string XML = "<fidelio>";
            try
            {
                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    XML += "\n\t<programme>";
                    XML += $"\n\t\t<id-programme>{reader.GetInt32(0)}</id-programme>";
                    XML += $"\n\t\t<nom>{reader.GetString(1)}</nom>";
                    XML += $"\n\t\t<cout>{reader.GetInt32(2)}</cout>";
                    XML += $"\n\t\t<duree>{reader.GetInt32(3)}</duree>";
                    XML += $"\n\t\t<rabais>{reader.GetInt32(4)}</rabais>";
                    XML += "\n\t</programme>";
                }
                reader.Close();

            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            XML += "\n</fidelio>";

            return XML;
        }

        static int GetRabais(int cliendId)
        {
            // Remise entreprise

            string query = "SELECT remise FROM `client` LEFT JOIN compagnie ON compagnie.id = client.id_compagnie WHERE client.id_client = @client;";
            MySqlCommand cmd = connexion.CreateCommand();
            cmd.CommandText = query;
            cmd.Parameters.AddWithValue("@client", cliendId);

            int rabais = 0;

            try
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if(!reader.IsDBNull(0))
                        rabais = Math.Max(rabais, reader.GetInt32(0));
                }
                reader.Close();
            }
            catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                rabais = Math.Max(rabais, 0);
            }


            string req = "SELECT rabais FROM `adhere` NATURAL JOIN fidelio WHERE adhere.id_client = @client AND date_fin > @dateajd ORDER BY id_programme DESC LIMIT 1;";
            MySqlCommand commande = connexion.CreateCommand();
            commande.CommandText = req;
            commande.Parameters.AddWithValue("@client", cliendId);
            commande.Parameters.AddWithValue("@dateajd", DateTime.Now.ToString("u").Substring(0, 10));
            
            try
            {
                MySqlDataReader reader = commande.ExecuteReader();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                        rabais = Math.Max(rabais, reader.GetInt32(0));
                }
                reader.Close();
                
            } catch (Exception e)
            {
                //Console.WriteLine(e.Message);
                rabais = Math.Max(rabais, 0);
            }

            return rabais;
        }

        static string[] GetTotalCommandes()
        {
            Dictionary<int, decimal> totaux = new Dictionary<int, decimal>();

            string[] clients = GetClientsParticuliers();
            for(int i = 0; i< clients.Length; i++)
            {
                int idclient = Convert.ToInt32(clients[i].Split(' ')[0]);
                totaux.Add(idclient, 0);
            }

            string req = "SELECT client.id_client, COALESCE(SUM(piece.prix * (100 - commande.remise) / 100), 0) + COALESCE(SUM(bicyclette.prix * (100 - commande.remise) / 100), 0) FROM achat LEFT JOIN piece ON piece.id = achat.id_piece LEFT JOIN bicyclette ON achat.id_bicyclette = bicyclette.id LEFT JOIN commande ON commande.id = achat.id_commande LEFT JOIN client ON client.id_client = commande.id_client GROUP BY client.id_client;";
            MySqlCommand cmd = connexion.CreateCommand();
            cmd.CommandText = req;
            try
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    decimal tot = reader.GetDecimal(1);
                    totaux[id] = tot;
                }
                reader.Close();
            } catch
            {

            }

            string[] chaines = new string[clients.Length];
            for (int i = 0; i < clients.Length; i++)
            {
                int idclient = Convert.ToInt32(clients[i].Split(' ')[0]);
                string nom = clients[i].Split(' ')[1];
                string prenom = clients[i].Split(' ')[2];
                chaines[i] = $"{idclient} {nom} {prenom} : {totaux[idclient]}€";
            }

            return chaines;


        }

        static string[] MeilleursClient(int count = 1)
        {


            Dictionary<int, string> clientNames = new Dictionary<int, string>();

            string[] clients = GetClientsParticuliers();
            for (int i = 0; i < clients.Length; i++)
            {
                int idclient = Convert.ToInt32(clients[i].Split(' ')[0]);
                string nom = clients[i].Split(' ')[1];
                string prenom = clients[i].Split(' ')[2];
                clientNames.Add(idclient, $"{nom} {prenom}");
            }


            List<string> meilleursClients = new List<string>();

            string req = "SELECT client.id_client, COALESCE(SUM(piece.prix * (100 - commande.remise) / 100), 0) + COALESCE(SUM(bicyclette.prix * (100 - commande.remise) / 100), 0) AS total FROM achat LEFT JOIN piece ON piece.id = achat.id_piece LEFT JOIN bicyclette ON achat.id_bicyclette = bicyclette.id LEFT JOIN commande ON commande.id = achat.id_commande LEFT JOIN client ON client.id_client = commande.id_client GROUP BY client.id_client ORDER BY total DESC LIMIT @count;";
            MySqlCommand cmd = connexion.CreateCommand();
            cmd.CommandText = req;
            cmd.Parameters.AddWithValue("@count", count);
            try
            {
                MySqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = reader.GetInt32(0);
                    decimal tot = reader.GetDecimal(1);

                    meilleursClients.Add($"{id}, {clientNames[id]}, {tot}€");
                }
                reader.Close();
            }
            catch
            {

            }

            return meilleursClients.ToArray();


        }

        static Dictionary<string, int> PiecesParFournisseur()
        {

            Dictionary<string, int> d = new Dictionary<string, int>();
            string req = "SELECT fournisseur.nom, COUNT(*) FROM piece LEFT JOIN fournisseur ON fournisseur.siret = piece.fournisseur GROUP BY fournisseur.siret;";
            MySqlCommand cmd = connexion.CreateCommand();
            cmd.CommandText = req;
            try
            {
                MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    d.Add(r.GetString(0), r.GetInt32(1));
                }
                r.Close();
            } catch
            {

            }

            return d;
        }

        static string[] ClientParProgramme()
        {
            string req = "SELECT CONCAT(client.nom, ' ', client.prenom), COALESCE(MAX(id_programme), 0), MAX(adhere.date_fin) FROM client LEFT JOIN adhere ON adhere.id_client = client.id_client WHERE adhere.date_fin > @today GROUP BY client.id_client ORDER BY COALESCE(MAX(id_programme), 0) DESC;";
            MySqlCommand cmd = connexion.CreateCommand();
            cmd.CommandText = req;
            cmd.Parameters.AddWithValue("@today", DateTime.Now.ToString("u").Substring(0, 10));
            List<string> liste = new List<string>();

            try
            {
                MySqlDataReader r = cmd.ExecuteReader();
                while (r.Read())
                {
                    liste.Add($"{r.GetString(0).PadRight(24)} | Programme: {r.GetInt32(1)} | Expire : {r.GetString(2)}");
                }
                r.Close();
            } catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }

            return liste.ToArray();
        }

        static string[] StatsCommandes()
        {


            string query = "SELECT commande.id, COUNT(piece.id), COUNT(bicyclette.id),COALESCE(SUM(piece.prix * (100 - commande.remise) / 100), 0) + COALESCE(SUM(bicyclette.prix * (100 - commande.remise) / 100), 0) FROM achat LEFT JOIN piece ON piece.id = achat.id_piece LEFT JOIN bicyclette ON achat.id_bicyclette = bicyclette.id LEFT JOIN commande ON commande.id = achat.id_commande LEFT JOIN client ON client.id_client = commande.id_client GROUP BY commande.id;";
            MySqlCommand cmd = connexion.CreateCommand();
            cmd.CommandText = query;
            List<string> liste = new List<string>();

            List<int> ids = new List<int>();
            List<int> nbPieces = new List<int>();
            List<int> nbVelos = new List<int>();
            List<double> montants = new List<double>();
            try
            {
                MySqlDataReader r = cmd.ExecuteReader();
                while(r.Read())
                {
                    ids.Add(r.GetInt32(0));
                    nbPieces.Add(r.GetInt32(1));
                    nbVelos.Add(r.GetInt32(2));
                    montants.Add(r.GetDouble(3));
                }
                r.Close();
            } catch(Exception e)
            {

            }

            double avgPieces = 0;
            double avgVelos = 0;
            double avgMontants = 0;
            for(int i = 0; i < nbPieces.Count; i++)
            {
                avgPieces += nbPieces[i];
                avgMontants += montants[i];
                avgVelos += nbVelos[i];
            }

            avgPieces /= (double)nbPieces.Count;
            avgVelos /= (double)nbPieces.Count;
            avgMontants /= (double)nbPieces.Count;

            liste.Add($"Nombre de pièces moyen : {avgPieces}");
            liste.Add($"Nombre de vélos moyen : {avgVelos}");
            liste.Add($"Montant moyen : {avgMontants}");
            return liste.ToArray();

        }

    }
}

