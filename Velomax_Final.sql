/*
CREATE USER 'velomax'@'%' IDENTIFIED WITH mysql_native_password BY 'password';
GRANT ALL PRIVILEGES ON velomax.* TO 'velomax'@'%';

CREATE USER 'bozo'@'%' IDENTIFIED WITH mysql_native_password BY 'bozo';
GRANT SELECT ON velomax.* TO 'bozo'@'%';

FLUSH PRIVILEGES;


*/


DROP DATABASE velomax;

CREATE DATABASE IF NOT EXISTS velomax;
USE velomax;

CREATE TABLE IF NOT EXISTS adresse(
	id INT NOT NULL AUTO_INCREMENT,
	voie VARCHAR(255),
    ville VARCHAR(100),
    code_postal VARCHAR(10),
    /*
    peut être donné comme un entier
    mais il est préférable d'avoir 02100
    et non pas 2100 -- d'où ce typage
    */
    province VARCHAR(100),
    PRIMARY KEY(id)
);

CREATE TABLE IF NOT EXISTS fournisseur(
	siret VARCHAR(50),
    nom VARCHAR(100),
    adresse INT,
    libelle INT,
    nom_contact VARCHAR(50),
    prenom_contact VARCHAR(50),
    PRIMARY KEY(siret),
    FOREIGN KEY (adresse) REFERENCES adresse(id) ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS piece(
	id VARCHAR(10),
	details VARCHAR(255), /* le mot description étant réservé et voulant garder une certaine logique */
    fournisseur VARCHAR(50),
    numcatalogue INT,
    prix DECIMAL,
    date_introduction DATETIME,
    date_discontinuation DATETIME,
    delai_approvisionnement INT, /* délai en jours */
    stock INT DEFAULT 0,
    PRIMARY KEY(id),
    FOREIGN KEY (fournisseur) REFERENCES fournisseur(siret) ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS compagnie(
	id INT NOT NULL AUTO_INCREMENT,
    nom VARCHAR(100),
    remise INT,
    PRIMARY KEY(id)
);

CREATE TABLE IF NOT EXISTS client(
	id_client INT NOT NULL AUTO_INCREMENT,
    nom VARCHAR(100),
    prenom VARCHAR(100),
    email VARCHAR(100),
    id_compagnie INT,
    telephone VARCHAR(40),
    adresse INT,
    PRIMARY KEY(id_client),
    FOREIGN KEY (adresse) REFERENCES adresse(id) ON UPDATE CASCADE,
    FOREIGN KEY (id_compagnie) REFERENCES compagnie(id) ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS fidelio(
	id_programme INT,
    nom VARCHAR(50),
    cout DECIMAL,
    duree INT,
    rabais INT,
    PRIMARY KEY(id_programme)
);

CREATE TABLE IF NOT EXISTS commande(
	id INT NOT NULL AUTO_INCREMENT,
    date_commande DATETIME,
    id_client INT,
    adresse INT,
    remise INT NOT NULL DEFAULT 0,
    PRIMARY KEY(id),
    FOREIGN KEY (id_client) REFERENCES client(id_client) ON UPDATE CASCADE,
    FOREIGN KEY (adresse) REFERENCES adresse(id) ON UPDATE CASCADE
);

/* Liaison des tables fidelio et client */
CREATE TABLE IF NOT EXISTS adhere(
	id INT,
    id_client INT,
    id_programme INT,
    date_adhesion DATETIME,
    date_fin DATETIME,
    PRIMARY KEY(id),
    FOREIGN KEY (id_client) REFERENCES client(id_client) ON UPDATE CASCADE,
    FOREIGN KEY (id_programme) REFERENCES fidelio(id_programme) ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS assemblage(
	id INT,
    cadre VARCHAR(10),
    guidon VARCHAR(10),
    freins VARCHAR(10),
    selle VARCHAR(10),
    derailleur_avant VARCHAR(10),
    derailleur_arriere VARCHAR(10),
    roue_avant VARCHAR(10),
    roue_arriere VARCHAR(10),
    reflecteurs VARCHAR(10),
    pedalier VARCHAR(10),
    ordinateur VARCHAR(10),
    panier VARCHAR(10),
    PRIMARY KEY (id),
	FOREIGN KEY (cadre) REFERENCES piece(id) ON UPDATE CASCADE,
	FOREIGN KEY (guidon) REFERENCES piece(id) ON UPDATE CASCADE,
	FOREIGN KEY (freins) REFERENCES piece(id) ON UPDATE CASCADE,
	FOREIGN KEY (selle) REFERENCES piece(id) ON UPDATE CASCADE,
	FOREIGN KEY (derailleur_avant) REFERENCES piece(id) ON UPDATE CASCADE,
	FOREIGN KEY (derailleur_arriere) REFERENCES piece(id) ON UPDATE CASCADE,
	FOREIGN KEY (roue_avant) REFERENCES piece(id) ON UPDATE CASCADE,
	FOREIGN KEY (roue_arriere) REFERENCES piece(id) ON UPDATE CASCADE,
	FOREIGN KEY (reflecteurs) REFERENCES piece(id) ON UPDATE CASCADE,
	FOREIGN KEY (pedalier) REFERENCES piece(id) ON UPDATE CASCADE,
	FOREIGN KEY (ordinateur) REFERENCES piece(id) ON UPDATE CASCADE,
	FOREIGN KEY (panier) REFERENCES piece(id) ON UPDATE CASCADE
);

CREATE TABLE IF NOT EXISTS bicyclette(
	id INT,
    id_assemblage INT,
    nom VARCHAR(50),
    grandeur INT, /* 1 Adulte, 2 Homme, 3 Dame, 4 Jeune, 5 Garçon, 6 Fille */
    prix DECIMAL,
    ligne_produit VARCHAR(32), /* 1 Classique, 2 VTT, 3 Vélo de course, 4 BMX */
    stock INT NOT NULL DEFAULT 0,
    PRIMARY KEY(id),
	FOREIGN KEY(id_assemblage) REFERENCES assemblage(id)
);

CREATE TABLE IF NOT EXISTS achat(
	id INT NOT NULL AUTO_INCREMENT,
	id_commande INT,
    id_bicyclette INT,
    id_piece VARCHAR(10),
    PRIMARY KEY(id),
    FOREIGN KEY (id_commande) REFERENCES commande(id) ON UPDATE CASCADE,
    FOREIGN KEY (id_bicyclette) REFERENCES bicyclette(id) ON UPDATE CASCADE,
    FOREIGN KEY (id_piece) REFERENCES piece(id) ON UPDATE CASCADE
);

/* Remplissage */

INSERT INTO fidelio(id_programme, nom, cout, duree, rabais) VALUES (1, "Fidélio", 15, 1, 5);
INSERT INTO fidelio(id_programme, nom, cout, duree, rabais) VALUES (2, "Fidélio Or", 25, 2, 8);
INSERT INTO fidelio(id_programme, nom, cout, duree, rabais) VALUES (3, "Fidélio Platine", 60, 2, 10);
INSERT INTO fidelio(id_programme, nom, cout, duree, rabais) VALUES (4, "Fidélio Max", 100, 3, 12);

/* Création d'un fournisseur */
INSERT INTO adresse(id, voie, ville, code_postal, province) VALUES (1, "1 Rue de la Paix", "Paris", 75001, "Île de France");
INSERT INTO fournisseur(siret, nom, adresse, libelle, nom_contact, prenom_contact) VALUES (30613890001294, "PEUGEOT", 1, 1, "Doe", "John");
INSERT INTO fournisseur(siret, nom, adresse, libelle, nom_contact, prenom_contact) VALUES (12345678901234, "DECATHLON", 1, 1, "Doe", "John");
INSERT INTO fournisseur(siret, nom, adresse, libelle, nom_contact, prenom_contact) VALUES (98765432234567, "GOSPORT", 1, 1, "Doe", "John");

/* Insertion des pièces */

INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("C32", "Cadre C32", 30613890001294, 332, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("C34", "Cadre C34", 30613890001294, 334, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("C76", "Cadre C76", 30613890001294, 376, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("C43", "Cadre C43", 30613890001294, 343, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("C43f", "Cadre C43f", 30613890001294, 3430, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("C44", "Cadre C44", 30613890001294, 344, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("C44f", "Cadre C44f", 30613890001294, 3440, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("C01", "Cadre C01", 30613890001294, 301, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("C02", "Cadre C02", 30613890001294, 302, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("C15", "Cadre C15", 30613890001294, 315, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("C87", "Cadre C87", 30613890001294, 387, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("C87f", "Cadre C87f", 30613890001294, 3870, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("C25", "Cadre C25", 30613890001294, 325, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("C26", "Cadre C26", 30613890001294, 326, 10);

INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("G7", "Guidon G7", 12345678901234, 77, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("G9", "Guidon G9", 12345678901234, 79, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("G12", "Guidon G12", 12345678901234, 712, 10);

INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("F3", "Freins F3", 12345678901234, 63, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("F9", "Freins F9", 12345678901234, 69, 10);

INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("S88", "Selle S88", 30613890001294, 1988, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("S37", "Selle S37", 30613890001294, 1937, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("S35", "Selle S35", 30613890001294, 1935, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("S02", "Selle S02", 30613890001294, 1902, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("S03", "Selle S03", 30613890001294, 1903, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("S36", "Selle S36", 30613890001294, 1936, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("S34", "Selle S34", 30613890001294, 1934, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("S87", "Selle S87", 30613890001294, 1987, 10);

INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("DV133", "Dérailleur avant DV133", 12345678901234, 422133, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("DV17", "Dérailleur avant DV17", 12345678901234, 42217, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("DV87", "Dérailleur avant DV87", 12345678901234, 42287, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("DV57", "Dérailleur avant DV57", 12345678901234, 42257, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("DV15", "Dérailleur avant DV15", 12345678901234, 42215, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("DV41", "Dérailleur avant DV41", 12345678901234, 42241, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("DV132", "Dérailleur avant DV132", 12345678901234, 422132, 10);


INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("DR56", "Dérailleur arrière DR56", 30613890001294, 41856, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("DR87", "Dérailleur arrière DR87", 30613890001294, 41887, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("DR86", "Dérailleur arrière DR86", 30613890001294, 41886, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("DR23", "Dérailleur arrière DR23", 30613890001294, 41823, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("DR76", "Dérailleur arrière DR76", 30613890001294, 41876, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("DR52", "Dérailleur arrière DR52", 30613890001294, 41852, 10);

INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("R45", "Roue R45", 30613890001294, 18145, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("R48", "Roue F48", 30613890001294, 18148, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("R12", "Roue R12", 30613890001294, 18112, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("R19", "Roue R19", 30613890001294, 18119, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("R1", "Roue R1", 30613890001294, 1811, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("R11", "Roue R11", 30613890001294, 18111, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("R44", "Roue R44", 30613890001294, 18144, 10);

INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("R46", "Roue R46", 30613890001294, 18146, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("R47", "Roue R47", 30613890001294, 18147, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("R32", "Roue R32", 30613890001294, 18132, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("R18", "Roue R18", 30613890001294, 18118, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("R2", "Roue R2", 30613890001294, 1812, 10);

INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("R02", "Réflecteurs R02", 30613890001294, 181802, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("R09", "Réflecteurs R09", 30613890001294, 181809, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("R10", "Réflecteurs R10", 30613890001294, 181810, 10);

INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("P12", "Pédalier P12", 98765432234567, 16012, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("P34", "Pédalier P34", 98765432234567, 16034, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("P1", "Pédalier P1", 98765432234567, 16001, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("P15", "Pédalier P15", 98765432234567, 16015, 10);

INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("O2", "Ordinateur O2", 98765432234567, 1502, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("O4", "Ordinateur O4", 98765432234567, 1504, 10);

INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("S01", "Panier S01", 98765432234567, 16101, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("S05", "Panier S05", 98765432234567, 16105, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("S73", "Panier S73", 98765432234567, 16173, 10);
INSERT INTO piece(id, details, fournisseur, numcatalogue, prix) VALUES ("S74", "Panier S74", 98765432234567, 16174, 10);

INSERT INTO assemblage(id, cadre, guidon, freins, selle, derailleur_avant, derailleur_arriere, roue_avant, roue_arriere, reflecteurs, pedalier, ordinateur, panier) VALUES (101, "C32", "G7", "F3", "S88", "DV133", "DR56", "R45", "R46", NULL, "P12", "O2", NULL);
INSERT INTO assemblage(id, cadre, guidon, freins, selle, derailleur_avant, derailleur_arriere, roue_avant, roue_arriere, reflecteurs, pedalier, ordinateur, panier) VALUES (102, "C34", "G7", "F3", "S88", "DV17", "DR87", "R48", "R47", NULL, "P12", NULL, NULL);
INSERT INTO assemblage(id, cadre, guidon, freins, selle, derailleur_avant, derailleur_arriere, roue_avant, roue_arriere, reflecteurs, pedalier, ordinateur, panier) VALUES (103, "C76", "G7", "F3", "S88", "DV17", "DR87", "R48", "R47", NULL, "P12", "O2", NULL);
INSERT INTO assemblage(id, cadre, guidon, freins, selle, derailleur_avant, derailleur_arriere, roue_avant, roue_arriere, reflecteurs, pedalier, ordinateur, panier) VALUES (104, "C76", "G7", "F3", "S88", "DV87", "DR86", "R12", "R32", NULL, "P12", NULL, NULL);
INSERT INTO assemblage(id, cadre, guidon, freins, selle, derailleur_avant, derailleur_arriere, roue_avant, roue_arriere, reflecteurs, pedalier, ordinateur, panier) VALUES (105, "C43", "G9", "F9", "S37", "DV57", "DR86", "R19", "R18", "R02", "P34", NULL, NULL);
INSERT INTO assemblage(id, cadre, guidon, freins, selle, derailleur_avant, derailleur_arriere, roue_avant, roue_arriere, reflecteurs, pedalier, ordinateur, panier) VALUES (106, "C44f", "G9", "F9", "S35", "DV57", "DR86", "R19", "R18", "R02", "P34", NULL, NULL);
INSERT INTO assemblage(id, cadre, guidon, freins, selle, derailleur_avant, derailleur_arriere, roue_avant, roue_arriere, reflecteurs, pedalier, ordinateur, panier) VALUES (107, "C43", "G9", "F9", "S37", "DV57", "DR87", "R19", "R18", "R02", "P34", "O4", NULL);
INSERT INTO assemblage(id, cadre, guidon, freins, selle, derailleur_avant, derailleur_arriere, roue_avant, roue_arriere, reflecteurs, pedalier, ordinateur, panier) VALUES (108, "C43f", "G9", "F9", "S35", "DV57", "DR87", "R19", "R18", "R02", "P34", "O4", NULL);
INSERT INTO assemblage(id, cadre, guidon, freins, selle, derailleur_avant, derailleur_arriere, roue_avant, roue_arriere, reflecteurs, pedalier, ordinateur, panier) VALUES (109, "C01", "G12", NULL, "S02", NULL, NULL, "R1", "R2", "R09", "P1", NULL, "S01");
INSERT INTO assemblage(id, cadre, guidon, freins, selle, derailleur_avant, derailleur_arriere, roue_avant, roue_arriere, reflecteurs, pedalier, ordinateur, panier) VALUES (110, "C02", "G12", NULL, "S03", NULL, NULL, "R1", "R2", "R09", "P1", NULL, "S05");
INSERT INTO assemblage(id, cadre, guidon, freins, selle, derailleur_avant, derailleur_arriere, roue_avant, roue_arriere, reflecteurs, pedalier, ordinateur, panier) VALUES (111, "C15", "G12", "F9", "S36", "DV15", "DR23", "R11", "R12", "R10", "P15", NULL, "S74");
INSERT INTO assemblage(id, cadre, guidon, freins, selle, derailleur_avant, derailleur_arriere, roue_avant, roue_arriere, reflecteurs, pedalier, ordinateur, panier) VALUES (112, "C87", "G12", "F9", "S36", "DV41", "DR76", "R11", "R12", "R10", "P15", NULL, "S74");
INSERT INTO assemblage(id, cadre, guidon, freins, selle, derailleur_avant, derailleur_arriere, roue_avant, roue_arriere, reflecteurs, pedalier, ordinateur, panier) VALUES (113, "C87f", "G12", "F9", "S34", "DV41", "DR76", "R11", "R12", "R10", "P15", NULL, "S73");
INSERT INTO assemblage(id, cadre, guidon, freins, selle, derailleur_avant, derailleur_arriere, roue_avant, roue_arriere, reflecteurs, pedalier, ordinateur, panier) VALUES (114, "C25", "G7", "F3", "S87", "DV132", "DR52", "R44", "R47", NULL, "P12", NULL, NULL);
INSERT INTO assemblage(id, cadre, guidon, freins, selle, derailleur_avant, derailleur_arriere, roue_avant, roue_arriere, reflecteurs, pedalier, ordinateur, panier) VALUES (115, "C26", "G7", "F3", "S87", "DV133", "DR52", "R44", "R47", NULL, "P12", NULL, NULL);

INSERT INTO bicyclette(id, id_assemblage, nom, grandeur, prix, ligne_produit) VALUES (101, 101, "Kilimandjaro", 1, 569, "VTT");
INSERT INTO bicyclette(id, id_assemblage, nom, grandeur, prix, ligne_produit) VALUES (102, 102, "NorthPole", 1, 329, "VTT");
INSERT INTO bicyclette(id, id_assemblage, nom, grandeur, prix, ligne_produit) VALUES (103, 103, "MontBlanc", 4, 399, "VTT");
INSERT INTO bicyclette(id, id_assemblage, nom, grandeur, prix, ligne_produit) VALUES (104, 104, "Hooligan", 4, 199, "VTT");
INSERT INTO bicyclette(id, id_assemblage, nom, grandeur, prix, ligne_produit) VALUES (105, 105, "Orléans", 2, 229, "Vélo de course");
INSERT INTO bicyclette(id, id_assemblage, nom, grandeur, prix, ligne_produit) VALUES (106, 106, "Orléans", 3, 229, "Vélo de course");
INSERT INTO bicyclette(id, id_assemblage, nom, grandeur, prix, ligne_produit) VALUES (107, 107, "BlueJay", 2, 349, "Vélo de course");
INSERT INTO bicyclette(id, id_assemblage, nom, grandeur, prix, ligne_produit) VALUES (108, 108, "BlueJay", 3, 349, "Vélo de course");
INSERT INTO bicyclette(id, id_assemblage, nom, grandeur, prix, ligne_produit) VALUES (109, 109, "Trail Explorer", 6, 129, "Classique");
INSERT INTO bicyclette(id, id_assemblage, nom, grandeur, prix, ligne_produit) VALUES (110, 110, "Trail Explorer", 5, 129, "Classique");
INSERT INTO bicyclette(id, id_assemblage, nom, grandeur, prix, ligne_produit) VALUES (111, 111, "Night Hawk", 4, 189, "Classique");
INSERT INTO bicyclette(id, id_assemblage, nom, grandeur, prix, ligne_produit) VALUES (112, 112, "Tierra Verde", 2, 199, "Classique");
INSERT INTO bicyclette(id, id_assemblage, nom, grandeur, prix, ligne_produit) VALUES (113, 113, "Tierra Verde", 3, 199, "Classique");
INSERT INTO bicyclette(id, id_assemblage, nom, grandeur, prix, ligne_produit) VALUES (114, 114, "Mud Zinger I", 4, 279, "BMX");
INSERT INTO bicyclette(id, id_assemblage, nom, grandeur, prix, ligne_produit) VALUES (115, 115, "Mud Zinger II", 1, 359, "BMX");

/* on met du stock pour chaque pièce */
SET SQL_SAFE_UPDATES = 0;
UPDATE piece SET stock = 4;
UPDATE bicyclette SET stock = 1;

/* SELECT bicyclette.nom, bicyclette.grandeur, assemblage.cadre, assemblage.guidon, assemblage.freins, assemblage.selle, assemblage.derailleur_avant, assemblage.derailleur_arriere, assemblage.roue_avant, assemblage.roue_arriere, assemblage.reflecteurs, assemblage.pedalier, assemblage.ordinateur, assemblage.panier
FROM bicyclette
LEFT JOIN assemblage
ON bicyclette.id_assemblage = assemblage.id; */