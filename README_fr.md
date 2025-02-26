# Projet EasySave - ProSoft

[![Version](https://img.shields.io/badge/version-1.0-blue)](https://github.com/Cesi-AlbanCalvo/EasySave/releases/tag/v1.0)
[![Version](https://img.shields.io/badge/version-1.1-blue)](https://github.com/Cesi-AlbanCalvo/EasySave/releases/tag/v1.1)
[![Version](https://img.shields.io/badge/version-2.0-blue)](https://github.com/Cesi-AlbanCalvo/EasySave/releases/tag/v2.0)
![Langues](https://img.shields.io/badge/langues-Fran%C3%A7ais%20%7C%20English-green)

---

🌍 **Ce projet est disponible en plusieurs langues :**  
🇬🇧 [English](README.md) | 🇫🇷 [Français (actuel)](README_fr.md)

---

## 📚 Table des Matières
- [À propos du projet](#à-propos-du-projet)
- [Objectifs](#objectifs)
- [Livrables](#livrables)
- [Calendrier](#calendrier)
- [Outils et méthodes](#outils-et-méthodes)
- [Contributeurs](#contributeurs)
- [Résumé des Versions](#résumé-des-versions)

---

## 🌟 À propos du projet

EasySave est un logiciel de sauvegarde performant et évolutif, développé dans le cadre du programme de développement de ProSoft.

---

## 🎯 Objectifs

- Développement d'un logiciel de sauvegarde multi-version.
- Documentation utilisateur et support technique.
- Gestion efficace des versions du logiciel.

---

## 📦 Livrables

### **Version 1.0** - 06/02/2025
✅ Première version stable du logiciel.  
✅ Documentation utilisateur et support client.  
✅ Diagrammes UML associés.

### **Version 2.0 & 1.1** - 14/02/2025
✅ Améliorations et corrections de la v1.0.  
✅ Nouvelle version avec des fonctionnalités avancées.  
✅ Mise à jour de la documentation et des diagrammes UML.

### **Version 3.0** - 28/02/2025
✅ Fonctionnalités finales et optimisation du logiciel.  
✅ Documentation complète et notes de mise à jour.  
✅ Version prête pour distribution commerciale.

---

## 📅 Calendrier

### **FISA Informatique : Janvier - Février 2025**
- **27/01/2025** : Début du projet.
- **06/02/2025** : Livraison de la version 1.0.
- **14/02/2025** : Livraison de la version 2.0 et 1.1.
- **28/02/2025** : Livraison de la version 3.0.

### **FISE Informatique : Avril - Mai 2025**
- **24/04/2025** : Début du projet.
- **09/05/2025** : Livraison du premier livrable.
- **19/05/2025** : Livraison du deuxième livrable.
- **28/05/2025** : Finalisation et tests.
- **02/06/2025** : Soutenance finale.

---

## 🛠 Outils et méthodes

- **IDE** : Visual Studio 2022.
- **Gestion de version** : GitHub.
- **Outil UML** : Visual Paradigm.
- **Langage** : C# (.NET 8.0).

---

## 👥 Contributeurs

| Nom | Email |
|------|--------------------------|
| Alban Calvo | alban.calvo1@viacesi.fr |
| Evan Joasson | evan.joasson@viacesi.fr |
| Jonas Mionnet | jonas.mionnet@viacesi.fr |
| Matheo Pinget | matheo.pinget@viacesi.fr |

---
# Manuel de l'utilisateur - Version 2.0

## Introduction

Ce logiciel vous permet de créer et de gérer des tâches de sauvegarde avec un support de chiffrement. La nouvelle version inclut une interface graphique, une sécurité renforcée et la prise en charge d'outils de chiffrement externes.

## Installation

1. Téléchargez l'installateur et lancez le programme d'installation.
2. Suivez les instructions à l'écran pour terminer l'installation.
3. Lancez l'application depuis le menu Démarrer ou le raccourci sur le bureau.

## Fonctionnalités

- **Interface graphique** : Interface WPF facile à utiliser.
- **Support multilingue** : Disponible en anglais et en français.
- **Modes de sauvegarde** : Sauvegarde unique ou séquentielle.
- **Tâches de sauvegarde illimitées** : Aucune restriction sur le nombre de tâches.
- **Fichier de log quotidien** : Suivi de l'avancement des sauvegardes, y compris le temps de chiffrement.
- **Fichier de statut** : Fournit un aperçu des statuts des sauvegardes.
- **Détection de logiciels métier** : Option pour arrêter les sauvegardes si un logiciel métier est détecté.
- **Support en ligne de commande** : Permet l'automatisation et les scripts.
- **Outil de chiffrement externe** : Supporte CryptoSoft pour des sauvegardes sécurisées.

## Comment utiliser

### Créer une tâche de sauvegarde

1. Ouvrez l'application.
2. Cliquez sur "Nouvelle sauvegarde" et configurez la source, la destination et l'horaire.
3. Cliquez sur "Enregistrer" pour enregistrer la tâche.

### Lancer une sauvegarde

1. Sélectionnez une tâche de sauvegarde et cliquez sur "Exécuter" pour démarrer immédiatement.
2. L'application traitera les fichiers selon le mode sélectionné.

### Voir les logs et les statuts

1. Allez dans la section "Logs" pour voir les rapports quotidiens.
2. Vérifiez la section "Statut" pour les résumés des tâches.

## Dépannage

- Assurez-vous que les chemins source et destination sont accessibles.
- Vérifiez les logs pour d'éventuels messages d'erreur.
- Si les sauvegardes s'arrêtent de manière inattendue, vérifiez les paramètres de détection des logiciels métier.
- Si vous utilisez CryptoSoft, confirmez son installation et sa configuration.

## 📊 Résumé des Versions

| Fonction | Version 1.0 | Version 1.1 | Version 2.0 |
|----------|------------|------------|------------|
| Interface Graphique | Console | Console | Graphique |
| Multi-langues | Anglais et Français | Anglais et Français | Anglais et Français |
| Travaux de sauvegarde | Limité à 5 | Limité à 5 | Illimité |
| Fichier Log journalier | Oui | Oui | Oui |
| Utilisation d'une DLL pour le fichier log | Oui | Oui | Oui |
| Fichier État | Oui | Oui | Oui |
| Type de fonctionnement Sauvegarde | Mono ou séquentielle | Mono ou séquentielle | Mono ou séquentielle |
| Arrêt si détection du logiciel métier | Non | Non | Oui |
| Ligne de commande | Oui | Oui | Identique version 1.0 |
| Utilisation du logiciel de cryptage externe "CryptoSoft" | Non | Non | Oui |

---

