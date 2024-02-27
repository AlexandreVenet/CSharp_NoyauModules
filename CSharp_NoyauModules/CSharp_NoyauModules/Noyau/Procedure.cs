using CSharp_NoyauModules.Noyau.Module;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_NoyauModules.Noyau
{
	internal class Procedure
	{
		#region Champs

		private string _nomModule;
		private Type _moduleType;
		private object _module;

		private IModule _moduleInstanceIModule;
		//private Type _moduleInstanceIModuleType;
		private List<Etape> _etapes;

		private bool _procedureActive = true;
		private int _etapeEnCoursNumero;
		private Procedure _procedureInstance;
		private Type _procedureInstanceType;

		#endregion



		#region Constructeurs

		public Procedure()
		{
			_procedureInstance = this;
			_procedureInstanceType = _procedureInstance.GetType();
		}

		#endregion



		#region Méthodes de démarrage

		public void Executer(string[] args)
		{
			if (!ArgsEstFourni(args))
			{
				EcrireErreur("Le programme requiert un nom de module en argument.");
				return;
			}

			DefinirNomModule(args[0]);

			DefinirTypeModule();

			if (!TypeModuleEstValide())
			{
				EcrireErreur($"Le type {_nomModule} est inconnu.");
				return;
			}

			InstancierModule();

			if (!ModuleEstIModule())
			{
				EcrireErreur($"Le module {_nomModule} doit implémenter IModule.");
				return;
			}

			ObtenirIModule();

			ObtenirEtapesModule();

			if (EtapesNull())
			{
				EcrireErreur("Les étapes doivent être configurées (valeur null).");
				return;
			}

			if (EtapesSansIndex())
			{
				EcrireErreur("Les étapes doivent avoir au moins une valeur (longueur de 0).");
				return;
			}

			if (EtapesAvecDoublons())
			{
				EcrireErreur("Les étapes doivent être uniques.");
				return;
			}

			Console.WriteLine("Exécution des étapes de la procédure");

			try
			{
				for (_etapeEnCoursNumero = 0; _etapeEnCoursNumero < _etapes.Count; _etapeEnCoursNumero++)
				{
					if (!_procedureActive) break;

					string etapeActuelle = _etapes[_etapeEnCoursNumero].ToString();

					EcrireEnCouleur($"\n{_etapeEnCoursNumero}. {_etapes[_etapeEnCoursNumero]}", ConsoleColor.DarkYellow);

					// Lancer la méthode de même nom qu'une valeur de l'enum Etape.
					// Ceci pour éviter de faire un switch géant.
					MethodInfo methodeDeProcedure = _procedureInstanceType.GetMethod(etapeActuelle, BindingFlags.Instance | BindingFlags.NonPublic);
					// Appel synchrone de méthode async et ce sans zombification
					Task tache = (Task)methodeDeProcedure.Invoke(_procedureInstance, null);
					tache.Wait();
				}
			}
			catch (Exception e)
			{
				Exception baseException = e.GetBaseException();
				Type baseExceptionType = baseException.GetType();

				EcrireErreur("=====================");
				EcrireErreur("Procédure interrompue");
				EcrireErreur("=====================");
				EcrireErreur("BaseException");
				EcrireErreur(string.Empty);
				EcrireErreur("Type : " + baseExceptionType.ToString());
				EcrireErreur("Message : " + baseException.Message);
				EcrireErreur("---------------------");
				EcrireErreur("Exception");
				EcrireErreur(string.Empty);
				EcrireErreur(GetType().ToString());
				EcrireErreur(e.ToString());
			}
			finally
			{
				ArreterProcedure();
			}

		}

		private bool ArgsEstFourni(string[] args)
		{
			if (args == null || args.Length == 0 || string.IsNullOrEmpty(args[0])) return false;
			return true;
		}

		private void DefinirNomModule(string nomModule)
		{
			_nomModule = nomModule;
		}

		private void DefinirTypeModule()
		{
			// https://stackoverflow.com/questions/8499593/c-sharp-how-to-check-if-namespace-class-or-method-exists-in-c

			string[] espaceDeNomListe = typeof(Procedure).Namespace.Split('.');
			// index 0 : namespace principal de l'application

			string espaceNomModule = $"{espaceDeNomListe[0]}.Modules.{_nomModule}.Module";
			// Ex : TEST_NoyauModules.Modules.Test.Module

			_moduleType = Type.GetType(typeName: espaceNomModule, throwOnError: false, ignoreCase: true);
		}

		private bool TypeModuleEstValide()
		{
			return _moduleType != null;
		}

		private void InstancierModule()
		{
			_module = Activator.CreateInstance(_moduleType);
		}

		private bool ModuleEstIModule()
		{
			return _module is IModule;
		}

		private void ObtenirIModule()
		{
			// On traite un IModule et non pas le type du module en tant que tel car ce type est inutile.
			_moduleInstanceIModule = (IModule)_module;

			//_moduleInstanceIModuleType = _moduleInstanceIModule.GetType();
			// Le type de l'instance sert d'ordinaire à explorer celle-ci.
			// Ici, pas utile car on va utiliser l'instance avec le polymorphisme.
		}

		private void ObtenirEtapesModule()
		{
			_etapes = _moduleInstanceIModule.ObtenirEtapes();
		}

		private bool EtapesNull()
		{
			return _etapes == null;
		}

		private bool EtapesSansIndex()
		{
			return _etapes.Count == 0;
		}

		private bool EtapesAvecDoublons()
		{
			List<Etape> etapesUniques = new();

			foreach (Etape item in _etapes)
			{
				if (etapesUniques.Contains(item))
				{
					return true;
				}
				etapesUniques.Add(item);
			}

			return false;
		}

		private void ArreterProcedure()
		{
			_procedureActive = false;
		}

		#endregion



		#region Méthodes de procédure

		// Les méthodes ont le même nom que les entrées de l'enum Etape.

		private async Task Preparer()
		{
			VerifierImplementeInterface<IPreparation>();

			IPreparation module = (IPreparation)_moduleInstanceIModule;
			string texte = module.ObtenirPreparationTexte();

			Console.WriteLine(texte);
		}

		private async Task DireBonjour()
		{
			VerifierImplementeInterface<IBonjour>();

			EcrireDetail("Attendre 3 secondes...");
			await Task.Delay(3000);

			// Tester la capture d'exception :
			//throw new ArgumentException("Tadam !");

			IBonjour module = (IBonjour)_moduleInstanceIModule;
			string texte = module.ObtenirBonjour();

			await Console.Out.WriteLineAsync(texte);
		}

		private async Task ObtenirNombre()
		{
			VerifierImplementeInterface<INombre>();
			INombre module = (INombre)_moduleInstanceIModule;

			// Le nombre est nullable. Donc, ici, on n'a pas encore de valeur (on a null).

			module.GenererNombre();
			// Le nombre est stocké dans l'objet.

			// L'interface INombre propose une méthode pour connaître ce nombre.
			Console.WriteLine($"Nombre = " + module.ObtenirNombre());
		}

		private async Task MultiplierNombre()
		{
			VerifierImplementeInterface<INombre>();

			// Récupérer le nombre. Celui-ci doit avoir été fourni par INombre.
			// Utiliser nullable pour tester si on dispose déjà d'une valeur.
			INombre moduleINombre = (INombre)_moduleInstanceIModule;
			int? nombre = moduleINombre.ObtenirNombre();
			if (nombre == null)
			{
				throw new NotImplementedException("Impossible de multiplier car le premier facteur vaut null. Implémenter l'interface INombre et définir l'étape correspondante de façon à ce qu'elle précède MultiplierNombre.");
			}

			VerifierImplementeInterface<INombreTraitement>();

			INombreTraitement module = (INombreTraitement)_moduleInstanceIModule;

			int facteur2 = module.ObtenirFacteur2();
			int resultat = module.Multiplier(nombre.Value);

			Console.WriteLine($"Facteur 2 posé par le module : {facteur2}");
			Console.WriteLine($"Traitement sur le nombre : {nombre.Value} x {facteur2} = {resultat}");
		}

		private async Task ObtenirDonnees()
		{
			VerifierImplementeInterface<IDonnees>();

			IDonnees module = (IDonnees)_moduleInstanceIModule;
			module.ChercherDonneesQuelquePart();
			// Le module contient maintenant les données.

			Console.WriteLine("OK");
		}

		private async Task TraiterDonnees()
		{
			VerifierImplementeInterface<IDonnees>();

			// On veut analyser la donnée spécifique du module, donnée qui fait objet d'un modèle contenant des propriétés.
			// On ne connait pas son type mais on sait que ses propriétés sont des types simples intégrés (Int, String, DateTime...).

			IDonnees module = (IDonnees)_moduleInstanceIModule;

			Type donneesType = module.ObtenirTypeDonnees();
			object donnees = module.ObtenirDonnees();

			Console.WriteLine($"Type de données : " + donneesType.Name);

			// Vérifier si on dispose déjà d'une donnée, celle-ci devant avoir été fournie par IDonnees.
			if (donnees == null)
			{
				throw new NotImplementedException("Impossible de traiter les données car elles valent null. Implémenter l'interface IDonnees et définir l'étape de façon à ce qu'elle précède l'étape TraiterDonnees.");
			}

			// Obtenir les propriétés du modèle de données et en consulter la valeur
			PropertyInfo[] tableauDesProprietes = donneesType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
			Console.WriteLine("Propriétés du type de données");
			for (int i = 0; i < tableauDesProprietes.Length; i++)
			{
				PropertyInfo item = tableauDesProprietes[i];
				object? valeur = donneesType.InvokeMember(
					name: item.Name,
					invokeAttr: BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
					binder: null,
					target: donnees,
					args: null);

				Console.Write($"\t{item.Name} : ");
				// Convertir objet? dans le type réel 
				// Switch ici équivalent à une chaîne de "if(valeur is int) else if(valeur is DateTime) else..."
				// Pour tester le case default, j'ai omis volontairement le type Byte, type utilisé pourtant par le modèle.
				switch (valeur)
				{
					case int:
						Console.Write((int)valeur + " (int)");
						break;
					case DateTime:
						Console.Write(((DateTime)valeur).ToString("dd/MM/yyyy hh:mm:ss.fff") + " (DateTime)");
						break;
					case string:
						Console.Write((string)valeur + " (string)");
						break;
					default:
						Console.Write(valeur.ToString() + " (type non reconnu)");
						break;
				}
				Console.Write('\n');
			}

		}

		#endregion



		#region Autres méthodes

		private void VerifierImplementeInterface<T>()
		{
			if (_moduleInstanceIModule == null)
			{
				throw new ArgumentNullException("Le module vaut null.");
			}

			if (_moduleInstanceIModule is not T)
			{
				throw new NotImplementedException($"Le module n'implémente pas l'interface {typeof(T).Name}.");
			}
		}

		#endregion



		#region Méthodes d'UI

		private void EcrireErreur(string message)
		{
			EcrireEnCouleur(message, ConsoleColor.DarkCyan);
		}

		private void EcrireDetail(string message)
		{
			EcrireEnCouleur(message, ConsoleColor.DarkGray);
		}

		private void EcrireEnCouleur(string message, ConsoleColor couleur)
		{
			Console.ForegroundColor = couleur;
			Console.WriteLine(message);
			Console.ResetColor();
		}

		#endregion

	}
}
