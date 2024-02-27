using CSharp_NoyauModules.Modules.Test.Test;
using CSharp_NoyauModules.Noyau.Module;
using CSharp_NoyauModules.Noyau;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSharp_NoyauModules.Modules.Test
{
	// Implémenter les interfaces utiles. Supprimer une entrée pour tester les Exceptions.
	internal class Module : IModule, IPreparation, IBonjour, INombre, INombreTraitement, IDonnees
	{
		#region IModule

		// Ordre (libre) des étapes pour ce module. Doublons interdits.
		// Mélanger les étapes pour tester les Exceptions.
		private List<Etape> _etapes = new()
		{
			Etape.DireBonjour,
			Etape.Preparer,
			Etape.ObtenirNombre,
			Etape.MultiplierNombre,
			Etape.ObtenirDonnees,
			Etape.TraiterDonnees,
		};

		public List<Etape> ObtenirEtapes() => _etapes;

		#endregion



		#region IPreparation

		public string ObtenirPreparationTexte() => "Module préparé.";

		#endregion



		#region IBonjour


		public string ObtenirBonjour() => "Bonjour, toi !";

		#endregion




		#region INombre

		private int? _nombre;

		public void GenererNombre()
		{
			Random random = new Random();
			_nombre = random.Next(10, 20);
		}

		public int? ObtenirNombre() => _nombre;

		#endregion



		#region INombreTraitement

		int _facteur2 = 12;

		public int ObtenirFacteur2() => _facteur2;

		public int Multiplier(int facteur1) => facteur1 * _facteur2;

		#endregion



		#region IDonnees

		private Personne _donnees;

		public void ChercherDonneesQuelquePart()
		{
			_donnees = new()
			{
				Nom = "Toto",
				Age = 1,
				Date = DateTime.Now,
				Octet = 255,
			};
		}

		public Type ObtenirTypeDonnees() => typeof(Personne);

		public object ObtenirDonnees() => _donnees;

		#endregion
	}
}
